param (
    [Parameter(Position = 0)] $Tasks,
    [string] $PackagePath = "./packages"
)

# Ensure and call the module.
if ([System.IO.Path]::GetFileName($MyInvocation.ScriptName) -ne 'Invoke-Build.ps1') {
    try {
        Invoke-Build -Task $Tasks -File $MyInvocation.MyCommand.Path @PSBoundParameters
        exit 0
    }
    catch {
        exit -1
    }
}

function _ensureDir ($dir) {
    New-Item -Path $dir -ItemType Directory -ErrorAction Ignore | Out-Null
}

Task . Clean, Build

Task Clean {
    remove $PackagePath
}

Task Build {
    _ensureDir $PackagePath
    $outdir = (Resolve-Path $PackagePath)

    foreach ( $n in (Get-ChildItem -File "./data/*.nuspec") ) 
    {
       Write-Output "Processing: ${n}"
       try {
           New-NugetPackage -ManifestFile $n -OutputPath $outdir   | Out-Null
       } catch {
           Write-Warning $_
       }
    }
}

