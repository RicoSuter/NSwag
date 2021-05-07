$configuration = $args[0]
$cliPath = "$PSScriptRoot/../src/NSwagStudio/bin/$configuration"
$samplesPath = "$PSScriptRoot/../samples"

function NSwagRun([string]$projectDirectory, [string]$configurationFile, [string]$runtime, [string]$configuration, [string]$build)
{
  $nswagConfigurationFile = [IO.Path]::GetFullPath("$projectDirectory/$configurationFile.nswag")
  $nswagSwaggerFile = [IO.Path]::GetFullPath("$projectDirectory/$($configurationFile)_swagger.json")

  if (Test-Path "$nswagSwaggerFile")
  {
    Remove-Item $nswagSwaggerFile
  }
  
  if ($build -eq "true")
  {
    dotnet build "$projectDirectory" -c $configuration
  }
  else
  {
    dotnet restore "$projectDirectory"
  }

  dotnet "$cliPath/$runtime/dotnet-nswag.dll" run "$nswagConfigurationFile" /variables:configuration=$configuration

  if (!(Test-Path "$nswagSwaggerFile"))
  {
    throw "Output ($($configurationFile)_swagger.json) not generated for $nswagConfigurationFile."
  }
}

# WithoutMiddleware/Sample.AspNetCore20
# NSwagRun "$samplesPath/WithoutMiddleware/Sample.AspNetCore20" "nswag_project" "NetCore21" "Release" false
# NSwagRun "$samplesPath/WithoutMiddleware/Sample.AspNetCore20" "nswag_assembly" "NetCore21" "Release" true

# NSwagRun "$samplesPath/WithoutMiddleware/Sample.AspNetCore20" "nswag_project" "NetCore21" "Debug" false
# NSwagRun "$samplesPath/WithoutMiddleware/Sample.AspNetCore20" "nswag_assembly" "NetCore21" "Debug" true

# WithoutMiddleware/Sample.AspNetCore21
NSwagRun "$samplesPath/WithoutMiddleware/Sample.AspNetCore21" "nswag_assembly" "NetCore21" "Release" true
NSwagRun "$samplesPath/WithoutMiddleware/Sample.AspNetCore21" "nswag_project" "NetCore21" "Release" false
NSwagRun "$samplesPath/WithoutMiddleware/Sample.AspNetCore21" "nswag_reflection" "NetCore21" "Release" true
 
NSwagRun "$samplesPath/WithoutMiddleware/Sample.AspNetCore21" "nswag_assembly" "NetCore21" "Debug" true
NSwagRun "$samplesPath/WithoutMiddleware/Sample.AspNetCore21" "nswag_project" "NetCore21" "Debug" false
NSwagRun "$samplesPath/WithoutMiddleware/Sample.AspNetCore21" "nswag_reflection" "NetCore21" "Debug" true

