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
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsDiagnostic.Resolve, "NuGetPackageDependencies",
        DefaultParameterSetName = "InfoObject")]
    [OutputType(typeof(SourcePackageDependencyInfo))]

    public class ResolveNugetPackageDependenciesCmdlet : AsyncCmdlet
    {
        [Parameter(ParameterSetName = "DepObject", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "DepObject-ConfigFile", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "DepObject-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PackageDependency[] PackageDependency { get; set; }

        [Parameter(ParameterSetName = "InfoObject", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "InfoObject-ConfigFile", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "InfoObject-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public SourcePackageDependencyInfo[] SourcePackageDependencyInfo { get; set; }

        [Parameter(ParameterSetName = "Args", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter(ParameterSetName = "Args", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        public string VersionRange { get; set; }

        [Parameter]
        public string Framework { get; set; } = "any";

        [Parameter(ParameterSetName = "ConfigFile", Mandatory = true)]
        [Parameter(ParameterSetName = "DepObject-ConfigFile", Mandatory = true)]
        [Parameter(ParameterSetName = "InfoObject-ConfigFile", Mandatory = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true)]
        public string ConfigFile { get; set; } = "";

        [Parameter(ParameterSetName = "ConfigArgs", Mandatory = true)]
        [Parameter(ParameterSetName = "DepObject-ConfigArgs", Mandatory = true)]
        [Parameter(ParameterSetName = "InfoObject-ConfigArgs", Mandatory = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true)]
        public string Source { get; set; } = "";

        [Parameter(ParameterSetName = "ConfigArgs")]
        [Parameter(ParameterSetName = "DepObject-ConfigArgs")]
        [Parameter(ParameterSetName = "InfoObject-ConfigArgs")]
        [Parameter(ParameterSetName = "Args-ConfigArgs")]
        [ValidateSet(new string[] { "2", "3" })]
        public int SourceProtocolVersion { get; set; } = 3;

        [Parameter(ParameterSetName = "ConfigArgs")]
        [Parameter(ParameterSetName = "DepObject-ConfigArgs")]
        [Parameter(ParameterSetName = "InfoObject-ConfigArgs")]
        [Parameter(ParameterSetName = "Args-ConfigArgs")]
        public PSCredential SourceCredential { get; set; } = PSCredential.Empty;

        private NuGetFramework nuGetFramework;
        private HashSet<PackageIdentity> packageIdentities;
        private HashSet<SourcePackageDependencyInfo> packages;
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
                var repository = Helpers.GetRepository(rootedSource, SourceCredential.GetNetworkCredential(), SourceProtocolVersion, logger:this);
                repositories.Add(repository);
            } else {
                var settings = Helpers.GetSettings(ConfigFile, ctx: this, logger: this);
                var newRepositories = Helpers.GetRepositories(settings, logger: this);
                repositories.AddRange(newRepositories);
            }

            packages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
            packageIdentities = new HashSet<PackageIdentity>(PackageIdentityComparer.Default);

            return Task.CompletedTask;
        }

        protected override async Task ProcessRecordAsync()
        {
            WriteVerbose("start processing");
            WriteVerbose($"ParameterSetName : {ParameterSetName}");


            if (ParameterSetName.StartsWith("Args"))
            {
                PackageDependency = new[] {new PackageDependency(Id, Versioning.VersionRange.Parse(VersionRange) ) };
            }

            var cancellationToken = CancellationToken.None;

            if (ParameterSetName.StartsWith("DepObject") || ParameterSetName.StartsWith("Args"))
            {
                // Find all potential dependencies
                foreach (var packageDependency in PackageDependency)
                {
                    WriteVerbose($"Getting BestMatchingVersion for main dependency: {packageDependency.Id} {packageDependency.VersionRange}");

                    // first get best (==highest) matching for direct dependencies given on command line
                    var v = await Helpers.GetBestMatchingVersion(packageDependency, nuGetFramework, repositories, cache, logger: this);

                    WriteVerbose($"   resolved to {v}");

                    // ... and add to package collection
                    var package = new PackageIdentity(packageDependency.Id, v);
                    packageIdentities.Add(package);

                    var pdi = await Helpers.GetPackageDependencyInfo(package, nuGetFramework, repositories, cache, logger: this);
                    packages.Add(pdi);

                    // ... and look at all dependencies within this main dependency + recurse
                    await Helpers.ListAllPackageDependencies(
                        package,
                        repositories,
                        nuGetFramework,
                        packages,
                        recurse: true,
                        cache,
                        logger: this,
                        cancellationToken);
                }
            }  else if (ParameterSetName.StartsWith("InfoObject") ) {

                // Find all potential dependencies
                foreach (var dependencyInfo in SourcePackageDependencyInfo)
                {
                    WriteVerbose($"Processing DependencyInfo for main dependency: {dependencyInfo.Id} {dependencyInfo.Version}");

                    // Add to package collection
                    packages.Add(dependencyInfo);
                    packageIdentities.Add(new PackageIdentity(dependencyInfo.Id, dependencyInfo.Version));

                    // ... and look at all dependencies within this main dependency + recurse
                    await Helpers.ListAllPackageDependencies(
                        dependencyInfo,
                        repositories,
                        nuGetFramework,
                        packages,
                        recurse: true,
                        cache,
                        logger: this,
                        cancellationToken);
                }

            }
        }

        protected override Task EndProcessingAsync()
        {
            WriteVerbose("Start Resolving");

            var toplevelIdString = "ResolveNugetPackages_Main";
            var toplevelId = new PackageIdentity(toplevelIdString, NuGetVersion.Parse("0.0.0"));

            WriteVerbose("Building top-level dependency for provided main package ids");
            var depList = new List<PackageDependency>();
            foreach (var pi in packageIdentities)
            {
                var d = new PackageDependency(pi.Id, NuGet.Versioning.VersionRange.Parse($"[{pi.Version}]"));
                depList.Add(d);
                WriteVerbose(d.ToString());
            }
            var toplevelDep = new SourcePackageDependencyInfo(toplevelId.Id, toplevelId.Version, depList, false, null);
            packageIdentities.Add(toplevelId);
            packages.Add(toplevelDep);

            WriteVerbose("Resolution is based on this package list");
            foreach (var p in packages) { WriteVerbose(p.ToString()); }

            try
            {
                var resolvedPackages = Helpers.ResolvePackages(packageIdentities, packages, repositories, logger: this)
                            .Select(p => packages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));

                if (null != resolvedPackages)
                {
                    foreach (var p in resolvedPackages)
                    {
                        if (p.Id != toplevelIdString) WriteObject(p);
                    }
                }
                else
                {
                    var er = new ErrorRecord(new ArgumentException(), $"Could not resolve dependency tree for input packages. Result empty.", ErrorCategory.ObjectNotFound, PackageDependency);
                    WriteError(er);
                }
            } catch (Exception e) {
                var er = new ErrorRecord(e, $"Could not resolve dependency tree for input packages. Exception thrown.", ErrorCategory.ObjectNotFound, PackageDependency);
                WriteError(er);
            }

            return Task.CompletedTask;
        }
    }
}
