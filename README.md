## NSwag: The Swagger API toolchain for .NET

**NSwag is still in development and not stable yet.**

NSwag is a Swagger 2.0 API toolchain for .NET and other platforms, written in C#. Swagger uses JSON Schema to describe types and JSON based HTTP REST web services. The primary intention of the project is to automatically generate client code from these Swagger files and integrate the whole process so that it can be used as easily as possible. 

**Swagger Generators:**

- ASP.NET Web API, [WebApiToSwaggerGenerator](https://github.com/NSwag/NSwag/wiki/WebApiToSwaggerGenerator)
- Types from .NET assemblies [AssemblyTypeToSwaggerGenerator](https://github.com/NSwag/NSwag/wiki/AssemblyTypeToSwaggerGenerator)

**Client Generators:** 

- TypeScript, [SwaggerToTypeScriptGenerator](https://github.com/NSwag/NSwag/wiki/SwaggerToTypeScriptGenerator)
- CSharp, [SwaggerToCSharpGenerator](https://github.com/NSwag/NSwag/wiki/SwaggerToCSharpGenerator)

[Read more about the available Swagger and Client Generators](https://github.com/NSwag/NSwag/wiki)

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
