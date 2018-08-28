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

Set-Alias Squirrel "$SquirrelPackageFolder\squirrel.windows.*\tools\Squirrel.exe"

# Regular expression pattern to find the version in the build number 
$VersionRegex = "\d+\.\d+\.\d+\.\d+"

$VersionData = [regex]::matches($Version,$VersionRegex)
$NugetPackageFileName = $AppName + "." + $VersionData + ".nupkg"

Write-Host "Starting Squirrel releasify on" $pPackageFolder\$NugetPackageFileName "release directory:" $ReleaseFolder

Squirrel --releasify $PackageFolder\$NugetPackageFileName --releaseDir $ReleaseFolder | Write-Output