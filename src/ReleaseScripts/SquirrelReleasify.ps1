#
# SquirrelReleasify.ps1
#

[CmdletBinding()]
param(
	[Parameter(Mandatory=$True)]
	[string]$AppName,
	[Parameter(Mandatory=$True)]
	[string]$PackageFolder,
	[Parameter(Mandatory=$True)]
	[string]$ReleaseFolder
)

Set-Alias Squirrel '.\packages\squirrel.windows.1.8.0\tools\Squirrel.exe'

# Regular expression pattern to find the version in the build number 
$VersionRegex = "\d+\.\d+\.\d+\.\d+"

$VersionData = [regex]::matches($Env:BUILD_BUILDNUMBER,$VersionRegex)
$NugetPackageFileName = $appName + "." + $VersionData + ".nupkg"

Write-Host "Starting Squirrel releasify on" $packageFolder\$NugetPackageFileName "release directory:" $releaseFolder

Squirrel --releasify $packageFolder\$NugetPackageFileName --releaseDir $releaseFolder | Write-Output