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
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "NuGetPackageDependencyInfo",
        DefaultParameterSetName = "Args")]
    [OutputType(typeof(SourcePackageDependencyInfo))]

    public class GetNugetPackageDependencyInfoCmdlet : AsyncCmdlet
    {
        [Parameter(ParameterSetName = "Object", Mandatory = true, Position = 0, ValueFromPipeline = true) ]
        [Parameter(ParameterSetName = "Object-ConfigFile", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Object-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PackageIdentity[] PackageIdentity { get; set; }

        [Parameter(ParameterSetName = "DepObject", Mandatory = true, Position = 0, ValueFromPipeline = true) ]
        [Parameter(ParameterSetName = "DepObject-ConfigFile", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "DepObject-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PackageDependency[] PackageDependency { get; set; }

        [Parameter( ParameterSetName = "Args", Mandatory = true, Position = 0)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 0)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 0)]
        public string Id { get; set; }

        [Parameter(ParameterSetName = "Args", Position = 1)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Position = 1)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Position = 1)]
        public string VersionRange { get; set; } = "*";

        [Parameter]
        public SwitchParameter Recurse { get; set; }

        [Parameter]
        public string Framework { get; set; } = "any";

        [Parameter]
        public SwitchParameter RemoveTopLevelDependencies { get; set; }

        [Parameter(ParameterSetName = "Object-ConfigFile", Mandatory = true)]
        [Parameter(ParameterSetName = "DepObject-ConfigFile", Mandatory = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true)]
        public string ConfigFile { get; set; } = "";

        [Parameter(ParameterSetName = "Object-ConfigArgs", Mandatory = true)]
        [Parameter(ParameterSetName = "DepObject-ConfigArgs", Mandatory = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true)]
        public string Source { get; set; } = "";

        [Parameter(ParameterSetName = "Object-ConfigArgs")]
        [Parameter(ParameterSetName = "DepObject-ConfigArgs")]
        [Parameter(ParameterSetName = "Args-ConfigArgs")]
        [ValidateSet(new string[] { "2", "3" })]
        public int SourceProtocolVersion { get; set; } = 3;

        [Parameter(ParameterSetName = "Object-ConfigArgs")]
        [Parameter(ParameterSetName = "DepObject-ConfigFile")]
        [Parameter(ParameterSetName = "Args-ConfigArgs")]
        public PSCredential SourceCredential { get; set; } = PSCredential.Empty;

        private NuGetFramework nuGetFramework;
        private List<SourceRepository> repositories = new List<SourceRepository>();

        private HashSet<SourcePackageDependencyInfo> packages;
        private SourceCacheContext cache = new SourceCacheContext();
        private string cwd;

        protected override Task BeginProcessingAsync()
        {
            nuGetFramework = NuGetFramework.ParseFolder(Framework);

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

            packages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);

            return Task.CompletedTask;
        }

        protected override async Task ProcessRecordAsync()
        {
            WriteVerbose("start processing");

            var cancellationToken = CancellationToken.None;

            if (ParameterSetName.StartsWith("Args"))
            {
                WriteVerbose($"Resolving Version for {Id} {VersionRange}");
                var versionRange = Versioning.VersionRange.Parse(VersionRange);
                var version = await Helpers.GetBestMatchingVersion(Id, versionRange, nuGetFramework, repositories, cache, logger: this);
                PackageIdentity = new[] { new PackageIdentity(Id, version) };
            }

            // Resolve Dependency Object Input to a Package Identity
            if (ParameterSetName.StartsWith("DepObject"))
            {
                List<PackageIdentity> tmpList = new List<PackageIdentity>();
                foreach (var dep in PackageDependency)
                {
                    WriteVerbose($"Resolving Version for {dep.Id} {dep.VersionRange}");
                    var result = await Helpers.GetBestMatchingVersion(dep, nuGetFramework, repositories, cache, logger: this);
                    tmpList.Add(new PackageIdentity(dep.Id, result));
                }
                PackageIdentity = tmpList.ToArray();
            }

            WriteVerbose("Analyzing dependencies");
            // Find all potential dependencies
            foreach (var package in PackageIdentity)
            {
                var dep = await Helpers.GetPackageDependencyInfo(package, nuGetFramework, repositories, cache, logger: this);
                packages.Add(dep);

                if (Recurse)
                {
                    await Helpers.ListAllPackageDependencies(
                        package,
                        repositories,
                        nuGetFramework,
                        packages,
                        Recurse,
                        cache,
                        logger: this,
                        cancellationToken);
                }
            }
        }

        protected override Task EndProcessingAsync()
        {
            foreach (var p in packages)
            {
                if (Recurse && RemoveTopLevelDependencies && PackageIdentity.Contains(p))
                {
                    WriteVerbose($"Remove top-level package from output: {p}");
                    continue;
                }
                WriteObject(p);
            }
            return Task.CompletedTask;
        }
    }
}

