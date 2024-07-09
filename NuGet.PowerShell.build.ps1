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

.EXAMPLE

    ```pwsh
    ./NuGet.PowerShell.build.ps1 ?
    ````

    List all possible build targets

.EXAMPLE

    ```pwsh
    ./NuGet.PowerShell.build.ps1 test
    ````

    Setup workspace with needed external modules and call `test` target

.Notes
    Build script is based on `InvokeBuild` Powershell module

#>

# Build script parameters
param(
    [Parameter(Position = 0)] $Tasks,
    [string] $Repsitory = $env:POWERSHELL_REPO_ID,
    [string] $ApiKey = $env:APIKEY,
    [ValidateSet("Release", "Debug")]
    [string] $Configuration = "Release",
    [string] $BuildOutputDir,
    [switch] $EnableAsyncLoggerExperimental,
    [string[]] $TestPath
)

##########################################################################################
# Hook to enable direct calling of the build script
# 1. sets up environment with needed modules (see ./build/Setup-Workspace.ps1)
# 2. calls Invoke-Build with provided paramaters to execute actual build
##########################################################################################
if ([System.IO.Path]::GetFileName($MyInvocation.ScriptName) -ne 'Invoke-Build.ps1') {
    # setup and load external modules
    . $PSScriptRoot/build/Setup-Workspace.ps1
    try {
        Invoke-Build -Task $Tasks -File $MyInvocation.MyCommand.Path @PSBoundParameters
        exit 0
    }
    catch {
        exit -1
    }
}
##########################################################################################


$ModuleName = "NuGet.Powershell"
if ([string]::IsNullOrEmpty($BuildOutputDir)) {
    $distDir = "$BuildRoot/dist"
} else {
    $distDir = $BuildOutputDir
}

$moduleDir = join-Path $distdir $ModuleName
$helpInputDir = join-Path $BuildRoot "docs/help"
$helpOutputDir = join-Path $moduleDir "en-US"
$devPrereleaseTag = "alpha"

$srcDir = Resolve-Path "./src"
$Timestamp = Get-date -uformat "%Y%m%d-%H%M%S"
$PSVersion = $PSVersionTable.PSVersion.Major
$TestFilePrefix = "TestResults_"
$TestFileExt = "xml"
$TestFile = "${TestFilePrefix}_PS${PSVersion}_$TimeStamp.${TestFileExt}"

# Ensure IB works in the strict mode.
Set-StrictMode -Version Latest

task . module

# Synopsis: Remove temporary items.
task clean {
    remove $moduleDir
    remove "${TestFilePrefix}*"
    exec { dotnet clean --configuration $Configuration $srcDir }
}

# Synopsis: Set $script:Version from Release-Notes.
task version {
    $versionMatches =  switch -Regex -File CHANGELOG.md {'##\s+(\d+\.\d+\.\d+)(-(\S+))?' {$Matches; break}}
    $script:FullVersion = $script:Version = $versionMatches[1]
    $script:PreRelease = $versionMatches[3]
    if ($script:PreRelease) {
        if ($script:PreRelease -eq $devPrereleaseTag) {
            # Powershell only support Semver 1.0.0 pre-release tags!
            $script:PreRelease += "$(Get-Date -Format "yyyyMMddHHmm")"
        }
        $script:FullVersion += "-$($script:PreRelease)"
    }
    assert $script:Version
}

# Synopsis: Make the module folder.
task build version, {

    # create dist folder
    New-Item $moduleDir -ItemType Directory -Force | Out-Null

    $publishArgs = @()
    if ($EnableAsyncLoggerExperimental) {
        $publishArgs += '/p:DefineConstants="ENABLE_PSLOGGERCMDLET"'
    }
    exec { dotnet publish @publishArgs --configuration $Configuration --output $moduleDir $srcDir }

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
    RootModule = 'NuGet.Powershell.dll'
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

# Synopsis: updates the PlatyPS markdown help files based on the cmdlet signatures
task updatehelpmarkdown {
    if ($PSVersion -ne '5') {
        throw "UpdateHelpMarkdown must be run from Powershell 5 to avoid wrong handling of common `-ProgressAction` parsmeter."
    }
}, build, {
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
    # Run twice to update index file in case of newly added cmdlets
    Update-MarkdownHelpModule @parameters

    Remove-Module $ModuleName -Force
}

# Synopsis: converts the markdown help files into xml file suitable for cmdline help
task createhelpxml {
    New-ExternalHelp $helpInputDir -OutputPath $helpOutputDir
}

# Synopsis: builds the module including the extern xml help file
task module clean, build, createhelpxml, version

# Synopsis: generates the zip file which includes all modules for ct-SetupWorkspace (SetupEnv, PSDepend & NuGet.Powershell)
task zip module, {
    Compress-Archive -DestinationPath (Join-Path $distDir "$ModuleName.${script:FullVersion}.zip") -Path $moduleDir
}

# Synopsis: Verify Repository status before publishing
task checkPublishPrerequisites {
    $changes = exec { git status --short }
    assert (!$changes) "Please, commit changes."
}

# Synopsis: Push Release including version tag.
task pushRelease checkPublishPrerequisites, version, {
    $gitVersionTag = "v$script:FullVersion"
    exec { git push }
    exec { git tag $gitVersionTag }
    exec { git push origin $gitVersionTag}
}

# Synopsis: execute publish powershell module to a repository without build or pre-publish checsk
task publish-only {
    Publish-Module -Path $distDir/$ModuleName -Repository $Repsitory -NuGetApiKey $ApiKey
}

# Synopsis: Create a module release : build, test, publish module, push repo with version tag
task publish checkPublishPrerequisites,  {
    if (-not $ApiKey) {
        throw "No ApiKey defined!"
    }
    if (-not $Repsitory) {
        throw "No Repository defined!"
    }
}, module, test, publish-only, pushRelease


function getPesterConfig {
    # Gather test results. Store them in a variable and file
    $pesterConfig = New-PesterConfiguration
    $pesterConfig.Run.PassThru = $true
    $pesterConfig.Output.Verbosity = "Detailed"
    $pesterConfig.TestResult.Enabled = $true
    $pesterConfig.TestResult.OutputFormat = "NUnitXml"
    $pesterConfig.TestResult.OutputPath = "$BuildRoot\$TestFile"

    return $pesterConfig
}

# Synopsis: trigger test execution without prior build within `powershell` shell
task test-ps5-only {
    powershell -c ". ./build/Setup-Workspace.ps1; Invoke-Build test-only"
}

# Synopsis: trigger test execution without prior build within `pwsh` shell
task test-ps7-only {
    pwsh -c ". ./build/Setup-Workspace.ps1; Invoke-Build test-only"
}

# Synopsis: Execute tests without prior build within current shell
task test-only {
    $pesterConfig = getPesterConfig
    $pesterConfig.Filter.ExcludeTag = @("disabled" )

    if ($null -eq $TestPath) {
        $pesterConfig.Run.Path = "$BuildRoot\test"
    } else {
        $pesterConfig.Run.Path = $TestPath
    }

    $TestResults = Invoke-Pester -Configuration $pesterConfig

    # Failed tests?
    if($TestResults.FailedCount -gt 0)
    {
        Write-Error "Failed '$($TestResults.FailedCount)' tests, build failed"
    }
}

# Synopsis: Execute `powershell` and `pwsh` tests after a clean module build
task test module, test-ps5-only, test-ps7-only