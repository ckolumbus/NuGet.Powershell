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

    $nugetId = "Resolve.NuGetPackageVersion.Test"
    $nugetVersion = "1.0.0"

    # setup pacakge structure
    New-NugetPackage -Id $nugetId -Version "1.0.0" -Authors "Authors" -Description "Description" -OutputPath $packageDir -Content "$TestData/content"
    New-NugetPackage -Id $nugetId -Version "1.1.0" -Authors "Authors" -Description "Description" -OutputPath $packageDir -Content "$TestData/content"
    New-NugetPackage -Id $nugetId -Version "2.1.0" -Authors "Authors" -Description "Description" -OutputPath $packageDir -Content "$TestData/content"
    New-NugetPackage -Id $nugetId -Version "3.0.0" -Authors "Authors" -Description "Description" -OutputPath $packageDir -Content "$TestData/content"
    New-NugetPackage -Id $nugetId -Version "3.1.0-alpha.1" -Authors "Authors" -Description "Description" -OutputPath $packageDir -Content "$TestData/content"
    New-NugetPackage -Id $nugetId -Version "3.2.0-alpha.2+build" -Authors "Authors" -Description "Description" -OutputPath $packageDir -Content "$TestData/content"
}

Describe "Resolve-NuGetPackageVersion PS$PSVersion" {

    It 'resolves latest major release version with "*" as search version' {
        $result = Resolve-NuGetPackageVersion $nugetId "*" -Source $packageDir

        $result.Version | Should -Be "3.0.0"
    }

    It 'resolves exact search version "<_>" ' -ForEach @("1.1.0", "2.1.0", "3.2.0-alpha.2") {
        $result = Resolve-NuGetPackageVersion $nugetId $_ -Source $packageDir
        $result.Version | Should -Be $_
    }

    It 'ignores build meta data in semantic version string without pre-prelase tag "2.1.0+anothertag"' {
        $result = Resolve-NuGetPackageVersion $nugetId "2.1.0+anothertag" -Source $packageDir
        $result.Version | Should -Be "2.1.0"
    }

    It 'ignores build meta data in semantic version string with pre-prelase tag "3.2.0-alpha.2+anothertag"' {
        $result = Resolve-NuGetPackageVersion $nugetId "3.2.0-alpha.2+anothertag" -Source $packageDir
        $result.Version | Should -Be "3.2.0-alpha.2"
    }

    It 'resolves latest  pre-release version with "*-*" as search version' {
        $result = Resolve-NuGetPackageVersion $nugetId "*-*" -Source $packageDir
        $result.Version | Should -Be "3.2.0-alpha.2"
    }
}
