---
external help file: NuGet.PowerShell.dll-Help.xml
Module Name: NuGet.PowerShell
online version:
schema: 2.0.0
---

# Resolve-NuGetPackageDependencies

## SYNOPSIS
Resolve the transitive dependency tree for all given input packages.

The result is a list of packages that fullfil all dependency constraints across all provided
packages if the resolution attempt is successful.

## SYNTAX

### InfoObject (Default)
```
Resolve-NuGetPackageDependencies [-SourcePackageDependencyInfo] <SourcePackageDependencyInfo[]>
 [-Framework <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### DepObject
```
Resolve-NuGetPackageDependencies [-PackageDependency] <PackageDependency[]> [-Framework <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### DepObject-ConfigFile
```
Resolve-NuGetPackageDependencies [-PackageDependency] <PackageDependency[]> [-Framework <String>]
 -ConfigFile <String> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### DepObject-ConfigArgs
```
Resolve-NuGetPackageDependencies [-PackageDependency] <PackageDependency[]> [-Framework <String>]
 -Source <String> [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### InfoObject-ConfigFile
```
Resolve-NuGetPackageDependencies [-SourcePackageDependencyInfo] <SourcePackageDependencyInfo[]>
 [-Framework <String>] -ConfigFile <String> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### InfoObject-ConfigArgs
```
Resolve-NuGetPackageDependencies [-SourcePackageDependencyInfo] <SourcePackageDependencyInfo[]>
 [-Framework <String>] -Source <String> [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Args
```
Resolve-NuGetPackageDependencies [-Id] <String> [-VersionRange] <String> [-Framework <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Args-ConfigFile
```
Resolve-NuGetPackageDependencies [-Id] <String> [-VersionRange] <String> [-Framework <String>]
 -ConfigFile <String> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Args-ConfigArgs
```
Resolve-NuGetPackageDependencies [-Id] <String> [-VersionRange] <String> [-Framework <String>] -Source <String>
 [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### ConfigFile
```
Resolve-NuGetPackageDependencies [-Framework <String>] -ConfigFile <String>
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### ConfigArgs
```
Resolve-NuGetPackageDependencies [-Framework <String>] -Source <String> [-SourceProtocolVersion <Int32>]
 [-SourceCredential <PSCredential>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
{{ Fill in the Description }}

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -ConfigFile
{{ Fill ConfigFile Description }}

```yaml
Type: String
Parameter Sets: DepObject-ConfigFile, InfoObject-ConfigFile, Args-ConfigFile, ConfigFile
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Framework
{{ Fill Framework Description }}

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
{{ Fill Id Description }}

```yaml
Type: String
Parameter Sets: Args, Args-ConfigFile, Args-ConfigArgs
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -PackageDependency
{{ Fill PackageDependency Description }}

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

### -Source
{{ Fill Source Description }}

```yaml
Type: String
Parameter Sets: DepObject-ConfigArgs, InfoObject-ConfigArgs, Args-ConfigArgs, ConfigArgs
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SourceCredential
{{ Fill SourceCredential Description }}

```yaml
Type: PSCredential
Parameter Sets: DepObject-ConfigArgs, InfoObject-ConfigArgs, Args-ConfigArgs, ConfigArgs
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SourcePackageDependencyInfo
{{ Fill SourcePackageDependencyInfo Description }}

```yaml
Type: SourcePackageDependencyInfo[]
Parameter Sets: InfoObject, InfoObject-ConfigFile, InfoObject-ConfigArgs
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -SourceProtocolVersion
{{ Fill SourceProtocolVersion Description }}

```yaml
Type: Int32
Parameter Sets: DepObject-ConfigArgs, InfoObject-ConfigArgs, Args-ConfigArgs, ConfigArgs
Aliases:
Accepted values: 2, 3

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -VersionRange
{{ Fill VersionRange Description }}

```yaml
Type: String
Parameter Sets: Args, Args-ConfigFile, Args-ConfigArgs
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### NuGet.Packaging.Core.PackageDependency[]
### NuGet.Protocol.Core.Types.SourcePackageDependencyInfo[]
### System.String
## OUTPUTS

### NuGet.Protocol.Core.Types.SourcePackageDependencyInfo
## NOTES

The (quirky) algorithm is as follows:

- if PackageDependency objects are provided as input: resolve the VersionRange first, creating
  a PackageDependencyInfo object containing a concrete package version with its dependencies
- recursively identify all dependencies for the PackageDependencyInfo packages
- create an internal virtual top-level dependency that references all provided packages as
  direct dependencies
- pack the virutal top-level, the provided and the recursively collected dependencies into
  one list
- call `Resolve` from the Nuget Client SDK on this list
- if the resolution is successful: output the dependency list (with the virtual top-level
  package removed again)

Maybe there is a better way to use the NuGet Cient SDK functions to achieve the same result

## RELATED LINKS
