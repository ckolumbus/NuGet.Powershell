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
    [Cmdlet(VerbsCommon.New, "NuGetPackage",
        DefaultParameterSetName = "File")]
    public class NewNugetPackageCmdlet : PSCmdlet
    {

        [Parameter(Mandatory=true,Position = 0, ParameterSetName = "File")]
        public string ManifestFile { get; set; }

        [Parameter(Mandatory=true,Position = 0, ParameterSetName = "Args")]
        public string Path { get; set; }

        [Parameter(Mandatory=true,Position = 1, ParameterSetName = "Args")]
        public string Id { get; set; }

        [Parameter(Mandatory=true, Position = 2, ParameterSetName = "Args")]
        public string Version { get; set; }

        [Parameter(Mandatory=true, Position = 3, ParameterSetName = "Args")]
        public string[] Authors { get; set; }

        [Parameter(Mandatory = true, Position = 4, ParameterSetName = "Args")]
        public string Description { get; set; }

        [Parameter(Position = 5, ParameterSetName = "Args")]
        public Hashtable Dependencies { get; set; }

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
            switch (ParameterSetName)
            {
                case "File":
                    string fullManifestFile = ManifestFile;
                    if (! System.IO.Path.IsPathRooted(ManifestFile) ) {
                        fullManifestFile = System.IO.Path.Combine(cwd, ManifestFile);
                    }
                    var fullManifestFilePath = System.IO.Path.GetFullPath(fullManifestFile);
                    var dir = System.IO.Path.GetDirectoryName(fullManifestFilePath);
                    var file = System.IO.Path.GetFileName(fullManifestFilePath);

                    builder = new PackageBuilder(fullManifestFilePath, dir, null, false, false);

                    Id = builder.Id;
                    Version = builder.Version.ToString();
                    break;

                case "Args":
                    string fullInputPath = Path;
                    if (! System.IO.Path.IsPathRooted(Path) ) {
                        fullInputPath = System.IO.Path.Combine(cwd, Path);
                    }

                    var manifestMetadata = new ManifestMetadata
                    {
                        Id = Id,
                        Version = NuGetVersion.Parse(Version),
                        Description = Description,
                        Authors = Authors
                    };

                    var manifestFile = new ManifestFile
                    {
                        Source = "**"
                    };

                    var manifestFileList = new List<ManifestFile>
                    {
                        manifestFile
                    };


                    builder = new PackageBuilder();

                    if (Dependencies.Count > 0)
                    {
                        ISet<PackageDependency> packageDependencies = new HashSet<PackageDependency>();

                        foreach (var dep in Dependencies.Keys)
                        {
                            string dependencyName = (string)dep;

                            Hashtable depParameters = Dependencies[dep] as Hashtable;
                            if (depParameters is null)
                            {
                                throw new ArgumentException("Dependency parameter is not a Hashtable");
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
                            string includesString = depParameters["includes"] as string;
                            if (!(includesString is null))
                            {
                                includes = includesString.Split(',').Select(s => s.Trim()).ToArray();
                            }
                            else
                            {
                                includes = new string[] { };
                            }

                            string[] excludes;
                            string excludesString = depParameters["excludes"] as string;
                            if (!(excludesString is null))
                            {
                                excludes = includesString.Split(',').Select(s => s.Trim()).ToArray();
                            }
                            else
                            {
                                excludes = new string[] { };
                            }

                            var pd = new PackageDependency(dependencyName, version, includes, excludes);
                            packageDependencies.Add(pd);
                        }
                        builder.DependencyGroups.Add(new PackageDependencyGroup(NuGetFramework.Parse(Framework), packageDependencies));
                    }

                    builder.PopulateFiles(fullInputPath, manifestFileList);
                    builder.Populate(manifestMetadata);

                    break;
                default:
                    throw new ArgumentException("Bad ParameterSet Name");
            } // switch (ParameterSetName...

            if (null == OutputFilename) {
                OutputFilename = $"{Id}.{Version}.nupkg";
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

