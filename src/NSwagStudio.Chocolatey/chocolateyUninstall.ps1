$packageName = "NSwagStudio";
$fileType = 'msi';
$silentArgs = '/qn /norestart'
$validExitCodes = @(0)

$packageGuid = Get-ChildItem HKLM:\SOFTWARE\Classes\Installer\Products |
	Get-ItemProperty -Name 'ProductName' |
	? { $_.ProductName -like $packageName + "*"} |
	Select -ExpandProperty PSChildName -First 1
$properties = Get-ItemProperty HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products\$packageGuid\InstallProperties
$file = $properties.LocalPackage

$msiArgs = "/x $file $silentArgs";
Start-ChocolateyProcessAsAdmin "$msiArgs" 'msiexec' -validExitCodes $validExitCodes