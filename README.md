## NSwag: The Swagger API toolchain for .NET, Web API and TypeScript

[![NuGet Version](https://img.shields.io/nuget/v/NSwag.Core.svg)](https://www.nuget.org/packages?q=NSwag)
[![npm](https://img.shields.io/npm/v/nswag.svg)](https://www.npmjs.com/package/nswag)
[![Build status](https://img.shields.io/appveyor/ci/rsuter/nswag.svg?label=build)](https://ci.appveyor.com/project/rsuter/nswag)
[![Build status](https://img.shields.io/appveyor/ci/rsuter/nswag-25x6o.svg?label=CI+build)](https://ci.appveyor.com/project/rsuter/nswag-25x6o)
[![Gitter](https://img.shields.io/badge/gitter-join%20chat-1dce73.svg)](https://gitter.im/NSwag/NSwag)
[![StackOverflow](https://img.shields.io/badge/questions-on%20StackOverflow-orange.svg?style=flat)](http://stackoverflow.com/questions/tagged/nswag)
[![Donate](https://img.shields.io/badge/donate-via PayPal-green.svg?style=flat)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=KLXZF8GMQ5DUE)

NSwag is a Swagger 2.0 API (OpenAPI) toolchain for .NET, Web API, TypeScript (jQuery, AngularJS, Angular 2, Aurelia, KnockoutJS, and more) and other platforms, written in C#. The [Swagger specification](http://swagger.io) uses JSON and JSON Schema to describe a RESTful web API. The NSwag project provides tools to generate Swagger specifications from existing ASP.NET Web API controllers and client code from these Swagger specifications. 

The project combines the functionality of Swashbuckle (Swagger generation) and AutoRest (client generation) in one toolchain. This way a lot of incompatibilites can be avoided and features which are not well described by the Swagger specification or JSON Schema are better supported (e.g. [inheritance](https://github.com/NJsonSchema/NJsonSchema/wiki/Inheritance), [enum](https://github.com/NJsonSchema/NJsonSchema/wiki/Enums) and reference handling). The NSwag project heavily uses [NJsonSchema for .NET](http://njsonschema.org) for JSON Schema handling and C#/TypeScript class/interface generation. 

![ToolchainDiagram](assets/ToolchainDiagram.png)

The project is developed and maintained by [Rico Suter](http://rsuter.com) and other contributors. 

**Ways to use the toolchain:** 

- Simple to use Windows GUI, [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio)
- Via [command line](https://github.com/NSwag/NSwag/wiki/CommandLine) (Windows, Mac and Linux support through [Mono](http://www.mono-project.com/) or .NET Core console binary, also via NPM package)
- By using the [Swagger or Swagger UI OWIN and ASP.NET Core Middlewares](https://github.com/NSwag/NSwag/wiki/Middlewares) (also serves the [Swagger UI](http://swagger.io/swagger-ui)) (recommended)
- In your C# code, via [NuGet](https://www.nuget.org/packages?q=NSwag)
- In your [MSBuild targets](https://github.com/NSwag/NSwag/wiki/MSBuild)
- Generate code with [T4 templates](https://github.com/NSwag/NSwag/wiki/T4) in Visual Studio
- In your [Cake](https://cakebuild.net) scripts using [Cake.NSwag](https://agc93.github.io/Cake.NSwag/doc/intro.html) (external community project, may not use latest NSwag version)

**Tutorials**

- [Integrate the NSwag toolchain into your ASP.NET Web API project](https://blog.rsuter.com/nswag-tutorial-integrate-the-nswag-toolchain-into-your-asp-net-web-api-project/)
- [Generate an Angular 2 TypeScript client from an existing ASP.NET Web API web assembly](https://blog.rsuter.com/nswag-tutorial-generate-an-angular-2-typescript-client-from-an-existing-asp-net-web-api-web-assembly/)

**Swagger Generators:**

- ASP.NET Web API assembly to Swagger (supports .NET Core)
    - [WebApiToSwaggerGenerator](https://github.com/NSwag/NSwag/wiki/WebApiToSwaggerGenerator)
        - Generates a Swagger specification for Web API controllers
    - [WebApiAssemblyToSwaggerGenerator](https://github.com/NSwag/NSwag/wiki/WebApiAssemblyToSwaggerGenerator)
        - Generates a Swagger specification for controllers in an external Web API assembly
        - [Also supports loading of .NET Core assemblies](https://github.com/NSwag/NSwag/wiki/WebApiAssemblyToSwaggerGenerator#net-core)
    - [AssemblyTypeToSwaggerGenerator](https://github.com/NSwag/NSwag/wiki/AssemblyTypeToSwaggerGenerator)
         - Generates a Swagger specification containing only types from .NET assemblies

**Code Generators:** 

- **TypeScript Client**
	- [SwaggerToTypeScriptClientGenerator](https://github.com/NSwag/NSwag/wiki/SwaggerToTypeScriptClientGenerator)
		- Generates TypeScript clients from a Swagger specification
		- Available templates/supported libraries: 
			- JQuery with Callbacks, `JQueryCallbacks`
			- JQuery with promises `JQueryPromises`
			- AngularJS using $http, `AngularJS`
			- Angular 2 using the http service, `Angular2`
			- window.fetch API and ES6 promises, `Fetch` (use this template in your React/Redux app)
			- Aurelia using the HttpClient from aurelia-fetch-client, `Aurelia` (based on the Fetch template)
- **CSharp Client**
	- [SwaggerToCSharpClientGenerator](https://github.com/NSwag/NSwag/wiki/SwaggerToCSharpClientGenerator)
		- Generates C# clients from a Swagger specification
		- Generates POCOs or classes implementing [INotifyPropertyChanged](https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx) supporting DTOs
- **CSharp WebAPI Controllers** (contract first/schema first development)
	- [SwaggerToCSharpControllerGenerator](https://github.com/NSwag/NSwag/wiki/SwaggerToCSharpControllerGenerator)
	    - Generates Web API Controllers based on a Swagger specification
	    
**Downloads**

- [Download latest **NSwagStudio MSI installer**](http://rsuter.com/Projects/NSwagStudio/installer.php) (Windows Desktop application)
- [Download latest **NSwag command line tools** and NSwagStudio as ZIP archive](http://rsuter.com/Projects/NSwagStudio/archive.php)
- [Download latest **Build Artifacts** from AppVeyor](https://ci.appveyor.com/project/rsuter/nswag/build/artifacts) (command line tools and NSwagStudio binaries)

**NPM Packages**

- [NSwag](https://www.npmjs.com/package/nswag): Command line tools (.NET and .NET Core) distributed as NPM package

**NuGet Packages**

- [NSwag.Core](https://www.nuget.org/packages/NSwag.Core/) (PCL 259 / .NETStandard 1.0): 
    - The Swagger reader and writer classes ([Source Code](https://github.com/NSwag/NSwag/tree/master/src/NSwag.Core))
- [NSwag.Annotations](https://www.nuget.org/packages/NSwag.Annotations/) (PCL 259 / .NETStandard 1.0): 
    - Attributes to decorate Web API controllers to control the Swagger generation ([Source Code](https://github.com/NSwag/NSwag/tree/master/src/NSwag.Annotations))
- [NSwag.CodeGeneration](https://www.nuget.org/packages/NSwag.CodeGeneration/) (PCL 259 / .NETStandard 1.0): 
    - Classes to generate Swagger specifications from Web API controllers and C# and TypeScript clients ([Source Code](https://github.com/NSwag/NSwag/tree/master/src/NSwag.CodeGeneration))
- [NSwag.AssemblyLoader](https://www.nuget.org/packages/NSwag.AssemblyLoader/) (.NET 4.5+): 
    - Classes to load assemblies in an isolated AppDomain and generate Swagger specs from Web API controllers
- [NSwag.AssemblyLoaderCore](https://www.nuget.org/packages/NSwag.AssemblyLoaderCore/) (.NET Core, .NETStandard 1.6): 
    - Classes to load assemblies in an AssemblyLoaderContext and generate Swagger specs from Web API controllers
- [NSwag.MSBuild](https://www.nuget.org/packages/NSwag.MSBuild/) (MSBuild .targets): 
    - Adds a .targets file to your Visual Studio project, so that you can run the NSwag command line tool in an MSBuild target
- [NSwag.AspNetCore](https://www.nuget.org/packages/NSwag.AspNetCore/) (.NETStandard 1.6 and .NET 4.5.1+): 
- [NSwag.AspNet.Owin](https://www.nuget.org/packages/NSwag.AspNet.Owin/) (.NET 4.5+): 
    - ASP.NET Core/OWIN middlewares for serving Swagger specifications and Swagger UI
- [NSwag.AspNet.WebApi](https://www.nuget.org/packages/NSwag.AspNet.WebApi/) (.NET 4.5+): 
    - ASP.NET Web API filter which serializes exceptions ([JsonExceptionFilterAttribute](https://github.com/NSwag/NSwag/wiki/JsonExceptionFilterAttribute))
- [NSwagStudio](https://chocolatey.org/packages/nswagstudio) (Chocolatey, Windows): 
    - Package to install the NSwagStudio and command line tools via Chocolatey
- [NSwag.Commands](https://www.nuget.org/packages/NSwag.Commands/) (PCL 259 / .NETStandard 1.0): 
    - Commands for the command line tool implementations and UI
- [NSwag.ConsoleCore](https://www.nuget.org/packages/NSwag.ConsoleCore/) (PCL 259 / .NETStandard 1.0): 
    - Command line tool for .NET Core (`dotnet nswag`)

The NuGet packages may require the **Microsoft.NETCore.Portable.Compatibility** package on .NET Core/UWP targets (if mscorlib is missing). 

![LayerDiagram](assets/LayerDiagram.png)

### Usage in C&#35;

The following code shows how to read a Swagger specification and generate C# client classes to call the described web services: 
	
```cs
var swaggerSettings = new WebApiToSwaggerGeneratorSettings();
var swaggerGenerator = new WebApiToSwaggerGenerator(swaggerSettings);

var document = swaggerGenerator.GenerateForController<PersonsController>();

var clientSettings = new SwaggerToCSharpClientGeneratorSettings 
{
    ClassName = "MyClass",
    Namespace = "MyNamespace"
};
var clientGenerator = new SwaggerToCSharpClientGenerator(document, clientSettings);

var code = clientGenerator.GenerateFile();
```

Check out the [project Wiki](https://github.com/NSwag/NSwag/wiki) for more information.

### NSwagStudio

The generators can be used in a comfortable and simple Windows GUI called [NSwagStudio](https://github.com/NSwag/NSwag/wiki/NSwagStudio): 

[![](https://raw.githubusercontent.com/NSwag/NSwag/master/assets/screenshots/03_WebAPI_CSharp.png)](https://raw.githubusercontent.com/NSwag/NSwag/master/assets/screenshots/03_WebAPI_CSharp.png)
