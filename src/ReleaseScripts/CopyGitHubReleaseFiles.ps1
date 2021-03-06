#
# CopyGitHubReleaseFiles.ps1
#

[CmdletBinding()]
param(
	[Parameter(Mandatory=$True)]
	[string]$ReleaseFolder,
	[Parameter(Mandatory=$True)]
	[string]$Version,
	[Parameter(Mandatory=$False)]
	[string]$GitHubReleaseFolder
)

If ($Version.Contains('+')) {
	$Version = $Version.Substring(0, $Version.IndexOf('+'))
}

Write-Host "Creating GitHubReleaseFolder..."
if([string]::IsNullOrEmpty($GitHubReleaseFolder)) {
	$GitHubReleaseFolder = Join-Path -Path $ReleaseFolder -ChildPath "github"
}
New-Item -ItemType Directory -Path $GitHubReleaseFolder | Out-Null

Write-Host "Looking for full and delta NuGet files..."
$fullNuGet = Get-ChildItem -Path $ReleaseFolder | Where-Object { $_.Name -eq "DocumentCreator-$Version-full.nupkg" }
Write-Host "Found '$($fullNuGet.Name)'"

$deltaNuGet = Get-ChildItem -Path $ReleaseFolder | Where-Object { $_.Name -eq "DocumentCreator-$Version-delta.nupkg" }
Write-Host "Found '$($deltaNuGet.Name)'"

$fileToCopy = @()
$fileToCopy += Join-Path -Path $ReleaseFolder -ChildPath "Setup.exe"
$fileToCopy += Join-Path -Path $ReleaseFolder -ChildPath "Setup.msi"
$fileToCopy += Join-Path -Path $ReleaseFolder -ChildPath "RELEASES"
$fileToCopy += $fullNuGet.FullName
$fileToCopy += $deltaNuGet.FullName

Write-Host "GitHub release folder path: " -NoNewline 
Write-Host $GitHubReleaseFolder
Write-Host
Write-Host "Files:"
$fileToCopy -join '`n' | Out-Host

Write-Host "Copying files..."
foreach ($file in $fileToCopy) {
	Copy-Item -Path $file -Destination $GitHubReleaseFolder
}

Write-Host "GitHub folder contents:"
Get-ChildItem $GitHubReleaseFolder | Out-Host