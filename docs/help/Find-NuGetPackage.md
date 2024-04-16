---
external help file: NuGet.Powershell.dll-Help.xml
Module Name: NuGet.Powershell
online version:
schema: 2.0.0
---

# Find-NuGetPackage

## SYNOPSIS
Search for NuGet packages within the configured repositories.

## SYNTAX

### Object
```
Find-NuGetPackage [-SearchString] <String[]> [-IncludePrerelease] [-MaxResult <Int32>] [<CommonParameters>]
```

### Object-ConfigFile
```
Find-NuGetPackage [-SearchString] <String[]> [-IncludePrerelease] [-MaxResult <Int32>] -ConfigFile <String>
 [<CommonParameters>]
```

### Object-ConfigArgs
```
Find-NuGetPackage [-SearchString] <String[]> [-IncludePrerelease] [-MaxResult <Int32>] -Source <String>
 [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>] [<CommonParameters>]
```

### ConfigFile
```
Find-NuGetPackage [-ConfigFile <String>] [<CommonParameters>]
```

### ConfigArgs
```
Find-NuGetPackage -Source <String> [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>]
 [<CommonParameters>]
```

## DESCRIPTION
Searches all repositories from NuGet packages that match the search string(s). Search term syntax can be found
here `https://learn.microsoft.com/en-us/nuget/consume-packages/finding-and-choosing-packages#search-syntax`:

* You can search the package id, packageid, version, title, tags, author, description, summary, or owner properties by using the syntax `<property>:<term>`.
* Search applies to keywords and descriptions, and is case-insensitive. For example, the following strings all search the id property for the string `nuget.core`:

  * `id:NuGet.Core`
  * `ID:nuget.core`
  * `Id:NUGET.CORE`

* Searches on the id property match substrings, while packageid and owner use exact, case-insensitive matches. For example:

  * `PackageId:jquery` searches for the exact package ID jquery.
  * `Id:jquery` searches for all package IDs that contain the string jquery.

* You can search for multiple values or properties at the same time. For example:

  * `id:jquery id:ui` searches for multiple terms in the id property.
  * `id:jquery tags:validation` searches for multiple properties.

  Search ignores unsupported properties, so invalid:jquery ui is the same as searching for ui, and invalid:jquery returns all packages.

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-NuGetPackage "json"
```

Find all packages that contain the term `json` within the package Id

### Example 2
```powershell
PS C:\> Find-NuGetPackage "PackageId:Newtonsoft.Json"
```

Find package where the package Id matches exactly `Newtonsoft.Json`

## PARAMETERS

### -ConfigFile
{{ Fill ConfigFile Description }}

```yaml
Type: String
Parameter Sets: Object-ConfigFile
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

### -IncludePrerelease
{{ Fill IncludePrerelease Description }}

```yaml
Type: SwitchParameter
Parameter Sets: Object, Object-ConfigFile, Object-ConfigArgs
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MaxResult
{{ Fill MaxResult Description }}

```yaml
Type: Int32
Parameter Sets: Object, Object-ConfigFile, Object-ConfigArgs
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SearchString
{{ Fill SearchString Description }}

```yaml
Type: String[]
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
Parameter Sets: Object-ConfigArgs, ConfigArgs
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
Parameter Sets: Object-ConfigArgs, ConfigArgs
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
Parameter Sets: Object-ConfigArgs, ConfigArgs
Aliases:
Accepted values: 2, 3

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

## OUTPUTS

### NuGet.Protocol.PackageSearchMetadata

## NOTES

## RELATED LINKS
