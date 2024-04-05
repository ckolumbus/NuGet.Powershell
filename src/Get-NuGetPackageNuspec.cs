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

using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "NuGetPackageNuspec")]
    [OutputType(typeof(XmlDocument))]
    public class GetNugetPackageNuspecCmdlet : AsyncCmdlet
    {
        [Parameter(ParameterSetName = "Directory", Mandatory = true, Position = 0)]
        public string[] Directory { get; set; }

        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string[] Path { get; set; }

        [Parameter(ParameterSetName = "Object", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Object-ConfigFile", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Object-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PackageIdentity[] PackageIdentity { get; set; }

        [Parameter(ParameterSetName = "Args", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter(ParameterSetName = "Args", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        public string Version { get; set; }

        [Parameter(ParameterSetName = "ConfigFile")]
        [Parameter(ParameterSetName = "Object-ConfigFile", Mandatory = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true)]
        public string ConfigFile { get; set; } = "";

        [Parameter(ParameterSetName = "ConfigArgs", Mandatory = true)]
        [Parameter(ParameterSetName = "Object-ConfigArgs", Mandatory = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true)]
        public string Source { get; set; } = "";

        [Parameter(ParameterSetName = "ConfigArgs")]
        [Parameter(ParameterSetName = "Object-ConfigArgs")]
        [Parameter(ParameterSetName = "Args-ConfigArgs")]
        [ValidateSet(new string[] { "2", "3" })]
        public int SourceProtocolVersion { get; set; } = 2;

        [Parameter(ParameterSetName = "ConfigArgs")]
        [Parameter(ParameterSetName = "Object-ConfigArgs")]
        [Parameter(ParameterSetName = "Args-ConfigArgs")]
        public PSCredential SourceCredential { get; set; } = null;

        private List<SourceRepository> repositories = new List<SourceRepository>();
        private SourceCacheContext cache = new SourceCacheContext();
        private string cwd;

        protected override Task BeginProcessingAsync()
        {
            cwd = SessionState.Path.CurrentFileSystemLocation.Path;
            if (Source != "")
            {
                var rootedSource = Helpers.GetRootedPath(Source, cwd);
                var repository = Helpers.GetRepository(rootedSource, SourceCredential?.GetNetworkCredential(), SourceProtocolVersion, logger: this);
                repositories.Add(repository);
            }
            else
            {
                var settings = Helpers.GetSettings(ConfigFile, ctx: this, logger: this);
                var newRepositories = Helpers.GetRepositories(settings, logger: this);
                repositories.AddRange(newRepositories);
            }

            return Task.CompletedTask;
        }

        protected override async Task ProcessRecordAsync()
        {
            var cancellationToken = CancellationToken.None;

            if (ParameterSetName.StartsWith("Args"))
            {
                PackageIdentity = new[] { new PackageIdentity(Id, NuGetVersion.Parse(Version)) };
            }

            if ((null != Directory) && (Directory.Length > 0))
            {
                foreach (var packageDirectory in Directory)
                {
                    var fullPackageDirectory = packageDirectory;
                    if (!System.IO.Path.IsPathRooted(packageDirectory))
                    {
                        fullPackageDirectory = System.IO.Path.Combine(cwd, packageDirectory);
                    }
                    fullPackageDirectory = System.IO.Path.GetFullPath(fullPackageDirectory);

                    WriteVerbose($"Reading {fullPackageDirectory}");

                    var packageReader = new PackageFolderReader(fullPackageDirectory);
                    var nuspecStream = packageReader.GetNuspec();
                    var xml = new XmlDocument();
                    xml.Load(nuspecStream);
                    WriteObject(xml);
                }
            } else if ((null != Path) && (Path.Length > 0))
            {
                foreach (var pathArg in Path)
                {
                    var path = Helpers.GetRootedPath(pathArg, root: cwd);
                    WriteVerbose($"Getting NuSpec from : {path}");
                    var packageReader = new PackageArchiveReader(path);

                    if (packageReader is null)
                    {
                        WriteWarning($"No Package found for : {path}");
                    }
                    else
                    {
                        WriteVerbose($"Output nuspec for : {path}");
                        var nuspecStream = await packageReader.GetNuspecAsync(cancellationToken);
                        var xml = new XmlDocument();
                        xml.Load(nuspecStream);
                        WriteObject(xml);
                    }
                }
            }
            else if ((null != PackageIdentity) && (PackageIdentity.Length > 0))
            {
                foreach (var identity in PackageIdentity)
                {
                    WriteVerbose($"Searching Package for : {identity}");
                    Stream nuspecStream = null;
                    foreach (var repo in repositories)
                    {
                        WriteVerbose($"      in repo : {repo}");
                        FindPackageByIdResource resource = await repo.GetResourceAsync<FindPackageByIdResource>();

                        try
                        {
                            MemoryStream packageStream = new MemoryStream();

                            await resource.CopyNupkgToStreamAsync(
                                identity.Id,
                                identity.Version,
                                packageStream,
                                cache,
                                logger: this,
                                cancellationToken);

                            PackageArchiveReader packageReader = new PackageArchiveReader(packageStream);
                            nuspecStream = await packageReader.GetNuspecAsync(cancellationToken);
                            WriteVerbose($"Found Package {identity}");
                            break;
                        }
                        catch { }
                    }
                    if (nuspecStream is null)
                    {
                        WriteWarning($"No Package found for : {identity}");
                    }
                    else
                    {
                        WriteVerbose($"Output nuspec for : {identity}");
                        var xml = new XmlDocument();
                        xml.Load(nuspecStream);
                        WriteObject(xml);
                    }
                }
            }
        }
    }
}
