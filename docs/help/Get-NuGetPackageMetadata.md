---
external help file: NuGet.Powershell.dll-Help.xml
Module Name: NuGet.Powershell
online version:
schema: 2.0.0
---

# Get-NuGetPackageMetadata

## SYNOPSIS
Fetch NuGet package metadata for a package without actually downloading the package itself.

## SYNTAX

### Object
```
Get-NuGetPackageMetadata [-PackageIdentity] <PackageIdentity[]> [<CommonParameters>]
```

### Object-ConfigFile
```
Get-NuGetPackageMetadata [-PackageIdentity] <PackageIdentity[]> -ConfigFile <String> [<CommonParameters>]
```

### Object-ConfigArgs
```
Get-NuGetPackageMetadata [-PackageIdentity] <PackageIdentity[]> -Source <String>
 [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>] [<CommonParameters>]
```

### Args
```
Get-NuGetPackageMetadata [-Id] <String> [-Version] <String> [<CommonParameters>]
```

### Args-ConfigFile
```
Get-NuGetPackageMetadata [-Id] <String> [-Version] <String> -ConfigFile <String> [<CommonParameters>]
```

### Args-ConfigArgs
```
Get-NuGetPackageMetadata [-Id] <String> [-Version] <String> -Source <String> [-SourceProtocolVersion <Int32>]
 [-SourceCredential <PSCredential>] [<CommonParameters>]
```

### ConfigFile
```
Get-NuGetPackageMetadata [-ConfigFile <String>] [<CommonParameters>]
```

### ConfigArgs
```
Get-NuGetPackageMetadata -Source <String> [-SourceProtocolVersion <Int32>] [-SourceCredential <PSCredential>]
 [<CommonParameters>]
```

## DESCRIPTION
This cmdlet uses the [NuGet V3 Package Metadata API](https://learn.microsoft.com/en-us/nuget/api/registration-base-url-resource)
to retrieve metadat for a package without actually downloading the package itself.

This can safe time and bandwidth in cases where the actual package content is not needed.

Caveat: the returned data structure dependends on the server implementation of the Metadata API!

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-NuGetPackageMetadata Serilog 3.1.1

CatalogUri               : https://api.nuget.org/v3/catalog0/data/2023.11.10.12.46.06/serilog.3.1.1.json
Authors                  : Serilog Contributors
DependencySetsInternal   : {[.NETFramework,Version=v4.6.2] (System.Diagnostics.DiagnosticSource [7.0.2, ), System.ValueTuple [4.5.0, )), [.NETFramework,Version=v4.7.1] (System.Diagnostics.DiagnosticSource [7.0.2, )), [net5.0] (),
                           [net6.0] ()...}
DependencySets           : {[.NETFramework,Version=v4.6.2] (System.Diagnostics.DiagnosticSource [7.0.2, ), System.ValueTuple [4.5.0, )), [.NETFramework,Version=v4.7.1] (System.Diagnostics.DiagnosticSource [7.0.2, )), [net5.0] (),
                           [net6.0] ()...}
Description              : Simple .NET logging with fully-structured events
DownloadCount            :
IconUrl                  : https://api.nuget.org/v3-flatcontainer/serilog/3.1.1/icon
Identity                 : Serilog.3.1.1
LicenseUrl               : https://www.nuget.org/packages/Serilog/3.1.1/license
Owners                   :
PackageId                : Serilog
ProjectUrl               : https://serilog.net/
Published                : 10.11.2023 13:43:00 +01:00
ReadmeUrl                : https://www.nuget.org/packages/Serilog/3.1.1#show-readme-container
ReportAbuseUrl           : https://www.nuget.org/packages/Serilog/3.1.1/ReportAbuse
PackageDetailsUrl        : https://www.nuget.org/packages/Serilog/3.1.1?_src=template
RequireLicenseAcceptance : False
Summary                  : Simple .NET logging with fully-structured events
Tags                     : serilog, logging, semantic, structured
Title                    : Serilog
Version                  : 3.1.1
ParsedVersions           :
PrefixReserved           : False
LicenseExpression        : Apache-2.0
LicenseExpressionVersion :
LicenseMetadata          : NuGet.Packaging.LicenseMetadata
IsListed                 : True
DeprecationMetadata      :
Vulnerabilities          :
```

Get metadata for `Serilog` package

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

### -Id
The Id of the package for which to get the metadata.

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
The version of the package for which to get the metadata, e.g. `13.2.3`
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

### NuGet.Packaging.Core.PackageIdentity[]
## OUTPUTS

### NuGet.Protocol.PackageSearchMetaData
## NOTES

## RELATED LINKS
