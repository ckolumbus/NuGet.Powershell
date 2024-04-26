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

    #$TestDepends = Join-Path $ProjectPath submodules\PSDepend\Tests\DependFiles
    #$TestDependsLocal = Resolve-Path $PSScriptRoot/DependFiles
}

Describe "Module within PS$PSVersion" {
    Context 'Strict mode' {
        BeforeAll {
            Set-StrictMode -Version latest
        }

        It 'Should load' {
            $Module = Get-Module $ProjectName
            $Module.Name | Should -Be $ProjectName
            $Module.ExportedCommands.Keys -contains 'New-NuGetPackage' | Should -Be $True
        }
    }
}

Describe "Manifest PS$PSVersion" -Skip {
    Context 'Strict mode' {
        BeforeAll {
            Set-StrictMode -Version latest
        }

        It 'Should be valid only with commandline parameters' {
            $true | Should -Be $true
        }

        It 'Should be valid only with provided manifest file path' {
            $true | Should -Be $true
        }

        It 'Should be valid with full manifest file and overwriting via command line parameter' {
            $true | Should -Be $true
        }
    }
}
