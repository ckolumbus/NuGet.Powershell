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

using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using System.Management.Automation;
using NuGet.Protocol;
using NuGet.Versioning;
using System.Net;
using System;

namespace NuGet.PowerShell
{
    public static class Helpers
    {

        public static string GetRootedPath(string path, string root)
        {
            // keep absolute uris
            if (Uri.TryCreate(path, UriKind.Absolute, out _))
                return path;

            const string prefixToRemove = "file://";
            if (path.ToLower().StartsWith(prefixToRemove)) {
                path = path.Substring(prefixToRemove.Length);
            }

            string result = root;
            if (path != "") {
                if (Path.IsPathRooted(path))
                {
                    result = path;
                } else {
                    result = Path.Combine(root, path);
                }
            }
            result = Path.GetFullPath(result);
            return result;
        }

        public static async Task<NuGetVersion> GetBestMatchingVersion (
            PackageDependency packageDependency,
            NuGetFramework framework,
            IEnumerable<SourceRepository> repositories,
            SourceCacheContext cache,
            ILogger logger = null)
        {
            return await GetBestMatchingVersion(packageDependency.Id, packageDependency.VersionRange,framework, repositories, cache, logger);
        }

        public static async Task<NuGetVersion> GetBestMatchingVersion (
            string id, VersionRange versionRange,
            NuGetFramework framework,
            IEnumerable<SourceRepository> repositories,
            SourceCacheContext cache,
            ILogger logger = null)
        {
            var versions = await FindPackageVersionsAsync(id, framework, repositories, cache, logger);
            var bestVersion = versionRange.FindBestMatch(versions);

            return bestVersion;
        }

        public static async Task<IEnumerable<NuGetVersion>>
        FindPackageVersionsAsync(string id, NuGetFramework framework, IEnumerable<SourceRepository> repositories, SourceCacheContext cache, ILogger logger = null)
        {
            if (logger is null) logger = NullLogger.Instance;

            CancellationToken cancellationToken = CancellationToken.None;

            HashSet<NuGetVersion> allVersions = new HashSet<NuGetVersion>();

            foreach (var r in repositories) {
                FindPackageByIdResource resource = await r.GetResourceAsync<FindPackageByIdResource>();

                IEnumerable<NuGetVersion> versions = await resource.GetAllVersionsAsync(
                    id,
                    cache,
                    logger,
                    cancellationToken);

                if (versions != null)
                {
                    allVersions.AddRange(versions);
                }
            }
            return allVersions;
        }

        public static ISettings GetSettings(string ConfigFile="", PSCmdlet ctx = null, ILogger logger = null)
        {
            ISettings settings;

            // src: https://stackoverflow.com/a/30893170
            string cwd = cwd = Directory.GetCurrentDirectory();
            cwd = ctx?.SessionState.Path.CurrentFileSystemLocation.Path;

            logger?.LogVerbose($"init with current working dir: {cwd}");

            if (ConfigFile != "")
            {
                string fullConfigFile = ConfigFile;
                if (! Path.IsPathRooted(fullConfigFile) ) {
                    fullConfigFile = Path.Combine(cwd, ConfigFile);
                }
                var fullConfigFilePath = Path.GetFullPath(fullConfigFile);
                var dir = Path.GetDirectoryName(fullConfigFilePath);
                var file = Path.GetFileName(fullConfigFilePath);

                logger?.LogVerbose($"Loading Config file {file} from {dir}");

                settings = Settings.LoadSpecificSettings(dir, file);
            }
            else
            {
                logger?.LogVerbose($"Loading default nuget config settings");
                settings = Settings.LoadDefaultSettings(root: cwd);
            }

            return settings;
        }

        public static SourceRepository GetRepository(string source, NetworkCredential credential, int protocolVersion = 3, ILogger logger = null)
        {
            SourceRepository repository = null;
            if (source.ToLower().StartsWith("http://") || source.ToLower().StartsWith("https://"))
            {

                var packageSource = new PackageSource(source) { ProtocolVersion = protocolVersion };
                if (!(credential is null))
                {
                    packageSource.Credentials = new PackageSourceCredential(
                            source: source,
                            username: credential.UserName,
                            passwordText: credential.Password,
                            isPasswordClearText: true,
                            validAuthenticationTypesText: null);
                };

                // If the `SourceRepository` is created with a `PackageSource`, the rest of APIs will consume the credentials attached to `PackageSource.Credentials`.
                repository = Repository.Factory.GetCoreV3(packageSource);
            } else {
                // assume file system repo
                repository = Repository.Factory.GetCoreV3(source);
            }

            return repository;
        }

        public static IEnumerable<SourceRepository> GetRepositories(ISettings settings, ILogger logger = null)
        {
            IEnumerable<SourceRepository> repositories = new PackageSourceProvider(settings)
                .LoadPackageSources()
                .Where(source => source.IsEnabled)
                .Select(source => Repository.Factory.GetCoreV3(source.Source));

            foreach (var r in repositories) {
                logger?.LogVerbose($"using repo: {r}");
            }

            return repositories;
        }

