#
# SquirrelReleasify.ps1
#

[CmdletBinding()]
param(
	[Parameter(Mandatory=$True)]
	[string]$AppName,
	[Parameter(Mandatory=$True)]
	[string]$Version,
	[Parameter(Mandatory=$True)]
	[string]$PackageFolder,
	[Parameter(Mandatory=$True)]
	[string]$ReleaseFolder,
	[Parameter(Mandatory=$True)]
	[string]$SquirrelPackageFolder
)

Set-Alias Squirrel "$SquirrelPackageFolder\squirrel.windows\1.8.0\tools\Squirrel.exe"

Write-Host "Contents of package directory:"
Get-ChildItem "$SquirrelPackageFolder" | Write-Output

Write-Host "Contents of release directory before:"
Get-ChildItem $ReleaseFolder | Write-Output

$NugetPackageFileName = $AppName + "." + $Version + ".nupkg"

Write-Host "Starting Squirrel releasify on" $PackageFolder\$NugetPackageFileName "release directory:" $ReleaseFolder

Squirrel --releasify $PackageFolder\$NugetPackageFileName --releaseDir $ReleaseFolder | Write-Output

Write-Host "Contents of release directory after:"
Get-ChildItem $ReleaseFolder | Write-Output