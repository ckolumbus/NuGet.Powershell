/*
   Copyright (c) Chris Drexler

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsCommon.New, "NuGetPackage")]
    public class NewNugetPackageCmdlet : PSCmdlet
    {

        [Parameter(Position = 0)]
        public string Id { get; set; }

        [Parameter(Position = 1)]
        public string Version { get; set; }

        [Parameter(Position = 2)]
        public string[] Authors { get; set; }

        [Parameter(Position = 3)]
        public string Description { get; set; }

        [Parameter(Position = 4)]
        public string ContentPath { get; set; }

        [Parameter()]
        public Hashtable Dependencies { get; set; }

        [Parameter()]
        public Hashtable FilesMapping { get; set; }

        [Parameter()]
        public string ManifestFile { get; set; } = "";

        [Parameter]
        public string Framework { get; set; } = "";

        [Parameter]
        public string OutputPath { get; set; } = ".";

        [Parameter]
        public string OutputFilename { get; set; }



        protected override void ProcessRecord()
        {
            string cwd = SessionState.Path.CurrentFileSystemLocation.Path;
            PackageBuilder builder;

            // load manifest file first, if provided
            if (!String.IsNullOrEmpty(ManifestFile))
            {
                string fullManifestFile = ManifestFile;
                if (!System.IO.Path.IsPathRooted(ManifestFile)) {
                    fullManifestFile = System.IO.Path.Combine(cwd, ManifestFile);
                }
                var fullManifestFilePath = System.IO.Path.GetFullPath(fullManifestFile);
                WriteVerbose($"Loading Nuspec file : {fullManifestFilePath}");
                var dir = System.IO.Path.GetDirectoryName(fullManifestFilePath);
                var file = System.IO.Path.GetFileName(fullManifestFilePath);

                builder = new PackageBuilder(fullManifestFilePath, dir, null, false, false);
            } else {
                builder = new PackageBuilder();
            }

            // overwrite / add metadatat provided via command line
            WriteVerbose("Setting Metadata from cmdline arguments");
            var manifestMetadata = new ManifestMetadata
            {
                Id = Id ?? builder.Id,
                Version = !String.IsNullOrEmpty(Version) ? NuGetVersion.Parse(Version) : builder.Version,
                Description = Description ?? builder.Description,
                Authors = Authors != null ? Authors.AsEnumerable() : builder.Authors
            };
            builder.Populate(manifestMetadata);

            WriteVerbose("Effective Metadata:");
            WriteVerbose("     Id=" + builder.Id);
            WriteVerbose("     Version=" + builder.Version);
            WriteVerbose("     Authors=" + String.Join(",", builder.Authors));
            WriteVerbose("     Description=" + builder.Description);

            // add dedicated ContentPath to files list
            if (!String.IsNullOrEmpty(ContentPath))
            {
                WriteVerbose("Adding all(!) files from base content path recursively.");
                string fullContentPath = ContentPath;
                if (! System.IO.Path.IsPathRooted(ContentPath) ) {
                    fullContentPath = System.IO.Path.Combine(cwd, ContentPath);
                }

                builder.AddFiles(fullContentPath, "**", "");
            }

            // add file mapping
            if (FilesMapping != null ) {
                foreach (var mappingKey in FilesMapping.Keys)
                {
                    var source = mappingKey.ToString();

                    Hashtable targetHash = FilesMapping[source] as Hashtable;
                    if (targetHash is null)
                    {
                        WriteVerbose($"target value not a hash, trying to convert dirctly to target string");
                        // not a hashtable, assume shortcut definition with target as string
                        targetHash = new Hashtable
                        {
                            { "target", FilesMapping[source] as string },
                            { "exclude", null }
                        };
                    }

                    if (source.Contains("/")) {
                        // nuget client sdk resolves subdirectories in a defined way only when backslashes are used
                        // see https://github.com/NuGet/NuGet.Client/blob/824a3c7d8823c3be1cf48e08e5f1993d2e8eb4ab/src/NuGet.Core/NuGet.Packaging/PackageCreation/Authoring/PackageBuilder.cs#L1108
                        throw new ArgumentException($"File mapping source '{source}' contains '/' (slashes), sub-directory resolution only works correctly with '\\' (backslashes)!");
                    }

                    try
                    {
                        var targetDir = targetHash["target"] as string;
                        var targetExclude = targetHash["exclude"] as string;
                        WriteVerbose($"Adding Files mapping from Arguments to Manifest: {source} -> {targetDir} (excludes={targetExclude})");
                        builder.AddFiles(cwd, source, targetDir, targetExclude);
                    } catch (Exception e) {
                        throw new ArgumentException($"Target defintion invalid! Check target parameters for source={source} : {e.Message}");
                    }
                }
            }

            // add dependency list
            if (Dependencies != null)
            {
                WriteVerbose("Adding Dependency information");
                ISet<PackageDependency> packageDependencies = new HashSet<PackageDependency>();

                foreach (var dep in Dependencies.Keys)
                {
                    string dependencyName = (string)dep;

                    Hashtable depParameters = Dependencies[dep] as Hashtable;
                    if (depParameters is null)
                    {
                        WriteVerbose($"Dependency value not a hash, trying to convert dirctly to version string");
                        // not a hashtable, assume shortcut definition with version as string
                        depParameters = new Hashtable
                        {
                            { "Version", Dependencies[dep] as string },
                            { "Includes", null },
                            { "Excludes", null }
                        };
                    }

                    VersionRange version;
                    string versionString = depParameters["Version"] as string;
                    if (versionString is null)
                    {
                        throw new ArgumentException("Dependency Version could not be read");
                    } else {
                        version = VersionRange.Parse(versionString);
                    }

                    // TODO: is dependency specific framework modeling needed?
                    //string framework = depParameters["Framework"] as string ?? "";

                    string[] includes;
                    string includesString = depParameters["Includes"] as string;
                    if (!(includesString is null))
                    {
                        includes = includesString.Split(',').Select(s => s.Trim()).ToArray();
                    }
                    else
                    {
                        includes = new string[] { };
                    }

                    string[] excludes;
                    string excludesString = depParameters["Excludes"] as string;
                    if (!(excludesString is null))
                    {
                        excludes = includesString.Split(',').Select(s => s.Trim()).ToArray();
                    }
                    else
                    {
                        excludes = new string[] { };
                    }

                    WriteVerbose(String.Format("Adding Dependency from Arguments to Manifest: name={0} version={1}, includes={2} excludes={3}",
                        dependencyName, version,
                        String.Join(",", includes),
                        String.Join(",", excludes)
                    ));
                    var pd = new PackageDependency(dependencyName, version, includes, excludes);
                    packageDependencies.Add(pd);
                }
                builder.DependencyGroups.Add(new PackageDependencyGroup(NuGetFramework.Parse(Framework), packageDependencies));
            }

            if (null == OutputFilename) {
                OutputFilename = $"{builder.Id}.{builder.Version}.nupkg";
            }

            string fullOutputPath = OutputPath;
            if (!System.IO.Path.IsPathRooted(OutputPath))
            {
                fullOutputPath = System.IO.Path.Combine(cwd, OutputPath);
            }

            var fullOutputFilePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(fullOutputPath, OutputFilename));
            if (File.Exists(fullOutputFilePath)) {
                throw new Exception($"File Exists: {fullOutputFilePath}");
            }

            using (FileStream nupkgFileStream = new FileStream(fullOutputFilePath, FileMode.Create) ) {
                builder.Save(nupkgFileStream);
            };

            WriteObject(fullOutputFilePath);
        }
    }
}

