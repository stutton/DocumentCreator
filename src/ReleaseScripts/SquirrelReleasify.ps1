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

Set-Alias Squirrel "$SquirrelPackageFolder\squirrel.windows\2.0.1\tools\Squirrel.exe"

If ($Version.Contains('+')) {
	$Version = $Version.Substring(0, $Version.IndexOf('+'))
}
Write-Output ("##vso[task.setvariable variable=Version;]$Version")

Write-Host "Begin SquirrelReleasify for version $Version"

Write-Host "Contents of package directory:"
Get-ChildItem "$SquirrelPackageFolder" | Write-Output

Write-Host "Contents of release directory before:"
Get-ChildItem $ReleaseFolder | Write-Output

$NugetPackageFileName = $AppName + "." + $Version + ".nupkg"

Write-Host "Starting Squirrel releasify on" $PackageFolder\$NugetPackageFileName "release directory:" $ReleaseFolder

Squirrel --releasify $PackageFolder\$NugetPackageFileName --releaseDir $ReleaseFolder | Write-Output

Write-Host "Contents of release directory after:"
Get-ChildItem $ReleaseFolder | Write-Output