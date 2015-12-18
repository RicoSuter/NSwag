## NSwag: The Swagger API toolchain for .NET

[![Build status](https://ci.appveyor.com/api/projects/status/aajfgxqf5dic7tkk?svg=true)](https://ci.appveyor.com/project/rsuter/nswag)
[![NuGet Version](http://img.shields.io/nuget/v/NSwag.Core.svg?style=flat)](https://www.nuget.org/packages?q=NSwag)

NSwag is a Swagger 2.0 API toolchain for .NET, TypeScript and other platforms, written in C#. The [Swagger specification](http://swagger.io) uses JSON and JSON Schema to describe a RESTful web API. The project provides tools to automatically generate client code from these Swagger specifications and integrate this generation into existing processes. 

**Ways to use the toolchain:** 

- Simple to use Windows GUI, [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio)
- In your C# code, via [NuGet](https://www.nuget.org/packages?q=NSwag)
- Via [command line](https://github.com/NSwag/NSwag/wiki/CommandLine) (Windows, Mac and Linux support through [Mono](http://www.mono-project.com/))
- Generate code with [T4 templates](https://github.com/NSwag/NSwag/wiki/T4) in Visual Studio

**Swagger Generators:**

- ASP.NET Web API
    - [WebApiToSwaggerGenerator](https://github.com/NSwag/NSwag/wiki/WebApiToSwaggerGenerator)
    - [WebApiAssemblyToSwaggerGenerator](https://github.com/NSwag/NSwag/wiki/WebApiAssemblyToSwaggerGenerator)
- Types from .NET assemblies, [AssemblyTypeToSwaggerGenerator](https://github.com/NSwag/NSwag/wiki/AssemblyTypeToSwaggerGenerator)

**Client Generators:** 

- CSharp, [SwaggerToCSharpGenerator](https://github.com/NSwag/NSwag/wiki/SwaggerToCSharpGenerator)
	- With [INotifyPropertyChanged](https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx) supporting DTOs
- TypeScript, [SwaggerToTypeScriptGenerator](https://github.com/NSwag/NSwag/wiki/SwaggerToTypeScriptGenerator)
	- Available templates/supported libraries: 
		- JQuery with Callbacks, `JQueryCallbacks`
		- JQuery with Q promises `JQueryQPromises`
		- AngularJS using $http, `AngularJS`

**Downloads**

- [Download latest **NSwagStudio MSI installer**](http://rsuter.com/Projects/NSwagStudio/installer.php) (Windows Desktop application)
- [Download latest **NSwag command line tools** and NSwagStudio as ZIP archive](http://rsuter.com/Projects/NSwagStudio/archive.php)
- [Download latest **Build Artifacts** from AppVeyor](https://ci.appveyor.com/project/rsuter/nswag/build/artifacts) (command line tools and NSwagStudio binaries)

This project uses [NJsonSchema for .NET](http://njsonschema.org) for JSON Schema, C# and TypeScript class/interface generation. 

### Usage in C&#35;

The following code shows how to read a Swagger specification and generate C# client classes to call the described web services: 
	
	var service = SwaggerService.FromJson("...");
	
	var settings = new SwaggerToCSharpGeneratorSettings 
	{
		ClassName = "MyClass",
		Namespace = "MyNamespace"
	};
	
	var generator = new SwaggerToCSharpGenerator(service, settings);
	var code = generator.GenerateFile();

Check out the [project Wiki](https://github.com/NSwag/NSwag/wiki) for more information.

### NSwagStudio

The generators can be used in a confortable and simple Windows GUI called NSwagStudio: 

[![](https://raw.githubusercontent.com/wiki/NSwag/NSwag/NSwagStudioScreenshot01.png)](https://raw.githubusercontent.com/wiki/NSwag/NSwag/NSwagStudioScreenshot01.png)
