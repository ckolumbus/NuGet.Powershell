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
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsCommon.Find, "NuGetPackage")]
    [OutputType(typeof(PackageSearchMetadata))]
    public class FindNugetPackageCmdlet : AsyncCmdlet
    {
        [Parameter(ParameterSetName = "Object", Mandatory = true, Position = 0, ValueFromPipeline = true) ]
        [Parameter(ParameterSetName = "Object-ConfigFile", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Object-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string[] SearchString { get; set; }

        [Parameter(ParameterSetName = "Object") ]
        [Parameter(ParameterSetName = "Object-ConfigFile")]
        [Parameter(ParameterSetName = "Object-ConfigArgs")]
        public SwitchParameter IncludePrerelease { get; set; }

        [Parameter(ParameterSetName = "Object")]
        [Parameter(ParameterSetName = "Object-ConfigFile")]
        [Parameter(ParameterSetName = "Object-ConfigArgs")]
        public int MaxResult { get; set; } = 100;

        [Parameter(ParameterSetName = "ConfigFile")]
        [Parameter(ParameterSetName = "Object-ConfigFile", Mandatory = true)]
        public string ConfigFile { get; set; } = "";

        [Parameter(ParameterSetName = "ConfigArgs", Mandatory = true)]
        [Parameter(ParameterSetName = "Object-ConfigArgs", Mandatory = true)]
        public string Source { get; set; } = "";

        [Parameter(ParameterSetName = "ConfigArgs")]
        [Parameter(ParameterSetName = "Object-ConfigArgs")]
        [ValidateSet(new string[] { "2", "3" })]
        public int SourceProtocolVersion { get; set; } = 2;

        [Parameter(ParameterSetName = "ConfigArgs")]
        [Parameter(ParameterSetName = "Object-ConfigArgs")]
        public PSCredential SourceCredential { get; set; } = null;

        private List<SourceRepository> repositories = new List<SourceRepository>();
        private SourceCacheContext cache = new SourceCacheContext {DirectDownload=true, NoCache=true};
        private string cwd;

        protected override Task BeginProcessingAsync()
        {
            cwd = SessionState.Path.CurrentFileSystemLocation.Path;
            if ( Source != "") {
                var rootedSource = Helpers.GetRootedPath(Source, cwd);
                var repository = Helpers.GetRepository(rootedSource, SourceCredential?.GetNetworkCredential(), SourceProtocolVersion, logger:this);
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

            foreach (var search in SearchString)
            {
                WriteVerbose($"Searching for : {search}");
                foreach (var repo in repositories)
                {
                    WriteVerbose($"      in repo : {repo}");

                    PackageSearchResource resource = await repo.GetResourceAsync<PackageSearchResource>();
                    SearchFilter searchFilter = new SearchFilter(includePrerelease: IncludePrerelease);

                    IEnumerable<IPackageSearchMetadata> results = await resource.SearchAsync(
                        search,
                        searchFilter,
                        skip: 0,
                        take: MaxResult,
                        this,
                        cancellationToken);

                    foreach (var pkg in results) {
                        WriteObject(pkg);
                    }
                }
            }
        }
    }
}
