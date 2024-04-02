---
external help file: NuGet.Powershell.dll-Help.xml
Module Name: NuGet.Powershell
online version:
schema: 2.0.0
---

# Install-NuGetPackage

## SYNOPSIS
Download and unpack a nuget package to a local directory.

## SYNTAX

### Object (Default)
```
Install-NuGetPackage [-SourcePackageDependencyInfo] <SourcePackageDependencyInfo[]> [-OutputPath <String>]
 [-Force] [-UseSideBySidePaths] [<CommonParameters>]
```

### Object-ConfigFile
```
Install-NuGetPackage [-SourcePackageDependencyInfo] <SourcePackageDependencyInfo[]> [-OutputPath <String>]
 [-Force] [-UseSideBySidePaths] -ConfigFile <String> [<CommonParameters>]
```

### Object-ConfigArgs
```
Install-NuGetPackage [-SourcePackageDependencyInfo] <SourcePackageDependencyInfo[]> [-OutputPath <String>]
 [-Force] [-UseSideBySidePaths] -Source <String> [-SourceProtocolVersion <Int32>]
 [-SourceCredential <PSCredential>] [<CommonParameters>]
```

### Path
```
Install-NuGetPackage [-Path] <String[]> [-OutputPath <String>] [-Force] [-UseSideBySidePaths]
 [<CommonParameters>]
```

### Path-ConfigFile
```
Install-NuGetPackage [-Path] <String[]> [-OutputPath <String>] [-Force] [-UseSideBySidePaths]
 -ConfigFile <String> [<CommonParameters>]
```

### Args
```
Install-NuGetPackage [-Id] <String> [-Version] <String> [[-Framework] <String>] [-Name <String>]
 [-OutputPath <String>] [-Force] [-UseSideBySidePaths] [<CommonParameters>]
```

### Args-ConfigFile
```
Install-NuGetPackage [-Id] <String> [-Version] <String> [[-Framework] <String>] [-Name <String>]
 [-OutputPath <String>] [-Force] [-UseSideBySidePaths] -ConfigFile <String> [<CommonParameters>]
```

### Args-ConfigArgs
```
Install-NuGetPackage [-Id] <String> [-Version] <String> [[-Framework] <String>] [-Name <String>]
 [-OutputPath <String>] [-Force] [-UseSideBySidePaths] -Source <String> [-SourceProtocolVersion <Int32>]
 [-SourceCredential <PSCredential>] [<CommonParameters>]
```

## DESCRIPTION
Downloads a nuget pacakge from the configured feed(s) and unpacks the content the output path.

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
Parameter Sets: Object-ConfigFile, Path-ConfigFile, Args-ConfigFile
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

### -Framework
{{ Fill Framework Description }}

```yaml
Type: String
Parameter Sets: Args, Args-ConfigFile, Args-ConfigArgs
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName)
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

### -Name
{{ Fill Name Description }}

```yaml
Type: String
Parameter Sets: Args, Args-ConfigFile, Args-ConfigArgs
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
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

### -Path
{{ Fill Path Description }}

```yaml
Type: String[]
Parameter Sets: Path, Path-ConfigFile
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

### -SourcePackageDependencyInfo
{{ Fill SourcePackageDependencyInfo Description }}

```yaml
Type: SourcePackageDependencyInfo[]
Parameter Sets: Object, Object-ConfigFile, Object-ConfigArgs
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
Parameter Sets: Object-ConfigArgs, Args-ConfigArgs
Aliases:
Accepted values: 2, 3

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UseSideBySidePaths
{{ Fill UseSideBySidePaths Description }}

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

### NuGet.Protocol.Core.Types.SourcePackageDependencyInfo[]
### System.String[]
### System.String
## OUTPUTS

### NuGet.Packaging.PackageReaderBase
## NOTES

## RELATED LINKS
