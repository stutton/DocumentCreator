#
# CopyAzureBlob.ps1
#

[CmdletBinding()]
param(
	[Parameter(Mandatory=$True)]
	[string]$ReleaseFolder,
	[Parameter(Mandatory=$True)]
	[string]$StorageAccountName,
	[Parameter(Mandatory=$True)]
	[string]$StorageAccountKey,
	[Parameter(Mandatory=$True)]
	[string]$Container
)

Write-Host "Authenticating with azure..."
$storageAccount = New-AzureStorageContext -StorageAccountName $StorageAccountName -StorageAccountKey $StorageAccountKey

Write-Host "Getting data from blob storage..."
$blobs = Get-AzureStorageBlob -Container $Container -Context $storageAccount

Write-Host "Creating local destination directory..."
New-Item -ItemType Directory -Force -Path $ReleaseFolder

Write-Host "Copy blobs to destination directory..."
foreach($blob in $blobs){
	Get-AzureStorageBlobContent -Container $Container -Blob $blob.Name -Force -Destination $ReleaseFolder -Context $storageAccount
}