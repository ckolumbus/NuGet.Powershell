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


Describe "Module within PS$PSVersion" {
    Context 'Strict mode' {
        BeforeAll {
            Set-StrictMode -Version latest
        }

        It 'Should have been loaded' {
            $Module = Get-Module $ProjectName
            $Module.Name | Should -Be $ProjectName
            $Module.ExportedCommands.Keys -contains 'New-NuGetPackage' | Should -Be $True
        }
    }
}
