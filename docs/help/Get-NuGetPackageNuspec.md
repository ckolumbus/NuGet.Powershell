---
external help file: NuGet.Powershell.dll-Help.xml
Module Name: NuGet.Powershell
online version:
schema: 2.0.0
---

# Get-NuGetPackageNuspec

## SYNOPSIS
Get the `nuspec` content for provided package identities as `XmlDocument`.

## SYNTAX

### Directory
```
Get-NuGetPackageNuspec -Directory <String[]> [<CommonParameters>]
```

### Path
```
Get-NuGetPackageNuspec [-Path] <String[]> [<CommonParameters>]
```

### Object
```
Get-NuGetPackageNuspec [-PackageIdentity] <PackageIdentity[]> [<CommonParameters>]
```

### Object-ConfigFile
```
Get-NuGetPackageNuspec [-PackageIdentity] <PackageIdentity[]> -ConfigFile <String> [<CommonParameters>]
```

### Object-ConfigArgs
```
Get-NuGetPackageNuspec [-PackageIdentity] <PackageIdentity[]> -Source <String> [-SourceProtocolVersion <Int32>]
 [-SourceCredential <PSCredential>] [<CommonParameters>]
```

### Args
```
Get-NuGetPackageNuspec [-Id] <String> [-Version] <String> [<CommonParameters>]
```

### Args-ConfigFile
```
Get-NuGetPackageNuspec [-Id] <String> [-Version] <String> -ConfigFile <String> [<CommonParameters>]
```

### Args-ConfigArgs
```
Get-NuGetPackageNuspec [-Id] <String> [-Version] <String> -Source <String> [-SourceProtocolVersion <Int32>]
 [-SourceCredential <PSCredential>] [<CommonParameters>]
```

### ConfigFile
```
Get-NuGetPackageNuspec [-ConfigFile <String>] [<CommonParameters>]
```

### ConfigArgs
```
Get-NuGetPackageNuspec -Source <String> [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>]
 [<CommonParameters>]
```

## DESCRIPTION
Returns a `XmlDocument` with the `.nuspec` file content of the given package.

It can retrieve the content from local directories (see `Install-NugetPackage`), local `.nupkg`
files and remote packages. For remote packages, the package needs to be downloaded.

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -ConfigFile
Path to the NuGet config file to use, if neither  `-ConfigFile` nor `-Source` is provide,
the standard configs are used.

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
Try to read data from a local directory where a NuGet package has been extracted,
e.g. with `Install-NugetPackage` or `nuget.exe install`.

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
The Id of the package for which to get the nuspec file content.

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

### -Path
Try to read data from a local nuget package file `.nupkg`.

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
The path or url to a NuGet package feed to be used.

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
The credentials for the `-Source` feed, if needed.

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
The protocol version of the `-Source` feed, defaults to `3`.

```yaml
Type: Int32
Parameter Sets: Object-ConfigArgs, Args-ConfigArgs, ConfigArgs
Aliases:
Accepted values: 2, 3

Required: False
Position: Named
Default value: 3
Accept pipeline input: False
Accept wildcard characters: False
```

### -Version
The version of the package for which to get the nuspec content, e.g. `13.2.3`
(no floating versions or version ranges allowed).

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

### System.String[]
### NuGet.Packaging.Core.PackageIdentity[]
### System.String
## OUTPUTS

### System.Xml.XmlDocument
## NOTES

## RELATED LINKS
