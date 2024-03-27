---
external help file: NuGet.PowerShell.dll-Help.xml
Module Name: NuGet.PowerShell
online version:
schema: 2.0.0
---

# Get-NuGetPackageDependencyInfo

## SYNOPSIS
Get the dependencies for package identified by an Id and a NuGet VersionRange. It can provide
only direct dependencies or the full list of transitive dependency objects.

## SYNTAX

### Args (Default)
```
Get-NuGetPackageDependencyInfo [-Id] <String> [-VersionRange <String>] [-Recurse] [-Framework <String>]
 [-RemoveTopLevelDependencies] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Object
```
Get-NuGetPackageDependencyInfo [-PackageIdentity] <PackageIdentity[]> [-Recurse] [-Framework <String>]
 [-RemoveTopLevelDependencies] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Object-ConfigFile
```
Get-NuGetPackageDependencyInfo [-PackageIdentity] <PackageIdentity[]> [-Recurse] [-Framework <String>]
 [-RemoveTopLevelDependencies] -ConfigFile <String> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Object-ConfigArgs
```
Get-NuGetPackageDependencyInfo [-PackageIdentity] <PackageIdentity[]> [-Recurse] [-Framework <String>]
 [-RemoveTopLevelDependencies] -Source <String> [-SourceProtocolVersion <Int32>]
 [-SourceCredential <PSCredential>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### DepObject
```
Get-NuGetPackageDependencyInfo [-PackageDependency] <PackageDependency[]> [-Recurse] [-Framework <String>]
 [-RemoveTopLevelDependencies] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### DepObject-ConfigFile
```
Get-NuGetPackageDependencyInfo [-PackageDependency] <PackageDependency[]> [-Recurse] [-Framework <String>]
 [-RemoveTopLevelDependencies] -ConfigFile <String> [-SourceCredential <PSCredential>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### DepObject-ConfigArgs
```
Get-NuGetPackageDependencyInfo [-PackageDependency] <PackageDependency[]> [-Recurse] [-Framework <String>]
 [-RemoveTopLevelDependencies] -Source <String> [-SourceProtocolVersion <Int32>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Args-ConfigFile
```
Get-NuGetPackageDependencyInfo [-Id] <String> [-VersionRange <String>] [-Recurse] [-Framework <String>]
 [-RemoveTopLevelDependencies] -ConfigFile <String> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Args-ConfigArgs
```
Get-NuGetPackageDependencyInfo [-Id] <String> [-VersionRange <String>] [-Recurse] [-Framework <String>]
 [-RemoveTopLevelDependencies] -Source <String> [-SourceProtocolVersion <Int32>]
 [-SourceCredential <PSCredential>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
The cmdlet resolves the given input parameters into a package identities and extract either only
the direct dependencies or recursivly all transitive dependencies of this package.

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-NugetPackageDependencyInfo Serilog 3.*

Listed       : True
Source       : https://api.nuget.org/v3/index.json
DownloadUri  : https://api.nuget.org/v3-flatcontainer/serilog/3.1.1/serilog.3.1.1.nupkg
PackageHash  :
Dependencies : {System.Diagnostics.DiagnosticSource [7.0.2, )}
Id           : Serilog
Version      : 3.1.1
HasVersion   : True
```

Resolve `3.*` to the lates version that matches this version range and retrieves the direct dependency information without traversing the dependency tree.

## PARAMETERS

### -ConfigFile
Path to the NuGet config file to use, if neither  `-ConfigFile` nor `-Source` is provide,
the standard configs are used.

```yaml
Type: String
Parameter Sets: Object-ConfigFile, DepObject-ConfigFile, Args-ConfigFile
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Framework
The DotNet Framework identfier for which the dependency analysis is performed. Defaults to `any`.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: any
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
The Id of the package to analyze.

```yaml
Type: String
Parameter Sets: Args, Args-ConfigFile, Args-ConfigArgs
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PackageDependency
A list of NuGet dependency objects describing the packages to analayze
(see `New-NugetPackageDependency` for a way to create such an object)

```yaml
Type: PackageDependency[]
Parameter Sets: DepObject, DepObject-ConfigFile, DepObject-ConfigArgs
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -PackageIdentity
A list of NuGet identity objects describing the packages to analayze
(see `New-NugetPackageIdentity` for a way to create such an object)

```yaml
Type: PackageIdentity[]
Parameter Sets: Object, Object-ConfigFile, Object-ConfigArgs
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Recurse
Recursively analyze the dependency tree of the given packages.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -RemoveTopLevelDependencies
With this option, the packages provided as input are removed from the output list,
so that only the dependencies are returned. This can be used for explicit processing
of dependencies without the root package.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Source
The path or url to a NuGet package feed to be used.

```yaml
Type: String
Parameter Sets: Object-ConfigArgs, DepObject-ConfigArgs, Args-ConfigArgs
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SourceCredential
The credentials for the `-Source` feed, if needed.

```yaml
Type: PSCredential
Parameter Sets: Object-ConfigArgs, DepObject-ConfigFile, Args-ConfigArgs
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SourceProtocolVersion
The protocol version of the `-Source` feed, defaults to `3`.

```yaml
Type: Int32
Parameter Sets: Object-ConfigArgs, DepObject-ConfigArgs, Args-ConfigArgs
Aliases:
Accepted values: 2, 3

Required: False
Position: Named
Default value: 3
Accept pipeline input: False
Accept wildcard characters: False
```

### -VersionRange
A NuGet version range string consisting either of a concrete version `12.0.1`,
a version range `[11,)` or a floating version `13.*`.
Defaults to `*`.

```yaml
Type: String
Parameter Sets: Args, Args-ConfigFile, Args-ConfigArgs
Aliases:

Required: False
Position: Named
Default value: "*"
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### NuGet.Packaging.Core.PackageIdentity[]
### NuGet.Packaging.Core.PackageDependency[]
## OUTPUTS

### NuGet.Protocol.Core.Types.SourcePackageDependencyInfo
## NOTES

## RELATED LINKS
