## NSwag: The Swagger API toolchain for .NET

[![Build status](https://ci.appveyor.com/api/projects/status/aajfgxqf5dic7tkk?svg=true)](https://ci.appveyor.com/project/rsuter/nswag)

NSwag is a Swagger 2.0 API toolchain for .NET, TypeScript and other platforms, written in C#. The [Swagger specification](http://swagger.io) uses JSON and JSON Schema to describe a RESTful web API. The project provides tools to automatically generate client code from these Swagger specifications and integrate this generation into existing processes. 

**Swagger Generators:**

- ASP.NET Web API
    - [WebApiToSwaggerGenerator](https://github.com/NSwag/NSwag/wiki/WebApiToSwaggerGenerator)
    - [WebApiAssemblyToSwaggerGenerator](https://github.com/NSwag/NSwag/wiki/WebApiAssemblyToSwaggerGenerator)
- Types from .NET assemblies, [AssemblyToSwaggerGenerator](https://github.com/NSwag/NSwag/wiki/AssemblyToSwaggerGenerator)

**Client Generators:** 

- TypeScript, [SwaggerToTypeScriptGenerator](https://github.com/NSwag/NSwag/wiki/SwaggerToTypeScriptGenerator)
- CSharp, [SwaggerToCSharpGenerator](https://github.com/NSwag/NSwag/wiki/SwaggerToCSharpGenerator)

**Ways to use the toolchain:** 

- In your C# code
- Via [command line](https://github.com/NSwag/NSwag/wiki/CommandLine)
- Generate code with [T4 templates](https://github.com/NSwag/NSwag/wiki/T4) in Visual Studio
- Windows GUI [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio)

[**Downloads and Build Artifacts** (command line tool and NSwagStudio)](https://ci.appveyor.com/project/rsuter/nswag/build/artifacts)

This project uses [NJsonSchema for .NET](http://njsonschema.org) for JSON Schema generation. 

### Usage in C&#35;

The following code shows how to generate C# client classes to call a web service: 
	
	var service = SwaggerService.FromJson("...");
	
	var generator = new SwaggerToCSharpGenerator(service);
	generator.Class = "MyClass";
	generator.Namespace = "MyNamespace";
	
	var code = generator.GenerateFile();
