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
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsData.Save, "NuGetPackage",
        DefaultParameterSetName = "Object")]
    public class SaveNugetPackageCmdlet : AsyncCmdlet
    {
        [Parameter( ParameterSetName = "Object", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter( ParameterSetName = "Object-ConfigFile", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Parameter( ParameterSetName = "Object-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PackageIdentity[] PackageIdentity { get; set; }

        [Parameter( ParameterSetName = "Args", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter( ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Parameter( ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter( ParameterSetName = "Args", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        [Parameter( ParameterSetName = "Args-ConfigFile", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        [Parameter( ParameterSetName = "Args-ConfigArgs", Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        public string Version { get; set; }

        [Parameter]
        public string OutputPath { get; set; } = ".";

        [Parameter]
        public SwitchParameter Force { get; set; } = false;

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

        private ISettings settings;
        private List<SourceRepository> repositories = new List<SourceRepository>();
        private SourceCacheContext cache = new SourceCacheContext();

        private string cwd;

        protected override Task BeginProcessingAsync()
        {
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
            var packagePathResolver = new MappingPackagePathResolver(OutputPath);
            WriteVerbose("StartProcessing");
            if (ParameterSetName.StartsWith("Args") ) {

                var pi =  new PackageIdentity(Id, NuGetVersion.Parse(Version));
                PackageIdentity = new[] { pi };
            }

            if ((null != PackageIdentity) && (PackageIdentity.Length > 0))
            {
                foreach (var packageToSave in PackageIdentity)
                {
                    bool found = false;

                    WriteVerbose($"Processing Package : {packageToSave}");
                    foreach (var repo in repositories) {
                        WriteVerbose($"Searching Repo: {repo}");
                        FindPackageByIdResource resource = await repo.GetResourceAsync<FindPackageByIdResource>();
                        string packageOutputPath = Path.Combine(OutputPath, packagePathResolver.GetPackageFileName(packageToSave));

                        var result = await resource.GetDependencyInfoAsync(
                            packageToSave.Id,
                            packageToSave.Version,
                            cache,
                            logger: this,
                            CancellationToken.None);

                        if (null != result)
                        {
                            WriteVerbose($"Saving to : {packageOutputPath}");
                            found = true;
                            if (File.Exists(packageOutputPath) && !Force)
                            {
                                WriteVerbose($"File exists, not downloading");
                            }
                            else
                            {
                                FileStream packageStream = File.Create(packageOutputPath);
                                try
                                {
                                    await resource.CopyNupkgToStreamAsync(
                                        packageToSave.Id,
                                        packageToSave.Version,
                                        packageStream,
                                        cache,
                                        logger: this,
                                        CancellationToken.None);
                                }
                                finally
                                {
                                    packageStream.Close();
                                }
                            }
                            break;
                        }
                    }
                    if (! found){
                        WriteWarning($"Package not found : {packageToSave}");
                    }
                }
            }
        }
    }
}
