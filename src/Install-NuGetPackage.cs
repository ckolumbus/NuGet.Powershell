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
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsLifecycle.Install, "NuGetPackage",
        DefaultParameterSetName = "Path")]
    [OutputType(typeof(string))]
    public class InstallNugetPackageCmdlet : AsyncCmdlet
    {
        [Parameter(ParameterSetName = "Object", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Object-ConfigFile", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "Object-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public SourcePackageDependencyInfo[] SourcePackageDependencyInfo { get; set; }

        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string[] Path { get; set; }

        [Parameter(ParameterSetName = "Args", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter(ParameterSetName = "Args", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        public string Version { get; set; }

        [Parameter(ParameterSetName = "Args", Position = 2, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Position = 2, ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Position = 2, ValueFromPipelineByPropertyName = true)]
        public string Framework { get; set; } = "any";

        [Parameter]
        public string[] Name { get; set; }

        [Parameter]
        public string OutputPath { get; set; } = ".";

        [Parameter]
        public SwitchParameter Force { get; set; } = false;

        [Parameter]
        public SwitchParameter UseSideBySidePaths { get; set; } = false;

        [Parameter(ParameterSetName = "Object-ConfigFile", Mandatory = true)]
        [Parameter(ParameterSetName = "Args-ConfigFile", Mandatory = true)]
        public string ConfigFile { get; set; } = "";

        [Parameter(ParameterSetName = "Object-ConfigArgs", Mandatory = true)]
        [Parameter(ParameterSetName = "Args-ConfigArgs", Mandatory = true)]
        public string Source { get; set; } = "";

        [Parameter(ParameterSetName = "Object-ConfigArgs")]
        [Parameter(ParameterSetName = "Args-ConfigArgs")]
        [ValidateSet(new string[] { "2", "3" })]
        public int SourceProtocolVersion { get; set; } = 2;

        [Parameter(ParameterSetName = "Object-ConfigArgs")]
        [Parameter(ParameterSetName = "Args-ConfigArgs")]
        public PSCredential SourceCredential { get; set; } = null;

        private NuGetFramework nuGetFramework;
        private ISettings settings;
        private List<SourceRepository> repositories = new List<SourceRepository>();
        private SourceCacheContext cache = new SourceCacheContext {DirectDownload=true, NoCache=true};

        private string cwd;

        protected override Task BeginProcessingAsync()
        {
            if (Name != null && UseSideBySidePaths) {
                throw new ArgumentException("'Name' paramter cannot be used together with 'UseSideBySidePaths'");
            }

            nuGetFramework = NuGetFramework.ParseFolder(Framework);

            // needed for "GlobalPackagesFolder"
            settings = Helpers.GetSettings(ConfigFile, ctx: this, logger: this);

            cwd = SessionState.Path.CurrentFileSystemLocation.Path;
            if ( Source != "") {
                var rootedSource = Helpers.GetRootedPath(Source, root: cwd);
                var repository = Helpers.GetRepository(rootedSource, SourceCredential?.GetNetworkCredential(), SourceProtocolVersion, logger:this);
                repositories.Add(repository);
            } else {
                WriteVerbose("Read settings from nuget config files");
                var newRepositories = Helpers.GetRepositories(settings, logger: this);
                repositories.AddRange(newRepositories);
            }

            OutputPath = Helpers.GetRootedPath(OutputPath, root: cwd);

            WriteVerbose($"ParamterSetName: {ParameterSetName}");
            return Task.CompletedTask;
        }

        protected override async Task ProcessRecordAsync()
        {
            WriteVerbose("StartProcessing");
            if (ParameterSetName.StartsWith("Args") ) {

                var pi =  new PackageIdentity(Id, NuGetVersion.Parse(Version));
                var packageToInstall = await Helpers.GetPackageDependencyInfo(pi, nuGetFramework, repositories, logger: this);
                SourcePackageDependencyInfo = new[] { packageToInstall };
            }

            if (MyInvocation.ExpectingInput) { // we are called with pipeline input
                if (Name != null) {
                   throw new InvalidOperationException("'Name' paramter cannot be used with pipeline input");
                }
            }

            var packagePathResolver = new MappingPackagePathResolver(OutputPath, useSideBySidePaths: UseSideBySidePaths);
            var packageExtractionContext = new PackageExtractionContext(
            PackageSaveMode.Nuspec | PackageSaveMode.Files,
            XmlDocFileSaveMode.None,
            clientPolicyContext: null,
            logger: this);

            if ((null != Path) && (Path.Length > 0))
            {
                foreach (var pathIndex in Enumerable.Range(0, Path.Length))
                {
                    WriteVerbose($"Processing Path index : {pathIndex}");
                    var pathArg = Path[pathIndex];
                    var path = Helpers.GetRootedPath(pathArg, root: cwd);
                    WriteVerbose($"Installing from Path : {path}");

                    using (var packageReaderPkg = new PackageArchiveReader(path))
                    {
                        var packageIdentity = packageReaderPkg.GetIdentity();
                        if (Name != null)
                        {
                            var id = packageIdentity.Id;
                            if (pathIndex < Name.Count()){
                                var mappedName = Name[pathIndex];
                                WriteVerbose($"adding output path name mapping : {id} -> {mappedName}");
                                packagePathResolver.AddPackageIdNameMapping(id, mappedName);
                            } else {
                                WriteVerbose($"no path name mapping provided for : {id}");
                            }
                        }

                        var installedPath = packagePathResolver.GetInstallPath(packageIdentity);

                        if (Directory.Exists(installedPath) && Force) {
                            Directory.Delete(installedPath, true);
                        }

                        if (!Directory.Exists(installedPath))
                        {

                            await PackageExtractor.ExtractPackageAsync(
                                path,
                                packageReaderPkg,
                                packagePathResolver,
                                packageExtractionContext,
                                CancellationToken.None);
                        } else {
                            using (var packageReader = new PackageFolderReader(installedPath))
                            {
                                var pi = packageReader.GetIdentity();
                                if ((pi.Id != packageIdentity.Id) || (pi.Version != packageIdentity.Version) ) {
                                    WriteWarning("Skipping install because target directory exists but id/version differs! Use '-Force' to force overwrite");
                                }
                            }
                        }
                        WriteObject(installedPath);
                    }
                }
            }
            else if ((null != SourcePackageDependencyInfo) && (SourcePackageDependencyInfo.Length > 0))
            {
                foreach (var packageIndex in Enumerable.Range(0,SourcePackageDependencyInfo.Count()))
                {
                    var packageToInstall = SourcePackageDependencyInfo[packageIndex];

                    WriteVerbose($"Installing Package : {packageToInstall}");
                    var downloadResource = await packageToInstall.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);
                    using (var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                            packageToInstall,
                            // issue #3 use DirectDownload to avoid caching - PackageDownloadContext needs explicit directDownloadDirectory
                            new PackageDownloadContext(cache, OutputPath, true),
                            SettingsUtility.GetGlobalPackagesFolder(settings),
                            logger: this, CancellationToken.None)
                    ) {
                        if (Name != null)
                        {
                            var id = packageToInstall.Id;
                            if (packageIndex < Name.Count()){
                                var mappedName = Name[packageIndex];
                                WriteVerbose($"adding output path name mapping : {id} -> {mappedName}");
                                packagePathResolver.AddPackageIdNameMapping(id, mappedName);
                            } else {
                                WriteVerbose($"no path name mapping provided for : {id}");
                            }
                        }

                        var installedPath = packagePathResolver.GetInstallPath(packageToInstall);

                        if (!Directory.Exists(installedPath) || Force)
                        {
                            await PackageExtractor.ExtractPackageAsync(
                                downloadResult.PackageSource,
                                downloadResult.PackageStream,
                                packagePathResolver,
                                packageExtractionContext,
                                CancellationToken.None);
                        }
                        WriteObject(installedPath);
                    };
                }
            }
        }
    }
}