        public static IEnumerable<PackageIdentity> ResolvePackages(
            IEnumerable<PackageIdentity> packageIdentities,
            ISet<SourcePackageDependencyInfo> packages,
            IEnumerable<SourceRepository> repositories,
            ILogger logger = null)
        {
            if (logger is null) logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            var packageIds = packageIdentities.Select(pi => pi.Id).ToList();
            var packageIdsString = string.Join(",", packageIds);
            // Find the best version for each package
            // TODO: find out what these parameters really mean? resolution process is difficult to understand,
            //       especially in case there are more than one top-level dependencies
            var resolverContext = new PackageResolverContext(
                dependencyBehavior: DependencyBehavior.Lowest,
                targetIds: packageIds,
                requiredPackageIds: packageIds,
                packagesConfig: Enumerable.Empty<PackageReference>(),
                preferredVersions: Enumerable.Empty<PackageIdentity>(),
                availablePackages: packages,
                repositories.Select(r => r.PackageSource),
                logger);

            var resolver = new PackageResolver();
            var resolvedPackages = resolver.Resolve(resolverContext, cancellationToken);

            return resolvedPackages;
        }

        public static async Task<SourcePackageDependencyInfo> GetPackageDependencyInfo(
            PackageIdentity package,
            NuGetFramework framework,
            IEnumerable<SourceRepository> repositories,
            ILogger logger = null)
        {
            var cache = new SourceCacheContext();
            var cancellationToken = CancellationToken.None;

            return await GetPackageDependencyInfo(package, framework, repositories, cache, logger);
        }
        public static async Task<SourcePackageDependencyInfo> GetPackageDependencyInfo(
            string id,
            string version,
            NuGetFramework framework,
            IEnumerable<SourceRepository> repositories,
            ILogger logger = null)
        {
            var cache = new SourceCacheContext();

            return await GetPackageDependencyInfo(id, version, framework, repositories, cache, logger);
        }
        public static async Task<SourcePackageDependencyInfo> GetPackageDependencyInfo(
            string id,
            string version,
            NuGetFramework framework,
            IEnumerable<SourceRepository> repositories,
            SourceCacheContext cache,
            ILogger logger = null
        )
        {
            var versionRange = VersionRange.Parse(version);
            var package = new PackageIdentity(id, versionRange.MinVersion);
            return await GetPackageDependencyInfo(package, framework, repositories, cache, logger);
        }
        public static async Task<SourcePackageDependencyInfo> GetPackageDependencyInfo(
            PackageIdentity package,
            NuGetFramework framework,
            IEnumerable<SourceRepository> repositories,
            SourceCacheContext cache,
            ILogger logger = null)
        {
            if (logger is null) logger = NullLogger.Instance;

            SourcePackageDependencyInfo dependencyInfo = null;
            CancellationToken cancellationToken = CancellationToken.None;

            foreach (var repository in repositories)
            {
                int retry = 3;
                while (retry > 0)
                {
                    try
                    {
                        var dependencyInfoResource = await repository.GetResourceAsync<DependencyInfoResource>();
                        dependencyInfo = await dependencyInfoResource.ResolvePackage(package, framework, cache, logger, cancellationToken);
                        break;
                    }
                    catch
                    {
                        retry--;
                    }
                }
                if (retry == 0)
                {
                    // dependency not found in repository, search next
                    dependencyInfo = null;
                }

                if (null != dependencyInfo) break;
            }

            if (null != dependencyInfo)
            {
                //throw new Exception($"Error getting dependency info for '{package}' from '{repository}! Ignoring !");
            }

            return dependencyInfo;
        }

        public static async Task ListAllPackageDependencies(
            SourcePackageDependencyInfo dependencyInfo,
            IEnumerable<SourceRepository> repositories,
            NuGetFramework framework,
            HashSet<SourcePackageDependencyInfo> dependencies,
            bool recurse,
            SourceCacheContext cache,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Getting dependency info for {dependencyInfo.Id} {dependencyInfo.Version}");
            await ListAllPackageDependencies_(
                dependencyInfo,
                repositories,
                framework,
                dependencies,
                recurse,
                cache,
                logger,
                cancellationToken);
        }

        public static async Task ListAllPackageDependencies(
            PackageIdentity package,
            IEnumerable<SourceRepository> repositories,
            NuGetFramework framework,
            HashSet<SourcePackageDependencyInfo> dependencies,
            bool recurse,
            SourceCacheContext cache,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Getting dependency info for {package.Id} {package.Version}");
            var dependencyInfo = await GetPackageDependencyInfo(package, framework, repositories, cache, logger);

            if (dependencyInfo == null)
            {
                logger.LogVerbose($"    dependency not found, skipping");
                return;
            }

            await ListAllPackageDependencies_(
                dependencyInfo,
                repositories,
                framework,
                dependencies,
                recurse,
                cache,
                logger,
                cancellationToken);
        }

        private static async Task ListAllPackageDependencies_(
            SourcePackageDependencyInfo dependencyInfo,
            IEnumerable<SourceRepository> repositories,
            NuGetFramework framework,
            HashSet<SourcePackageDependencyInfo> dependencies,
            bool recurse,
            SourceCacheContext cache,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Starting to getting direct dependency info for {dependencyInfo.Id} {dependencyInfo.Version}");

            foreach (var dependency in dependencyInfo.Dependencies)
            {
                var pi = new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion);
                logger.LogVerbose($"Processing {pi.Id} {pi.Version}");

                var di = await GetPackageDependencyInfo(pi, framework, repositories, cache, logger);

                if (dependencies.Add(di))
                {
                    logger.LogVerbose($"Adding {pi.Id} {pi.Version}");
                    if (recurse)
                    {
                        logger.LogVerbose($"Recursing into dependencies of {pi.Id} {pi.Version}");
                        await ListAllPackageDependencies_(
                            di,
                            repositories,
                            framework,
                            dependencies,
                            recurse,
                            cache,
                            logger,
                            cancellationToken);
                    }
                } else {
                    logger.LogVerbose($"Skipping {pi.Id} {pi.Version}, already processed");
                }
            }
        }
    }
}