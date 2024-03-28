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
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsDiagnostic.Resolve, "NuGetPackageVersion",
        DefaultParameterSetName = "Object")]
    [OutputType(typeof(SourcePackageDependencyInfo))]
    public class ResolveNugetPackageVersionCmdlet : AsyncCmdlet
    {
        [Parameter(ParameterSetName = "Object", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Object-ConfigFile", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Object-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PackageDependency[] PackageDependency { get; set; }

        [Parameter(ParameterSetName = "Args", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string Id { get; set; }

        [Parameter(ParameterSetName = "Args", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        public string VersionRange { get; set; }

        [Parameter]
        public string Framework { get; set; } = "any";

        [Parameter(ParameterSetName = "Object-ConfigFile", Mandatory = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true)]
        public string ConfigFile { get; set; } = "";

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
        public PSCredential SourceCredential { get; set; } = null;


        private NuGetFramework nuGetFramework;
        private List<SourceRepository> repositories = new List<SourceRepository>();
        private SourceCacheContext cache = new SourceCacheContext();
        private string cwd;

        protected override Task BeginProcessingAsync()
        {
            WriteVerbose($"ParameterSetName : {ParameterSetName}");

            nuGetFramework = NuGetFramework.ParseFolder(Framework);

            cwd = SessionState.Path.CurrentFileSystemLocation.Path;
            if ( Source != "") {
                var rootedSource = Helpers.GetRootedPath(Source, root: cwd);
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

            if (ParameterSetName.StartsWith("Args")) {
                var versionRange = Versioning.VersionRange.Parse(VersionRange);
                PackageDependency = new[] { new PackageDependency(Id, versionRange) };
            }

            foreach (var packageDependency in PackageDependency)
            {
                var result = await Helpers.GetBestMatchingVersion(packageDependency, nuGetFramework, repositories, cache, logger: this);
                var pi = new PackageIdentity(Id, result);
                WriteObject(await Helpers.GetPackageDependencyInfo(pi, nuGetFramework, repositories, cache, logger: this));
            }
        }
    }
}
