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

    #$TestDepends = Join-Path $ProjectPath submodules\PSDepend\Tests\DependFiles
    #$TestDependsLocal = Resolve-Path $PSScriptRoot/DependFiles
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

Describe "New-NugetPackage PS$PSVersion with commandline parameter only" {
    BeforeAll {
        $id = 'Comp.Test1'
        $version = '1.0.0'
        $nupkg = "$id.$version.nupkg"
        $authors = @("a.b@c.d", "x.y")
        $sep = [System.Environment]::NewLine
        $desc = "line 1${sep}line 2"

        $repoType = 'git'
        $repoCommit = '3f35aed'
        $repoUrl = 'https://github.com/ckolumbus/NuGet.Powershell'

        $nupkgPath = Join-Path $packageDir $nupkg
    }

    It 'creation should succeed' {
        New-NugetPackage `
                $id $version $authors $desc "$TestData/content" `
                -Dependencies @{ "a" = @{Version="1.0.0"; Includes = "none"}; "b" = @{ Version="2.0.0"; Excludes="all"}; "c" = "3.0.0"} `
                -FrameworkAssemblies @{"System.Data"="net48"; "System.IO" = ".NETFramework4.8"} `
                -FilesMapping @{"$TestData\content2\lib\lib.txt" = "./lib/lib2.txt"} `
                -OutputPath $packageDir `
                -RepositoryInfo @{Url=$repoUrl; Type=$repoType; Commit=$repoCommit}

        $nupkgPath | Should -Exist

    }

    Context "expanded nuget package" {
        BeforeAll {
            # rename needed for PS5 Expand-Archive
            Rename-Item  $nupkgPath "$nupkgPath.zip"
            $dst = Join-Path "$packageDir" "$id.$version"
            Expand-Archive "$nupkgPath.zip" -DestinationPath $dst
        }

        It 'only contains the files provided on the command line' {
            # filter out all nupkg internal files
            $files = Get-ChildItem -File  -Recurse $dst |
                Where-Object { $_.DirectoryName.ToString() -notmatch '_rels' } |
                Where-Object { $_.DirectoryName.ToString() -notmatch 'services' } |
                Where-Object { $_.Name -ne '[Content_Types].xml' } |
                Where-Object { $_.Name -ne "$id.nuspec" } |
                Select-Object -ExpandProperty FullName |  Sort-Object

            $files | Should -Be @(
                [IO.Path]::Combine($dst,"lib","lib2.txt"),
                [IO.Path]::Combine($dst,"test.txt")
            ) -Because "only 'lib/lib2.txt' and 'test.txt' where packaged"
        }

        Context 'has a a nuspec file that' {
            BeforeAll {
                $nuspec = New-Object xml
                $nuspec.Load( (Convert-Path (Join-Path $dst "$id.nuspec") ) )
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

            It "has correct Repository data" {
                $nuspec.package.metadata.repository.type = $repoType
                $nuspec.package.metadata.repository.url = $repoUrl
                $nuspec.package.metadata.repository.commit = $repoCommit
            }

            It "has correct Dependency Info" {
                $deps = $nuspec.package.metadata.dependencies.group.ChildNodes | Sort-Object -Property id | Select-Object -Property id,version,include,exclude
                $deps.Count | Should -Be 3

                $deps[0].id | Should -Be  "a"
                $deps[0].version | Should -Be  "1.0.0"
                $deps[0].include | Should -Be  "none"
                $deps[0].exclude | Should -Be  $null

                $deps[1].id | Should -Be  "b"
                $deps[1].version | Should -Be  "2.0.0"
                $deps[1].include | Should -Be  $null
                $deps[1].exclude | Should -Be  "all"

                $deps[2].id | Should -Be  "c"
                $deps[2].version | Should -Be  "3.0.0"
                $deps[2].include | Should -Be  $null
                $deps[2].exclude | Should -Be  $null
            }

            It "has correct Framework Reference Assembly Info" {
                $fw = $nuspec.package.metadata.frameworkAssemblies.ChildNodes | Sort-Object -Property assemblyName | Select-Object -Property assemblyName, targetFramework
                $fw.Count | Should -Be 2

                $fw[0].assemblyName | Should -Be "System.Data"
                $fw[0].targetFramework | Should -Be ".NETFramework4.8"

                $fw[1].assemblyName | Should -Be "System.IO"
                $fw[1].targetFramework | Should -Be ".NETFramework4.8"
            }
        }

    }
}

Describe "New-NugetPackage PS$PSVersion with nuspec file" {
    BeforeAll {
        $id = 'ComponentTest'  # different from nuspec filename
        $version = '2.0.0-alpha.1'
        $nupkg = "$id.$version.nupkg"

        $authors = @("a@b.c", "x@y.z")
        $repoType = 'git'
        $repoCommit = 'e1c65e4524cd70ee6e'
        $repoUrl = 'https://github.com/ckolumbus/NuGet.Powershell'
        $desc = "test"

        $nupkgPath = Join-Path $packageDir $nupkg
    }

    It 'creation should succeed' {
        New-NugetPackage  -ManifestFile (Join-Path $TestData "Comp.Test2.nuspec") -OutputPath $packageDir

        $nupkgPath | Should -Exist

    }

    Context "expanded nuget package" {
        BeforeAll {
            # rename needed for PS5 Expand-Archive
            Rename-Item  $nupkgPath "$nupkgPath.zip"
            $dst = Join-Path "$packageDir" "$id.$version"
            Expand-Archive "$nupkgPath.zip" -DestinationPath $dst
        }

        It 'should contain exactly the files provided in the nuspec file' {
            # filter out all nupkg internal files
            $files = Get-ChildItem -File  -Recurse $dst |
            Where-Object { $_.DirectoryName.ToString() -notmatch '_rels' } |
            Where-Object { $_.DirectoryName.ToString() -notmatch 'services' } |
            Where-Object { $_.Name -ne '[Content_Types].xml' } |
            Where-Object { $_.Name -ne "$id.nuspec" } |
            Select-Object -ExpandProperty FullName |  Sort-Object

            $files | Should -Be @(
                [IO.Path]::Combine($dst, "lib1", "test.txt"),
                [IO.Path]::Combine($dst, "lib2", "lib", "lib.txt")
            ) -Because "only 'lib1/test.txt' and 'lib2/lib/lib.txt' where packaged"
        }

        Context 'has a a nuspec file that' {
            BeforeAll {
                $nuspec = New-Object xml
                $nuspec.Load( (Convert-Path (Join-Path $dst "$id.nuspec") ) )
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

            It "has correct Repository data" {
                $nuspec.package.metadata.repository.type = $repoType
                $nuspec.package.metadata.repository.url = $repoUrl
                $nuspec.package.metadata.repository.commit = $repoCommit
            }

            It "has correct Dependency Info" {
                $deps = $nuspec.package.metadata.dependencies.group.ChildNodes | Sort-Object -Property id | Select-Object -Property id, version, include, exclude
                $deps.Count | Should -Be 2

                $deps[0].id | Should -Be  "Comp1"
                $deps[0].version | Should -Be  "[1.0.0]"
                $deps[0].include | Should -Be  "none"
                $deps[0].exclude | Should -Be  $null

                $deps[1].id | Should -Be  "Comp2"
                $deps[1].version | Should -Be  "[2.0.0]"
                $deps[1].include | Should -Be  $null
                $deps[1].exclude | Should -Be  "all"
            }
        }
    }
}

Describe "New-NugetPackage PS$PSVersion with nuspec & cmdline overwrites/addons" {
    BeforeAll {
        $id = 'Comp.Test3'  # different from nuspec filename
        $version = '3.1.0'
        $nupkg = "$id.$version.nupkg"

        $authors = @("a@b.c", "x@y.z")
        $repoType = 'git'
        $repoCommit = 'e1c65e4524cd70ee6e'
        $repoUrl = 'https://github.com/ckolumbus/NuGet.Powershell'
        $desc = "test"

        $nupkgPath = Join-Path $packageDir $nupkg
    }

    It 'creation should succeed' {
        New-NugetPackage  `
            -ManifestFile (Join-Path $TestData "Comp.Test3.nuspec") `
                $id $version $authors $desc "$TestData/content2" `
            -Dependencies @{ "Comp2" = "2.0.0" } `
            -RepositoryInfo @{Url=$repoUrl; Type=$repoType; Commit=$repoCommit} `
            -OutputPath $packageDir

        $nupkgPath | Should -Exist
    }


    Context "The expanded package" {
        BeforeAll {
            # rename needed for PS5 Expand-Archive
            Rename-Item  $nupkgPath "$nupkgPath.zip"
            $dst = Join-Path "$packageDir" "$id.$version"
            Expand-Archive "$nupkgPath.zip" -DestinationPath $dst
        }

        It 'should contain exactly the files provided in the nuspec file, amended by cmdline parameter' {
            # filter out all nupkg internal files
            $files = Get-ChildItem -File  -Recurse $dst |
                Where-Object { $_.DirectoryName.ToString() -notmatch '_rels' } |
                Where-Object { $_.DirectoryName.ToString() -notmatch 'services' } |
                Where-Object { $_.Name -ne '[Content_Types].xml' } |
                Where-Object { $_.Name -ne "$id.nuspec" } |
                Select-Object -ExpandProperty FullName |  Sort-Object

            $files | Should -Be @(
                [IO.Path]::Combine($dst,"lib", "lib.txt"),
                [IO.Path]::Combine($dst,"lib1","test.txt")
            ) -Because "only 'lib/lib.txt' and 'lib1/test.txt' where packaged"
        }

        Context 'has a a nuspec file that' {
            BeforeAll {
                $nuspec = New-Object xml
                $nuspec.Load( (Convert-Path (Join-Path $dst "$id.nuspec") ) )
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

            It "has correct Repository data" {
                $nuspec.package.metadata.repository.type = $repoType
                $nuspec.package.metadata.repository.url = $repoUrl
                $nuspec.package.metadata.repository.commit = $repoCommit
            }

            It "has correct Dependency Info" {
                $deps = $nuspec.package.metadata.dependencies.group.ChildNodes | Sort-Object -Property id | Select-Object -Property id, version, include, exclude
                $deps.Count | Should -Be 2

                $deps[0].id | Should -Be  "Comp1"
                $deps[0].version | Should -Be  "[1.0.0]"
                $deps[0].include | Should -Be  "none"
                $deps[0].exclude | Should -Be  $null

                $deps[1].id | Should -Be  "Comp2"
                $deps[1].version | Should -Be  "2.0.0"
                $deps[1].include | Should -Be  $null
                $deps[1].exclude | Should -Be  $null
            }
        }
    }
}

Describe "New-NugetPackage PS$PSVersion with nuspec properties" {
    BeforeAll {
        $id = 'Comp.Test4'  # different from nuspec filename
        $version = '4.1.0'
        $nupkg = "$id.$version.nupkg"

        $authors = @("a@b.c", "x@y.z")
        $desc = "test"

        $nupkgPath = Join-Path $packageDir $nupkg
    }

    It 'creation should succeed' {
        New-NugetPackage  `
            -ManifestFile (Join-Path $TestData "Comp.Properties.nuspec") `
            -ContentPath (Join-Path $TestData "content") `
            -Properties @{id=$id; version=$version; desc=$desc; authors=($authors -join ",")}  `
            -OutputPath $packageDir

        $nupkgPath | Should -Exist
    }


    Context "The expanded package" {
        BeforeAll {
            # rename needed for PS5 Expand-Archive
            Rename-Item  $nupkgPath "$nupkgPath.zip"
            $dst = Join-Path "$packageDir" "$id.$version"
            Expand-Archive "$nupkgPath.zip" -DestinationPath $dst
        }

        It 'should contain exactly the files provided in the nuspec file' {
            # filter out all nupkg internal files
            $files = Get-ChildItem -File  -Recurse $dst |
                Where-Object { $_.DirectoryName.ToString() -notmatch '_rels' } |
                Where-Object { $_.DirectoryName.ToString() -notmatch 'services' } |
                Where-Object { $_.Name -ne '[Content_Types].xml' } |
                Where-Object { $_.Name -ne "$id.nuspec" } |
                Select-Object -ExpandProperty FullName |  Sort-Object

            $files | Should -Be @(
                [IO.Path]::Combine($dst, "test.txt")
            ) -Because "only 'test.txt' where packaged"
        }

        Context 'has a a nuspec file that' {
            BeforeAll {
                $nuspec = New-Object xml
                $nuspec.Load( (Convert-Path (Join-Path $dst "$id.nuspec") ) )
            }

            It "has substituted Id" {
                $nuspec.package.metadata.id | Should -Be $id
            }

            It "has substituted Version" {
                $nuspec.package.metadata.version | Should -Be $version
            }

            It "has substituted Description" {
                $nuspec.package.metadata.description | Should -Be $desc
            }

            It "has substituted Authors" {
                $nuspec.package.metadata.authors | Should -Be ($authors -join ",")
            }
        }
    }
}