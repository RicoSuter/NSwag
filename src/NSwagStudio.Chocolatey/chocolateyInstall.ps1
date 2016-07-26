$packageName = 'NSwagStudio'
$fileType = 'msi'
$silentArgs = '/quiet'
$scriptPath =  $(Split-Path $MyInvocation.MyCommand.Path)
$fileFullPath = Join-Path $scriptPath 'NSwagStudio.msi'

Install-ChocolateyInstallPackage $packageName $fileType $silentArgs $fileFullPath