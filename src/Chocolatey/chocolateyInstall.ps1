$packageName = 'NSwagStudio'
$fileType = 'msi'
$silentArgs = '/quiet'
$scriptPath =  $(Split-Path $MyInvocation.MyCommand.Path)
$fileFullPath = Join-Path $scriptPath 'NSwagStudio.msi'

try { 
  Install-ChocolateyInstallPackage $packageName $fileType $silentArgs $fileFullPath
} catch {
  Write-ChocolateyFailure $packageName $($_.Exception.Message)
  throw 
}
