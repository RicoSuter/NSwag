Features in this sample: 

- Expose swagger.json and Swagger UI 3 via HTTP (middleware)
    - See Startup.cs
    - Packages in Sample.AspNetCore31.csproj
- Automatically generate a C# client library on build (Sample.AspNetCore31.Client)
    - Add NSwag.MSBuild in Sample.AspNetCore31.csproj and
    - Add a build task in the .csproj
    - Output is in the Sample.AspNetCore31.Client library project