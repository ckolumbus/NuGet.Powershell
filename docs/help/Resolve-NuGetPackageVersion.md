﻿---
external help file: NuGet.Powershell.dll-Help.xml
Module Name: NuGet.Powershell
online version:
schema: 2.0.0
---

# Resolve-NuGetPackageVersion

## SYNOPSIS
Resolve a NuGet version range or floating version specification to a concrete version number.

## SYNTAX

### Object (Default)
```
Resolve-NuGetPackageVersion [-PackageDependency] <PackageDependency[]> [-Framework <String>]
 [<CommonParameters>]
```

### Object-ConfigFile
```
Resolve-NuGetPackageVersion [-PackageDependency] <PackageDependency[]> [-Framework <String>]
 -ConfigFile <String> [<CommonParameters>]
```

### Object-ConfigArgs
```
Resolve-NuGetPackageVersion [-PackageDependency] <PackageDependency[]> [-Framework <String>] -Source <String>
 [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>] [<CommonParameters>]
```

### Args
```
Resolve-NuGetPackageVersion [-Id] <String> -VersionRange <String> [-Framework <String>] [<CommonParameters>]
```

### Args-ConfigFile
```
Resolve-NuGetPackageVersion [-Id] <String> -VersionRange <String> [-Framework <String>] -ConfigFile <String>
 [<CommonParameters>]
```

### Args-ConfigArgs
```
Resolve-NuGetPackageVersion [-Id] <String> -VersionRange <String> [-Framework <String>] -Source <String>
 [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>] [<CommonParameters>]
```

### ConfigArgs
```
Resolve-NuGetPackageVersion [-Framework <String>] [-SourceProtocolVersion <Int32>]
 [-SourceCredential <PSCredential>] [<CommonParameters>]
```

## DESCRIPTION
Searches for the best matching version within the list of existing versions for a version range or
a floating verson specification.

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
Parameter Sets: Object-ConfigFile, Args-ConfigFile
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
Parameter Sets: Args
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

```yaml
Type: String
Parameter Sets: Args-ConfigFile, Args-ConfigArgs
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
Parameter Sets: Object, Object-ConfigFile, Object-ConfigArgs
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
Parameter Sets: Object-ConfigArgs, Args-ConfigArgs
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
Parameter Sets: Object-ConfigArgs, Args-ConfigArgs, ConfigArgs
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SourceProtocolVersion
{{ Fill SourceProtocolVersion Description }}

```yaml
Type: Int32
Parameter Sets: Object-ConfigArgs, Args-ConfigArgs, ConfigArgs
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
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### NuGet.Packaging.Core.PackageDependency[]
### System.String
## OUTPUTS

### NuGet.Protocol.Core.Types.SourcePackageDependencyInfo
## NOTES

## RELATED LINKS
