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
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace NuGet.PowerShell
{

    public class MappingPackagePathResolver : PackagePathResolver
    {
        public Dictionary<string, string> PackageIdNameMap { get; }

        public MappingPackagePathResolver(string packageId, string mappedName, string rootDirectory, bool useSideBySidePaths = true) : base(rootDirectory, useSideBySidePaths)
        {
            PackageIdNameMap = new Dictionary<string, string>() {{ packageId, mappedName}};
        }

        public MappingPackagePathResolver(Dictionary<string, string> packageIdentityNameMap, string rootDirectory, bool useSideBySidePaths = true) : base(rootDirectory, useSideBySidePaths)
        {
            PackageIdNameMap = new Dictionary<string, string>(packageIdentityNameMap);
        }

        public MappingPackagePathResolver(string rootDirectory, bool useSideBySidePaths = true) : base(rootDirectory, useSideBySidePaths)
        {
            PackageIdNameMap = new Dictionary<string, string>();
        }

        public bool RemovePackageIdentityNameMapping(string packageId)
        {
            return PackageIdNameMap.Remove(packageId);
        }

        public bool RemovePackageIdentityNameMapping(PackageIdentity packageIdentity)
        {
            return PackageIdNameMap.Remove(packageIdentity.Id);
        }

        public void AddPackageIdNameMapping(string packageId, string name) => PackageIdNameMap.Add(packageId, name);
        public void AddPackageIdNameMapping(PackageIdentity packageIdentity, string name)
        {
            PackageIdNameMap.Add(packageIdentity.Id, name);
        }

        public override string GetPackageDirectoryName(PackageIdentity packageIdentity)
        {
            if (PackageIdNameMap.ContainsKey(packageIdentity.Id))
            {
                return PackageIdNameMap[packageIdentity.Id];

            } else {
                return base.GetPackageDirectoryName(packageIdentity);
            }
        }
    }

}