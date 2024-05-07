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

Describe "Install-NuGetPackage PS$PSVersion" {
    BeforeAll {
        $id = 'Comp.Test1'
        $version = '1.0.0'
        $nupkg = "$id.$version.nupkg"

        $id2 = 'Comp.Test2'
        $version2 = '2.0.0'
        $nupkg2 = "$id2.$version2.nupkg"

        $authors = @("a.b@c.d", "x.y")
        $sep = [System.Environment]::NewLine
        $desc = "line 1${sep}line 2"

        $nupkgPath = Join-Path $packageDir $nupkg
        $nupkgPath2 = Join-Path $packageDir $nupkg2

        New-NugetPackage $id $version $authors $desc "$TestData/content" -OutputPath $packageDir
        New-NugetPackage $id2 $version2 $authors $desc "$TestData/content" -OutputPath $packageDir

        $installDst = Join-Path $packageDir $id
    }

    It 'installs a locally stored NuGet packages into correct directory' {

        $targetDir = Install-NuGetPackage -Path $nupkgPath -OutputPath $packageDir

        $targetDir | Should -Be $installDst
        $installDst | Should -Exist
        Join-Path $installDst "$id.nuspec" | Should -Exist
    }

    It 'installs a locally stored NuGet packages with -UseSideBySidePaths parameter into correct side-by-side directory' {

        $installDstVersion = "${installDst}.${version}"
        $targetDir = Install-NuGetPackage -Path $nupkgPath -OutputPath $packageDir -UseSideBySidePaths

        $targetDir | Should -Be $installDstVersion
        $installDstVersion | Should -Exist
        Join-Path $installDstVersion "$id.nuspec" | Should -Exist
    }

    It 'installs a mulitple local NuGet packages with -Name parameter into correctly named directory' {
        $dstNames = "test1", "test2"

        $targetDirs = Install-NuGetPackage -Path $nupkgPath, $nupkgPath2 -OutputPath $packageDir -Name $dstNames

        $dstTargetDirs = @((Join-Path $packageDir $dstNames[0]), (Join-Path $packageDir $dstNames[1]))
        $targetDirs | Should -Be $dstTargetDirs
    }

    It 'does not install any nuget package specific files' {
        # fget nupkg internal files
        $files = Get-ChildItem -File  -Recurse $installDst |
            Where-Object { $_.DirectoryName.ToString() -match '_rels' } |
            Where-Object { $_.DirectoryName.ToString() -match 'services' } |
            Where-Object { $_.Name -ne '[Content_Types].xml' } |
            Select-Object -ExpandProperty FullName |  Sort-Object

        $files | Should -Be @()
    }

    Context 'has a nuspec file that' {
        BeforeAll {
            $nuspec = New-Object xml
            $nuspec.Load( (Convert-Path (Join-Path $installDst "$id.nuspec") ) )
        }

        It "has correct Id" {
            $nuspec.package.metadata.id | Should -Be $id
        }

        It "has correct Version" {
            $nuspec.package.metadata.version | Should -Be $version
        }

        It "has correct Description" {
            $nuspec.package.metadata.description | Should -Be $desc
        }

        It "has correct Authors" {
            $nuspec.package.metadata.authors | Should -Be ($authors -join ",")
        }
    }
}