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
using NuGet.Packaging.Core;

namespace NuGet.PowerShell
{
    [Cmdlet(VerbsCommon.New, "NuGetPackageDependency")]
    [OutputType(typeof(PackageDependency))]
    public class NewNugetPackageDependencyCmdlet : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        public string VersionRange { get; set; }

        [Parameter(
            Position = 2,
            ValueFromPipelineByPropertyName = true)]
        public string[] Include { get; set; }

        [Parameter(
            Position = 3,
            ValueFromPipelineByPropertyName = true)]
        public string[] Exclude { get; set; }


        protected override void ProcessRecord()
        {
            WriteObject(new PackageDependency(Id, NuGet.Versioning.VersionRange.Parse(VersionRange), Include, Exclude));
        }
    }
}
