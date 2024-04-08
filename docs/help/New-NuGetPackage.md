---
external help file: NuGet.Powershell.dll-Help.xml
Module Name: NuGet.Powershell
online version:
schema: 2.0.0
---

# New-NuGetPackage

## SYNOPSIS
Create a new NuGet package from use a nuspec file, command line paramters or both.

## SYNTAX

```
New-NuGetPackage [[-Id] <String>] [[-Version] <String>] [[-Authors] <String[]>] [[-Description] <String>]
 [-ContentPath <String>] [[-Dependencies] <Hashtable>] [-FilesMapping <Hashtable>] [[-ManifestFile] <String>]
 [-Framework <String>] [-OutputPath <String>] [-OutputFilename <String>] [<CommonParameters>]
```

## DESCRIPTION
Create a NuGet package using by either

* using the content of a nuspec manifest file
* only the provided command line parameter for ad-hoc package generation
* or a combination where the command line paramter overwrite or extend the data loaded from manifest file

The command line parammters allow to set/extend the following nuspec elements

* metadata/id (overwrite)
* metadata/version (overwrite)
* metadata/description (overwrite)
* metdata/authors (add)
* dependencies (add)
* files (add)

A simplified way of setting the nupkg content is by providing the `ContentPath` paramter: everything
in this path will be recursively added to the nupkg root dir.

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -Authors
{{ Fill Authors Description }}

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 3
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ContentPath
{{ Fill ContentPath Description }}

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

### -Dependencies
Hashtable with information about NuGet package dependencies. Two formats are possible

Short version: the value for each id is a string and interpreted as version.

```powershell
@{ "Id1"= "1.0.3";  "Id2" = "3.0"}
```

Complete version: the value for each package id is a hashtable describing "Version", "includes"

```powershell
@{ "Id1"= @{Version=1.0.3; Includes="PrivateAssets,Build"; Excludes="Analyzers"} }
```

```yaml
Type: Hashtable
Parameter Sets: (All)
Aliases:

Required: False
Position: 5
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Description
{{ Fill Description Description }}

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 4
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FilesMapping
{{ Fill FilesMapping Description }}

```yaml
Type: Hashtable
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
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ManifestFile
{{ Fill ManifestFile Description }}

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputFilename
{{ Fill OutputFilename Description }}

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

### -Version
{{ Fill Version Description }}

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None
## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
