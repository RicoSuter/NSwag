Try
{
    New-Item -ItemType directory -Force -Path "$PSScriptRoot/Output"

    Invoke-WebRequest -Uri 'http://localhost:65384/swagger_new_ui/v1/swagger.json' -OutFile "$PSScriptRoot/Output/swagger_new_v2.json"

    Invoke-WebRequest -Uri 'http://localhost:65384/swagger_new_v3/v1/swagger.json' -OutFile "$PSScriptRoot/Output/swagger_new_v3.json"

    Invoke-WebRequest -Uri 'http://localhost:65384/swagger_old_ui/v1/swagger.json' -OutFile "$PSScriptRoot/Output/swagger_old_v2.json"

    Invoke-WebRequest -Uri 'http://localhost:65384/swagger_old_v3/v1/swagger.json' -OutFile "$PSScriptRoot/Output/swagger_old_v3.json"
}
Catch
{
    "Could not download the OpenAPI/Swagger specifications: "    
    Throw
}

