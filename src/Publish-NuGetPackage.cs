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
using System.Management.Automation;
using System.Threading.Tasks;

using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsData.Publish, "NuGetPackage",
        DefaultParameterSetName = "ApiKey")]
    [OutputType(typeof(string))]
    public class PublishNugetPackageCmdlet: AsyncCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string[] Path { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1)]
        public string Source { get; set; }

        [Parameter(
            ParameterSetName = "ApiKey",
            Position = 2)]
        public string ApiKey { get; set; } = null;

        [Parameter(
            ParameterSetName = "Cred",
            Position = 2)]
        public PSCredential SourceCredential { get; set; } = PSCredential.Empty;


        private string cwd;

        protected override Task BeginProcessingAsync()
        {
            cwd = SessionState.Path.CurrentFileSystemLocation.Path;

            if (ParameterSetName == "Cred" && !(SourceCredential == PSCredential.Empty )) {
                string usr = SourceCredential.GetNetworkCredential().UserName;
                string pwd = SourceCredential.GetNetworkCredential().Password;
                ApiKey = $"{usr}:{pwd}";
            }

            return Task.CompletedTask;
        }

        protected override async Task ProcessRecordAsync()
        {
            foreach (var packagePath in Path){
                try {
                    var fullPackagePath = packagePath;
                    if (! System.IO.Path.IsPathRooted(packagePath) ) {
                        fullPackagePath = System.IO.Path.Combine(cwd, packagePath);
                    }
                    fullPackagePath = System.IO.Path.GetFullPath(fullPackagePath);

                    // indidviduall publishing for better error reporting
                    // Push api below could hanlde array of package. Maybe that's faster, but
                    // how to do individual error handling?

                    WriteVerbose($"Publishing : {fullPackagePath} to {Source}");
                    await PublishNugetPackageAsync(fullPackagePath, Source, ApiKey);
                }
                catch (Exception e)
                {
                    var er = new ErrorRecord(e, "Package Upload Failed", ErrorCategory.ProtocolError, null);
                    WriteError(er);
                }
            }
        }

        protected override Task EndProcessingAsync()
        {
            WriteVerbose("End!");
            return Task.CompletedTask;
        }

        // TODO: move to helper, but need to implement logger enabled cmdlet base class first
        protected async Task PublishNugetPackageAsync(string packagePath, string packageSource, string apiKey)
        {
            SourceRepository repository = Repository.Factory.GetCoreV3(packageSource);
            PackageUpdateResource resource = await repository.GetResourceAsync<PackageUpdateResource>();
            WriteVerbose($"Push: start pushing [{packagePath}] to  [{packageSource}");
            await resource.Push(
                new[] { packagePath },
                symbolSource: null,
                timeoutInSecond: 60,
                disableBuffering: false,
                getApiKey: pSource => apiKey,
                getSymbolApiKey: pSource => null,
                noServiceEndpoint: false,
                skipDuplicate: false,   // error if trying to upload existing version
                symbolPackageUpdateResource: null,
                NullLogger.Instance);
            WriteVerbose("Push: End");
        }
    }
}