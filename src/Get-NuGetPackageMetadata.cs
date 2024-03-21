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
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "NuGetPackageMetadata")]
    [OutputType(typeof(PackageIdentity))]
    public class GetNugetPackageMetadataCmdlet : AsyncCmdlet
    {
        [Parameter(ParameterSetName = "Object", Mandatory = true, Position = 0, ValueFromPipeline = true) ]
        [Parameter(ParameterSetName = "Object-ConfigFile", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Object-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PackageIdentity[] PackageIdentity { get; set; }

        [Parameter(ParameterSetName = "Args", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter( ParameterSetName = "Args", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
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
        public int SourceProtocolVersion { get; set; } = 3;

        [Parameter(ParameterSetName = "ConfigArgs")]
        [Parameter(ParameterSetName = "Object-ConfigArgs")]
        [Parameter(ParameterSetName = "Args-ConfigArgs")]
        public PSCredential SourceCredential { get; set; } = PSCredential.Empty;

        private List<SourceRepository> repositories = new List<SourceRepository>();
        private SourceCacheContext cache = new SourceCacheContext();
        private string cwd;

        protected override Task BeginProcessingAsync()
        {
            cwd = SessionState.Path.CurrentFileSystemLocation.Path;
            if ( Source != "") {
                var rootedSource = Helpers.GetRootedPath(Source, cwd);
                var repository = Helpers.GetRepository(rootedSource, SourceCredential.GetNetworkCredential(), SourceProtocolVersion, logger:this);
                repositories.Add(repository);
            } else {
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

            foreach (var identity in PackageIdentity)
            {
                WriteVerbose($"Searching Metadata for : {identity}");
                IPackageSearchMetadata packageMetadata = null;
                foreach (var repo in repositories)
                {
                    WriteVerbose($"      in repo : {repo}");
                    PackageMetadataResource resource = await repo.GetResourceAsync<PackageMetadataResource>();

                    packageMetadata = await resource.GetMetadataAsync(
                        identity,
                        cache,
                        log: this,
                        cancellationToken);

                    if (!(packageMetadata is null))
                    {
                        WriteVerbose($"Found Metadata : {packageMetadata}");
                        break;
                    }
                }
                if (packageMetadata is null) {
                        WriteWarning($"No Metadata for found for : {identity}");
                } else {
                    WriteObject(packageMetadata);
                }
            }
        }
    }
}
