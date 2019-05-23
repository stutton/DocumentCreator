Write-Host "Creating github release tag..."
$buildNumber = "$(Build.BuildNumber)"
Write-Host "Build number: $buildNumber"
If ($buildNumber.Contains('+')) {
    $GitHubTag =$buildNumber.Substring(0, $buildNumber.IndexOf('+'))
    $GitHubTag = "v$GitHubTag"
}
Write-Host "Tag: $GitHubTag"
Write-Host "Exporting variable..."
Write-Output ("##vso[task.setvariable variable=GitHubTag;]$GitHubTag")