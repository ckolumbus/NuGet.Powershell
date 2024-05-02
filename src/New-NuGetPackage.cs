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

        [Parameter]
        public Hashtable Dependencies { get; set; }

        [Parameter]
        public Hashtable FrameworkAssemblies { get; set; }

        [Parameter]
        public Hashtable FilesMapping { get; set; }

        [Parameter]
        public string ManifestFile { get; set; } = "";

        [Parameter]
        public string OutputPath { get; set; } = ".";

        [Parameter]
        public string OutputFilename { get; set; }

        [Parameter]
        public Hashtable RepositoryInfo { get; set; }

        [Parameter]
        public Hashtable Properties { get; set; }


        protected override void ProcessRecord()
        {
            string cwd = SessionState.Path.CurrentFileSystemLocation.Path;

            Manifest manifest = null;
            string manifestBaseDir = null;
            // load manifest file first, if provided
            if (!String.IsNullOrEmpty(ManifestFile))
            {
                string fullManifestFile = ManifestFile;
                if (!System.IO.Path.IsPathRooted(ManifestFile)) {
                    fullManifestFile = System.IO.Path.Combine(cwd, ManifestFile);
                }
                var fullManifestFilePath = System.IO.Path.GetFullPath(fullManifestFile);
                WriteVerbose($"Loading Nuspec file : {fullManifestFilePath}");
                manifestBaseDir = System.IO.Path.GetDirectoryName(fullManifestFilePath);

                using (Stream stream = File.OpenRead(fullManifestFilePath))
                {
                    manifest = (Properties == null) ?
                            Manifest.ReadFrom(stream, validateSchema: true) :
                            Manifest.ReadFrom(stream, p => (string)Properties[p], validateSchema: true);
                }
            }

            PackageBuilder builder = new PackageBuilder();

            // overwrite / add metadata provided via command line
            WriteVerbose("Setting Metadata from cmdline arguments (when provided)");
            var metadata = (manifest != null) ? manifest.Metadata : new ManifestMetadata();

            metadata.Id = Id ?? metadata.Id;
            metadata.Version = !String.IsNullOrEmpty(Version) ? NuGetVersion.Parse(Version) : metadata.Version;
            metadata.Description = Description ?? metadata.Description;
            metadata.Authors = Authors != null ? Authors.AsEnumerable() : metadata.Authors;

            WriteVerbose("Effective Metadata (nuspec + cmdline):");
            WriteVerbose("     Id=" + metadata.Id);
            WriteVerbose("     Version=" + metadata.Version);
            WriteVerbose("     Authors=" + String.Join(",", metadata.Authors));
            WriteVerbose("     Description=" + metadata.Description);

            // populate builder with metadata and files from manifest
            builder.Populate(metadata);
            if (manifest != null && manifest.HasFilesNode) {
                builder.PopulateFiles(manifestBaseDir,manifest.Files);
            }

            // add dedicated ContentPath to files list
            if (!String.IsNullOrEmpty(ContentPath))
            {
                WriteVerbose("Adding all(!) files from base content path recursively.");
                string fullContentPath = ContentPath;
                if (! System.IO.Path.IsPathRooted(ContentPath) ) {
                    fullContentPath = System.IO.Path.Combine(cwd, ContentPath);
                }

                builder.AddFiles(fullContentPath, @"**\*", "");
            }

            // add Repository meta data
            if (RepositoryInfo != null)
            {
                string[] repoInfoElements= { "Type", "Url", "Branch", "Commit" };
                string allowedElements = string.Join(",", repoInfoElements);

                if (RepositoryInfo.Count == 0){
                    throw new ArgumentException($"Repository Metadata empty! Please provide at least one of '{allowedElements}'");
                }

                WriteVerbose($"Setting Repository Metadata");
                var repoData = new RepositoryMetadata();
                foreach (string key in RepositoryInfo.Keys) {
                    if (repoInfoElements.Contains(key)) {
                        var property = repoData.GetType().GetProperty(key);
                        property.SetValue(repoData,RepositoryInfo[key].ToString());
                    } else {
                        throw new ArgumentException($"Repository Metadata Key unknown: {key}, allowed values '{allowedElements}'");
                    }
                }
                builder.Repository = repoData;
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
            if (Dependencies != null) {
                WriteVerbose("Adding Dependency information");
                Dictionary<string, ISet<PackageDependency>> packageDependencyGroups = new Dictionary<string, ISet<PackageDependency>>();

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
                            { "Framework", "" },
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

                    string framework = depParameters["Framework"] as string ?? "";

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
                        excludes = excludesString.Split(',').Select(s => s.Trim()).ToArray();
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
                    if (!packageDependencyGroups.ContainsKey(framework)) {
                        ISet<PackageDependency> packageDependencies = new HashSet<PackageDependency>();
                        packageDependencyGroups.Add(framework, packageDependencies);
                    }
                    packageDependencyGroups[framework].Add(pd);
                }
                foreach (var framework in packageDependencyGroups.Keys) {
                    builder.DependencyGroups.Add(new PackageDependencyGroup(NuGetFramework.Parse(framework), packageDependencyGroups[framework]));
                }
            }

            // Framework Assembly References
            if (FrameworkAssemblies != null)
            {
                WriteVerbose("Adding Framework Assembly Reference information");
                Dictionary<string, ISet<FrameworkAssemblyReference>> frameworkAssemblyReferenceGroups = new Dictionary<string, ISet<FrameworkAssemblyReference>>();

                foreach (var assembly in FrameworkAssemblies.Keys)
                {
                    string assemblyName = (string)assembly;
                    var assemblyTargetFramework = NuGetFramework.ParseFolder((string)FrameworkAssemblies[assembly]);


                    WriteVerbose(String.Format(
                            "Adding Framework Assembly Reference from Arguments to Manifest: name={0} targetFramework={1}",
                            assemblyName, assemblyTargetFramework
                        )
                    );

                    var far = ((string)FrameworkAssemblies[assembly] == "") ?
                            new FrameworkAssemblyReference(assemblyName, new List<NuGetFramework>() { }) :
                            new FrameworkAssemblyReference(assemblyName, new List<NuGetFramework>() { assemblyTargetFramework });

                    builder.FrameworkReferences.Add(far);
                }
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

            try
            {
                using (FileStream nupkgFileStream = new FileStream(fullOutputFilePath, FileMode.Create))
                {
                    builder.Save(nupkgFileStream);
                };
                WriteObject(fullOutputFilePath);
            } catch {
                // clean up in case of exception, don't leave incorrect nupkg file
                File.Delete(fullOutputFilePath);
                throw;
            }
        }
    }
}

