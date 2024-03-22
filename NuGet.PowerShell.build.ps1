<#
   Copyright (c) Chris Drexler

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

>#

<#
.Synopsis
    Build script NuGet.PowerShell binary module

.Notes
    Build script is based on `InvokeBuild` Powershell module

#>

# Build script parameters
param(
    [string] $Repsitory = $env:POWERSHELL_REPO_ID,
    [string] $ApiKey = $env:APIKEY,
    [ValidateSet("Release", "Debug")]
    [string] $Configuration = "Release",
    [string] $BuildOutputDir
)

$ModuleName = "NuGet.PowerShell"
if ([string]::IsNullOrEmpty($BuildOutputDir)) {
    $distDir = "$BuildRoot/dist"
} else {
    $distDir = $BuildOutputDir
}

$moduleDir = join-Path $distdir $ModuleName
$helpInputDir = join-Path $BuildRoot "docs/help"
$helpOutputDir = join-Path $moduleDir "en-US"
$devPrereleaseTag = "alpha"

$Timestamp = Get-date -uformat "%Y%m%d-%H%M%S"
$PSVersion = $PSVersionTable.PSVersion.Major
$TestFile = "TestResults_PS$PSVersion`_$TimeStamp.xml"

# Ensure IB works in the strict mode.
Set-StrictMode -Version Latest

task . module

# Synopsis: Remove temporary items.
task clean {
    remove $moduleDir
}

# Synopsis: Set $script:Version from Release-Notes.
task version {
    $matches =  switch -Regex -File CHANGELOG.md {'##\s+(\d+\.\d+\.\d+)(-(\S+))?' {$Matches; break}}
    $script:FullVersion = $script:Version = $matches[1]
    $script:PreRelease = $matches[3]
    if ($script:PreRelease) {
        if ($script:PreRelease -eq $devPrereleaseTag) {
            # Powershell only support Semver 1.0.0 pre-release tags!
            $script:PreRelease += "$(Get-Date -Format "yyyyMMddHHmm")"
        }
        $script:FullVersion += "-$($script:PreRelease)"
    }
    assert $script:Version
}

task restore {
}

# Synopsis: Make the module folder.
task build restore, version, {

    # create dist folder
    New-Item $moduleDir -ItemType Directory -Force | Out-Null

    exec { dotnet publish --configuration $Configuration --output $moduleDir ./src }

    $preReleaseSnippet = ""
    if ($script:PreRelease ) {
        $preReleaseSnippet = @"
    PrivateData = @{
        PSData = @{
            Prerelease = '$($script:PreRelease)'
        }
    }
"@
    }

    # make manifest
    Set-Content "$moduleDir\$ModuleName.psd1" @"
@{
    RootModule = 'NuGet.PowerShell.dll'
    ModuleVersion = '$script:Version'
    GUID = '90d98f7e-3d3e-4870-93fa-d50557d7b999'
    Author = 'Chris Drexler'
    Copyright = 'Copyright (c) Chris Drexler. All rights reserved.'
    Description = 'Netstandard2.0 based NuGet implementation for PowerShell 5.1 & PowerShell Core'
    PowerShellVersion = '5.1'
    FunctionsToExport = @()  # not relevant for binary modules
$preReleaseSnippet
}
"@
}

task updatehelp build, {
    Import-Module $moduleDir -Force

    $parameters = @{
        Path = $helpInputDir
        RefreshModulePage = $true
        AlphabeticParamsOrder = $true
        UpdateInputOutput = $true
        ExcludeDontShow = $true
        Encoding = [System.Text.Encoding]::UTF8
    }
    Update-MarkdownHelpModule @parameters

    Remove-Module $ModuleName -Force
}

task createhelp {
    New-ExternalHelp $helpInputDir -OutputPath $helpOutputDir
}

task module clean, build, createhelp, version

task zip module, {
    Compress-Archive -DestinationPath (Join-Path $distDir "$ModuleName.${script:FullVersion}.zip") -Path $moduleDir
}

task checkPublishPrerequisites {
    $changes = exec { git status --short }
    assert (!$changes) "Please, commit changes."
}

# Synopsis: Push with a version tag.
task pushRelease checkPublishPrerequisites, version, {

    exec { git push }
    exec { git tag -a "v$script:FullVersion" -m "v$script:FullVersion" }
    exec { git push origin "$script:FullVersion" }
}

task publish-only {
    Publish-Module -Path $distDir/$ModuleName -Repository $Repsitory -NuGetApiKey $ApiKey
}

# Synopsis: Push PSGallery package.
task publish checkPublishPrerequisites,  {
    if (-not $ApiKey) {
        throw "No ApiKey defined!"
    }
    if (-not $Repsitory) {
        throw "No Repository defined!"
    }
}, module, test, publish-only, pushRelease

task test {
    Write-Warning "Tests not yet implemented"
}
