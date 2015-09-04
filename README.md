## NSwag: Swagger API toolchain for .NET

NSwag is a Swagger API toolchain for .NET and other platforms written in C#. Swagger uses JSON Schema to describe types and JSON based HTTP REST web services. The primary intention of the project is to automatically generate client code from these Swagger files and integrate the whole process so that it can be used as easily as WCF web service references in .NET. 

- Swagger Generators: ASP.NET Web API, Tyes from .NET assemblies
- Client Generators: TypeScript, CSharp

[Read more about the available Swagger and Client Generators](https://github.com/NSwag/NSwag/wiki)

[**Downloads and Build Artifacts** (command line tool and NSwagStudio)](https://ci.appveyor.com/project/rsuter/nswag/build/artifacts)

This project uses [NJsonSchema for .NET](http://njsonschema.org) for JSON Schema generation and [NConsole for .NET](https://github.com/NConsole/NConsole) for command line argument parsing. 

### Usage

The following code shows how to generate C# client classes to call the web service: 
	
	var service = SwaggerService.FromJson("...");
	
	var generator = new SwaggerToCSharpGenerator(service);
	generator.Class = "MyClass";
	generator.Namespace = "MyNamespace";
	
	var code = generator.GenerateFile();
