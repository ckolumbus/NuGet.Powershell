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

* `New-NugetPackage`
* `Publish-NugetPackage`

### Handling NuGet Package Information Structures

* `New-NugetPackageDependency`
* `New-NugetPackageIdentity`

### Providing access to Individual Package Version Information

* `Get-NugetPackageMetadata`
* `Get-NugetPackageIdentityFromFolder`
* `Get-NugetPackageVersions`
* `Resolve-NugetPackageVersion`

### Handling Dependency Information & Dependency Tree Resolution

* `Get-NugetPackageDependencyInfo`
* `Resolve-NugetPackageDependencies`

### Installation of NuGet Packages

* `Install-NugetPackage`

## Development

### Prerequisites

For building the module, the following components are needed

* `dotnet` (>6.0)
* `InvokeBuild` Powershell module
* `PlatyPS` Powershell module

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

`NuGet.Powershell` is licensed under the terms of the MIT license. You can find
the complete text in LICENSE.

Please refer to our [Contributors](https://github.com/ckolumbus/NuGet.PowerShell/graphs/contributors) page
for a complete list of our contributors.
