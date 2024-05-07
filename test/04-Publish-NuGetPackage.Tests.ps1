BeforeDiscovery {
    $ProjectName = "NuGet.Powershell"
    $PSVersion = $PSVersionTable.PSVersion.Major
}

BeforeAll {
    $ProjectName = "NuGet.Powershell"
    $ProjectPath = Resolve-Path "$PSScriptRoot\.."

    $distDir = Join-Path $ProjectPath "dist"
    $moduleDir = Resolve-Path (Join-Path $distDir $ProjectName)
    Remove-Module $ProjectName -ErrorAction SilentlyContinue
    Import-Module $moduleDir -Force

    $outputDir = $TestDrive
    $packageDir = Join-Path $TestDrive "packages"
    New-Item -ItemType Directory $packageDir -Force -ErrorAction SilentlyContinue

    $TestData = Resolve-Path $PSScriptRoot/data

    $nugetId = "Publish.NuGetPackage.Test"
    $nugetVersion = "1.0.0"

    New-NugetPackage -Id $nugetId -Version $nugetVersion -Authors "Authors" -Description "Description" -OutputPath $outputDir -Content "$TestData/content"
    $nugetPkgName = "${nugetId}.${nugetVersion}.nupkg"
    $nugetPkgPath = Join-Path $outputDir $nugetPkgName
}

Describe "Publish-NugetPackage PS$PSVersion" {

    It 'can publish to local folder' {
        Publish-NugetPackage $nugetPkgPath $packageDir

        "$packageDir/$nugetPkgName" | Should -Exist
    }
}
