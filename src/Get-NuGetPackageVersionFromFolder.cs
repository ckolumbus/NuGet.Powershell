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

using System.Management.Automation;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "NuGetPackageIdentityFromFolder")]
    [OutputType(typeof(PackageIdentity))]
    public class GetNugetPackageIdentityFromFolderCmdlet : PSLoggerCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string[] Path { get; set; }

        private string cwd;

        protected override void BeginProcessing()
        {
            cwd = SessionState.Path.CurrentFileSystemLocation.Path;
        }

        protected override void ProcessRecord()
        {
            foreach (var packagePath in Path) {
                var fullPackagePath = packagePath;
                if (!System.IO.Path.IsPathRooted(packagePath))
                {
                    fullPackagePath = System.IO.Path.Combine(cwd, packagePath);
                }
                fullPackagePath = System.IO.Path.GetFullPath(fullPackagePath);

                WriteVerbose($"Reading {fullPackagePath}");

                var packageReader = new PackageFolderReader(fullPackagePath);
                var pi = packageReader.GetIdentity();
                WriteObject(pi);
            }
        }
    }
}
