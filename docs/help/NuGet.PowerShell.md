---
Module Name: NuGet.Powershell
Module Guid: {{ Update Module Guid }}
Download Help Link: {{ Update Download Link }}
Help Version: {{ Update Help Version }}
Locale: {{ Update Locale }}
---

# NuGet.Powershell Module
## Description
This Powershell module provides a platform independent NuGet client as cmdlets based on the NuGet Client SDK. The project was started because the only platform independent solution was to either use the dotnet toolchain, which is focused on C# development, or nuget.exe, which is requires full-fledeged Mono installation to run (and has very limited functionality). The NuGet package format itself has applications far beyond the C# ecosystem and this module provides the functionality to leverage the NuGet ecosystem everywhere, where powershell core exists.

## NuGet.Powershell Cmdlets
### [Find-NuGetPackage](Find-NuGetPackage.md)
Search for NuGet packages within the configured repositories.

### [Get-NuGetPackageDependencyInfo](Get-NuGetPackageDependencyInfo.md)
Get the dependencies for package identified by an Id and a NuGet VersionRange. It can provide
only direct dependencies or the full list of transitive dependency objects.

### [Get-NuGetPackageIdentityFromFolder](Get-NuGetPackageIdentityFromFolder.md)
Retrive NuGet version for local folder that contains NuGet package content.

### [Get-NuGetPackageMetadata](Get-NuGetPackageMetadata.md)
Fetch NuGet package metadata for a package without actually downloading the package itself.

### [Get-NuGetPackageNuspec](Get-NuGetPackageNuspec.md)
Get the `nuspec` content for provided package identities as `XmlDocument`.

### [Get-NuGetPackageVersions](Get-NuGetPackageVersions.md)
Get a list of all available versions for the given packages ids.

### [Install-NuGetPackage](Install-NuGetPackage.md)
Download and unpack a nuget package to a local directory.

### [New-NuGetPackage](New-NuGetPackage.md)
Create a new NuGet package from use a nuspec file, command line paramters or both.

### [New-NuGetPackageDependency](New-NuGetPackageDependency.md)
Creates a NuGet `PackageDependency` object from the provided paramters.

### [New-NuGetPackageIdentity](New-NuGetPackageIdentity.md)
Creates a NuGet `PackageIdentity` object from the provided paramters.

### [Publish-NuGetPackage](Publish-NuGetPackage.md)
Publish a local NuGet package file to a NuGet feed.

### [Resolve-NuGetPackageDependencies](Resolve-NuGetPackageDependencies.md)
Resolve the transitive dependency tree for all given input packages.

The result is a list of packages that fullfil all dependency constraints across all provided
packages if the resolution attempt is successful.

### [Resolve-NuGetPackageVersion](Resolve-NuGetPackageVersion.md)
Resolve a NuGet version range or floating version specification to a concrete version number.

### [Save-NuGetPackage](Save-NuGetPackage.md)
Download and save a NuGet Package.

