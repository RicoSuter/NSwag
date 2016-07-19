$packageName = "NSwagStudio";
$fileType = 'msi';
$silentArgs = '/quiet /qr /norestart'
$validExitCodes = @(0)

try {
  	$packageGuid = Get-ChildItem HKLM:\SOFTWARE\Classes\Installer\Products |
    	Get-ItemProperty -Name 'ProductName' |
    	? { $_.ProductName -like $packageName + "*"} |
    	Select -ExpandProperty PSChildName -First 1
	
	$properties = Get-ItemProperty HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products\$packageGuid\InstallProperties
	
	$file = $properties.LocalPackage
	
	# Would like to use the following, but looks like there is a bug in this method when uninstalling MSI's
	# Uninstall-ChocolateyPackage $packageName $fileType $silentArgs $file -validExitCodes $validExitCodes
	
	# Use this instead
	$msiArgs = "/x $file $silentArgs";
	Start-ChocolateyProcessAsAdmin "$msiArgs" 'msiexec' -validExitCodes $validExitCodes
	
	Write-ChocolateySuccess $package
}
catch {
	Write-ChocolateyFailure $package "$($_.Exception.Message)"
	throw
}
