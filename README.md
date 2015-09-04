## JSDL (JSON Service Description Language) Toolchain

[![Build status](https://ci.appveyor.com/api/projects/status/lm4f3mbi3xfhktmf?svg=true)](https://ci.appveyor.com/project/rsuter/jsdl)

JSON Service Description Language (JSDL) is a JSON-based interface description language that is used for describing the functionality offered by a (JSON) HTTP web service. The project contains the JSDL specification and gives the tools to automatically generate JSDL definitions and service client code for various programming languages. Using these generators the developer can concentrate on developing business code rather than writing boiler plate code for calling web services and deserializing their results. 

JSDL uses JSON Schema to describe types and introduces some new structures to define JSON web services. The primary intention of this project is to automatically generate code from these JSDL files and integrate the whole process so that it can be used as easily as WCF web service references in .NET. 

- JSDL Generators: ASP.NET Web API
- Client Generators: TypeScript, CSharp

[Read more about the available JSDL and Client Generators](https://github.com/rsuter/Jsdl/wiki)

[**Downloads and Build Artifacts** (command line tool and JSDL Studio)](https://ci.appveyor.com/project/rsuter/jsdl/build/artifacts)

This project uses [NJsonSchema for .NET](http://njsonschema.org) for JSON Schema generation and [NConsole for .NET](https://github.com/NConsole/NConsole) for command line argument parsing. 
