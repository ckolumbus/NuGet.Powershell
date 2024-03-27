---
external help file: NuGet.PowerShell.dll-Help.xml
Module Name: NuGet.PowerShell
online version:
schema: 2.0.0
---

# Get-NuGetPackageNuspec

## SYNOPSIS
Get the `nuspec`s for provided package identities as `XmlDocument`.

Can read from local directories (see `Install-NugetPackage`), local `.nupkg`
files and remote packages. For the latter the packages needs to be downloaded.

## SYNTAX

### Directory
```
Get-NuGetPackageNuspec -Directory <String[]> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Path
```
Get-NuGetPackageNuspec [-Path] <String[]> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Object
```
Get-NuGetPackageNuspec [-PackageIdentity] <PackageIdentity[]> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### Object-ConfigFile
```
Get-NuGetPackageNuspec [-PackageIdentity] <PackageIdentity[]> -ConfigFile <String>
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Object-ConfigArgs
```
Get-NuGetPackageNuspec [-PackageIdentity] <PackageIdentity[]> -Source <String> [-SourceProtocolVersion <Int32>]
 [-SourceCredential <PSCredential>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Args
```
Get-NuGetPackageNuspec [-Id] <String> [-Version] <String> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### Args-ConfigFile
```
Get-NuGetPackageNuspec [-Id] <String> [-Version] <String> -ConfigFile <String>
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Args-ConfigArgs
```
Get-NuGetPackageNuspec [-Id] <String> [-Version] <String> -Source <String> [-SourceProtocolVersion <Int32>]
 [-SourceCredential <PSCredential>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### ConfigFile
```
Get-NuGetPackageNuspec [-ConfigFile <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### ConfigArgs
```
Get-NuGetPackageNuspec -Source <String> [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
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
Parameter Sets: Object-ConfigFile, Args-ConfigFile
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

```yaml
Type: String
Parameter Sets: ConfigFile
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Directory
{{ Fill Directory Description }}

```yaml
Type: String[]
Parameter Sets: Directory
Aliases:

Required: True
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

### -Path
{{ Fill Path Description }}

```yaml
Type: String[]
Parameter Sets: Path
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
Parameter Sets: Object-ConfigArgs, Args-ConfigArgs, ConfigArgs
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
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]
### NuGet.Packaging.Core.PackageIdentity[]
### System.String
## OUTPUTS

### System.Xml.XmlDocument
## NOTES

## RELATED LINKS
