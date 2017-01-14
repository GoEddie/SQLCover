$root = $PSScriptRoot
$lib = "$root\.package\lib\45\"
If (Test-Path $lib)
{
	Remove-Item $lib -recurse
}
new-item -Path $lib -ItemType directory
new-item -Path $root\.nupkg -ItemType directory -force
Copy-Item $root\src\SQLCover\SQLCover\bin\Debug\* $lib

Write-Host "Setting .nuspec version tag to $env:nugetVersion"

$content = (Get-Content $root\SqlCover.nuspec -Encoding UTF8) 
$content = $content -replace '\$version\$',$env:nugetVersion

$content | Out-File $root\.package\SqlCover.compiled.nuspec -Encoding UTF8

If (!(Test-Path $root\.nuget))
{
	new-item -Path $root\.nuget -ItemType directory -force
}
If (!(Test-Path $root\.nuget\nuget.exe))
{
    $sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
    $targetNugetExe = "$root\.nuget\nuget.exe"
    Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe
    Set-Alias nuget $targetNugetExe -Scope Global -Verbose
}

& nuget pack $root\.package\SqlCover.compiled.nuspec -Version $env:nugetVersion -OutputDirectory $root\.nupkg
