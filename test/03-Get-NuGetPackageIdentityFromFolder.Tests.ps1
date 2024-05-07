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

    $packageDir = $TestDrive

    $TestData = Resolve-Path $PSScriptRoot/data
}

Describe "Get-NuGetPackageIdentityFromFolder PS$PSVersion with commandline parameter only" {
    BeforeAll {
        $id = 'Comp.Test1'
        $version = '1.0.0'
        $nupkg = "$id.$version.nupkg"
        $authors = @("a.b@c.d", "x.y")
        $sep = [System.Environment]::NewLine
        $desc = "line 1${sep}line 2"

        $nupkgPath = Join-Path $packageDir $nupkg

        New-NugetPackage $id $version $authors $desc "$TestData/content" -OutputPath $packageDir

        Install-NuGetPackage $nupkgPath -OutputPath $packageDir -Name $id
        $installedPath = Join-Path $packageDir $id
    }

    It 'reads the correct package information from a directory with installed nuget package' {

        $result = Get-NuGetPackageIdentityFromFolder $installedPath

        $expected = New-NugetPackageIdentity $id $version
        $result | Should -Be $expected
    }
}