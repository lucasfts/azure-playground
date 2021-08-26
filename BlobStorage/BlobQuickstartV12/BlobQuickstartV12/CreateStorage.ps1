$resourceGroup = ""
$storageName = ""
$location = ""


New-AzStorageAccount -ResourceGroupName $resourceGroup `
	-Name $storageName `
	-Location $location `
	-SkuName Standard_RAGRS `
	-Kind StorageV2