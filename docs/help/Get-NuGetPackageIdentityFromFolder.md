---
external help file: NuGet.Powershell.dll-Help.xml
Module Name: NuGet.Powershell
online version:
schema: 2.0.0
---

# Get-NuGetPackageIdentityFromFolder

## SYNOPSIS
Retrive NuGet version for local folder that contains NuGet package content.

## SYNTAX

```
Get-NuGetPackageIdentityFromFolder -Path <String[]> [<CommonParameters>]
```

## DESCRIPTION
Reads NuGet package information from a local folder and returns a \`PackageIdentity\` object if actual NuGet package content has been found.

## EXAMPLES

### Example 1
```
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -Path
{{ Fill Path Description }}

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]
list of directories to read the data from

## OUTPUTS

### NuGet.Packaging.Core.PackageIdentity
the package identitie retrieved from the input directory

## NOTES

## RELATED LINKS
