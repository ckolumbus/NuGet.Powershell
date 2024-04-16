---
external help file: NuGet.Powershell.dll-Help.xml
Module Name: NuGet.Powershell
online version:
schema: 2.0.0
---

# Save-NuGetPackage

## SYNOPSIS
Download and save a NuGet Package.

## SYNTAX

### Object (Default)
```
Save-NuGetPackage [-PackageIdentity] <PackageIdentity[]> [-OutputPath <String>] [-Force] [<CommonParameters>]
```

### Object-ConfigFile
```
Save-NuGetPackage [-PackageIdentity] <PackageIdentity[]> [-OutputPath <String>] [-Force] -ConfigFile <String>
 [<CommonParameters>]
```

### Object-ConfigArgs
```
Save-NuGetPackage [-PackageIdentity] <PackageIdentity[]> [-OutputPath <String>] [-Force] -Source <String>
 [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>] [<CommonParameters>]
```

### Args
```
Save-NuGetPackage [-Id] <String> [-Version] <String> [-OutputPath <String>] [-Force] [<CommonParameters>]
```

### Args-ConfigFile
```
Save-NuGetPackage [-Id] <String> [-Version] <String> [-OutputPath <String>] [-Force] -ConfigFile <String>
 [<CommonParameters>]
```

### Args-ConfigArgs
```
Save-NuGetPackage [-Id] <String> [-Version] <String> [-OutputPath <String>] [-Force] -Source <String>
 [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>] [<CommonParameters>]
```

## DESCRIPTION
Searches the configured repositories for the Package with the given verion and saves
the complete package to the local disk without unpacking it.

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

### -Force
{{ Fill Force Description }}

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

### -OutputPath
{{ Fill OutputPath Description }}

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

### -PackageIdentity
{{ Fill PackageIdentity Description }}

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
Parameter Sets: Object-ConfigArgs, Args-ConfigArgs
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
Parameter Sets: Object-ConfigArgs, Args-ConfigArgs
Aliases:
Accepted values: 2, 3

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Version
{{ Fill Version Description }}

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
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### NuGet.Packaging.Core.PackageIdentity[]

### System.String

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
