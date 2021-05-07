NSwag is a Swagger 2.0 API (OpenAPI) toolchain for .NET, Web API, TypeScript (jQuery, AngularJS, Angular 2+, Aurelia, KnockoutJS, and more) and other platforms, written in C#. The Swagger specification uses JSON and JSON Schema to describe a RESTful web API. The NSwag project provides tools to generate Swagger specifications from existing ASP.NET Web API controllers and client code from these Swagger specifications.

**This NPM module requires .NET 4.6.1+ or .NET Core 1.0/1.1/2.0/2.1/2.2 to be installed on your system!**

- [More information about NSwag](http://nswag.org)
- [More information about the available commands](https://github.com/RicoSuter/NSwag/wiki/CommandLine)

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

## Change runtime

The full .NET Framework in x64 mode is preferred as execution environment. If you need to run the command line tool in x86 mode use

    nswag version /runtime:WinX86

Add the switch `/runtime:NetCore*` to the command to execute one of the .NET Core binaries (automatically detects whether .NET Core 1.0 or 1.1 is installed):

    nswag version /runtime:NetCore

To specify what .NET Core binaries to execute, either use (default)

    nswag version /runtime:NetCore21

or

    nswag version /runtime:NetCore31

or

    nswag version /runtime:Net50

## Development

Run the following command to compile and copy the current NSwag console binaries into the NPM module directory `binaries` directory:

    build/01_Npm_Build.bat

To run the NodeJS binary locally:

    cd "src/NSwag.Npm"
    node "bin/nswag" version

The JavaScript command line tool can be found here:

    src/NSwag.Npm/bin/nswag.js

To publish the package (login required):

    build/02_Npm_Publish.bat
