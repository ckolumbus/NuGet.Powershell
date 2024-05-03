$dst = "$PSScriptRoot/../deps"

if (-not (Test-Path $dst) ){
    New-Item -ItemType Directory $dst
}

$modules = @(
    "InvokeBuild",
    "Pester"
)

foreach ($m in $modules) {
    $modulePath = Join-Path $dst $m
    if (-not (Test-Path $modulePath) ) {
        Save-Module $m -Path $dst
    }
    Import-Module $modulePath
}