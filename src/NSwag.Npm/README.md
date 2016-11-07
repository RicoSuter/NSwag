NSwag is a Swagger 2.0 API (OpenAPI) toolchain for .NET, Web API, TypeScript (jQuery, AngularJS, Angular 2, Aurelia, KnockoutJS, and more) and other platforms, written in C#. The Swagger specification uses JSON and JSON Schema to describe a RESTful web API. The NSwag project provides tools to generate Swagger specifications from existing ASP.NET Web API controllers and client code from these Swagger specifications. 

**This NPM module requires .NET 4.6+ or .NET Core to be installed on your system!**

- [More information about NSwag](http://nswag.org)
- [More information about the available commands](https://github.com/NSwag/NSwag/wiki/CommandLine)

## Usage

### Global installation

Install the package globally: 

    npm install nswag -g

Show available commands: 

    nswag help

### Project installation
	
Install the package for the current project: 

    npm install nswag --save-dev
	
Show available commands: 

    "node_modules/.bin/nswag" help
	
## Development

Compile and copy the current NSwag console binaries into the NPM module directory `binaries` directory: 

    build/03_Npm_Build.bat

To run the NodeJS binary locally: 

    cd "src/NSwag.Npm"
    node "bin/nswag" help
	
Publish package: 

    build/04_Npm_Publish.bat