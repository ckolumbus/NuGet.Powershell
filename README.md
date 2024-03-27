# NuGet PowerShell Cmdlets

This Powershell module provides a platform independent NuGet client as cmdlets
based on the NuGet Client SDK.

The project was started because the only platform independent solution was to either
use the `dotnet` toolchain, which is focused on `C#` development, or `nuget.exe`, which
is requires full-fledeged Mono installation to run (and has very limited functionality).

The NuGet packages format itself has its application far beyond the `C#` ecosystem and
this module provides the functionality to leverage the NuGet ecosystem everywhere, where
powershell core exists.

## Documention

This section provides exampe usage scenarios and below is a short summary of available cmdlets.
See [Markdown Help](./docs/help) (or the module online help generated from it) for more
detailed information on the module and each cmdlet.

SCENARIOS: TBD

## Available Cmdlets

### Creating NuGet Packages

* `New-NugetPackage` : create package, based on a `.nuspec` file or purely based on command line arguments
* `Publish-NugetPackage` : publish a `.nupkg` file to a package feed

### Handling NuGet Package Information Structures

* `New-NugetPackageDependency`: create a `PackageDependency` object
* `New-NugetPackageIdentity`: create a `PackageIdentity` object

### Providing access to Individual Package Version Information

* `Get-NugetPackageMetadata`: retrieve metadata of a NuGet package from a feed. Works without downloading.
* `Get-NugetPackageNuspec`: retrieve the full `nuspec` file of a NuGet package from an installed package directory, a local `.nupkg` file or a packcage from a feed. For this, the package needs to be downloaded.
* `Get-NugetPackageVersions`: get all available versions for a package
* `Resolve-NugetPackageVersion` : resolve a NuGet version range to the best matching availabel NuGet package

### Handling Dependency Information & Dependency Tree Resolution

* `Get-NugetPackageDependencyInfo` : retrieve dependency information for a NuGet package
* `Resolve-NugetPackageDependencies` : resolve all transitive dependendencies of the given packages into a consistent package list that fulfills all version constraints

### Installation of NuGet Packages

* `Install-NugetPackage` : downloads and extracts a NuGet package to a local folder

## Development

### Prerequisites

For building the module, the following components are needed

* `dotnet` (>6.0)
* `InvokeBuild` Powershell module for building
* `PlatyPS` Powershell module for module xml help generation
* `Pester` Powershell module for testing

### Build

```pwsh
git clone https://github.com/ckolumbus/NuGet.Powershell
cd NuGetPowershell
Invoke-Build
```

## Contributing

This project has been started to fill a gap in the NuGet ecosystem. Anyone contribution
to make this more complete is welcome.

Please create an Issue to first discuss the topic. Based on the discussion result you can
sent a PR.

## License

`NuGet.Powershell` is licensed under the terms of the Apache 2.0 license. You can find
the complete text in LICENSE.

The implementation of `AsyncCmdlet` has been adapted from [PSKubectl](https://github.com/felixfbecker/PSKubectl)
which is released under the MIT license (see `LICENSE.AsyncCmdlet`).

Please refer to our [Contributors](https://github.com/ckolumbus/NuGet.PowerShell/graphs/contributors) page
for a complete list of our contributors.
