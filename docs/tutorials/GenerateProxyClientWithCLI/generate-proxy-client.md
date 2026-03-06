# A How-To: Generating the Service Client Proxy code

## The Sample Problem

You've recently joined a large project, with a distributed team, collaborating on building a Client Application - which could be a Mobile, Desktop, or Web App - and the application being built needs to integrate with a 3rd-Party Service that you don't have much visibility into, or little ability to change.

The 3rd-Party Service does have well defined contracts, deployed to integration-test endpoints, and the Specs are shared with you via an [OpenAPI Spec](https://swagger.io/specification/).

You are asked to find a repeatable approach to generate interfaces and DTOs from the spec, based on the above constraints.

Also, since you've come to this repo, we'll assume you want to use NSwag as part of your solution.

## Prerequisites

- `NSwag` - This process we'll cover requires the `NSwag` commandline tool to help automate generation of a service client, an interface definition and DTOs. Follow [install instructions](https://github.com/RicoSuter/NSwag/wiki/CommandLine) to install the command line version of nswag.
- [OpenAPI Swagger Editor VS Code Extension](https://marketplace.visualstudio.com/items?itemName=42Crunch.vscode-openapi) *(optional)* - This Visual Studio Code (VS Code) extension adds rich support for the OpenAPI Specification (OAS) (formerly known as Swagger Specification) in JSON or YAML format. The features include, for example, SwaggerUI and ReDoc preview, IntelliSense, linting, schema enforcement, code navigation, definition links, snippets, static security analysis, and more!
- If in later steps you choose to download the 3rd-Party Service's Open API Spec, this plugin makes it easy visualize

**Notes**:  

- You may need to specify runtime version `nswag version /runtime:Net50` to run nswag on your local machine, since the sample [nswag config](https://github.com/RicoSuter/NSwag/wiki/NSwag-Configuration-Document) we'll use specifies `runtime` as `Net50`.
- If you chose to download NSwag as a ZIP Archive, you may see dotnet version errors when trying to execute commands. If you are not able to resolve the issues, you may opt to install via Chocolatey or the MSI from the install instructions page provided above.
- For this sample, we'll use the public [Swagger Petstore](https://petstore.swagger.io/) as the sample 3rd-Party Service, later referred to as `[YourRemoteService]`.

## Creating Client Interface, DTOs and Proxy classes

### **OPTION 1**: The sample process for (re)creating the service client, interface definition and DTOs, using an existing nswag configuration file, is as follows

- Use an existing [nswag config document](https://github.com/RicoSuter/NSwag/wiki/NSwag-Configuration-Document), similar to [sample.nswag](./sample.nswag) from this sample folder.
- In your App codebase, check for folders similar to `MainApp > Services > [YourRemoteService]`, and `MainApp > Contracts > [YourRemoteService]`, if they are missing, create them
- If `[YourRemoteService]` has a public unauthenticated Open API Spec endpoint, one that gets appropriately versioned before changes are published, you can use it directly.
- Otherwise, I recommend downloading the desired version (typically current version) of your [Service Swagger](https://petstore.swagger.io/v2/swagger.json) from the Cloud and place the file in the same directory as the [sample.nswag](./sample.nswag).
  - If you downloaded a copy of the OpenAPI-Spec, you may want to include it along side with the nswag generated files, for reference and ad-hoc version tracking.
- Generate the service client, interface definitions and DTOs by running the `nswag` CLI:

```
nswag run sample.nswag /runtime:Net50
```

- Take the output from the above command, an update files, if needed, in your `MainApp > Services  > [YourRemoteService]`, and `MainApp > Contracts > [YourRemoteService]` folders
- Check if any updates are needed to service instances (or mock instances) that you may have registered with a Dependency Injection container used in your App, if any.

### **OPTION 2**: To customize the outputs, follow these additional steps

- Create your own `sample.nswag` configuration based on the starter sample below.
- Check for folders similar to `MainApp > Services  > [YourRemoteService]`, and `MainApp > Contracts > [YourRemoteService]`, if missing, create them
- Update the location of the OpenAPI spec to the `documentGenerator.fromDocument.url` parameter in the `nswag` config file. The parameter can point to the local Yaml or Json file you downloaded, or an http address.
- Update the class name `codeGenerators.openApiToCSharpClient.className` parameter in the `nswag` config file, to something other than *`SampleService`*
- Update the namespace `codeGenerators.openApiToCSharpClient.namespace` parameter in the `nswag` config file, to something other than *`MainApp.Services.SampleService`*
- Update the location of the generated C# code file `codeGenerators.openApiToCSharpClient.output` parameter in the `nswag` config file.
- If you want the interface definition and DTOs as a separate file, ensure `codeGenerators.openApiToCSharpClient.generateContractsOutput` is set to **`true`**, and
  - Update the contractsNamespace `codeGenerators.openApiToCSharpClient.contractsNamespace` parameter in the `nswag` config file, to something other than *`MainApp.Services.SampleService.Contracts`*
  - Update the location of the generated C# contracts code file `codeGenerators.openApiToCSharpClient.contractsOutputFilePath` parameter in the `nswag` config file.
- If you do not use a BaseClass beyond the generated interface, then set `codeGenerators.openApiToCSharpClient.clientBaseClass` to **`null`**, and `codeGenerators.openApiToCSharpClient.useHttpRequestMessageCreationMethod` to **`false`**
- Generate the service client, interface definitions and DTOs by running the `nswag` CLI:

```
nswag run sample.nswag /runtime:Net50
```

- Update files, if needed, in your `MainApp > Services  > [YourRemoteService]`, and `MainApp > Contracts > [YourRemoteService]` folders
- Check if any updates are needed to service instances (or mock instances) registered with a Dependency Injection container in your App, if any.

## Sample NSwag config

```
{
    "runtime": "Net50",
    "documentGenerator": {
        "fromDocument": {
            "json": "",
            "url": "YOUR_OPENAPI_SPEC_LOCATION_HERE",
            "output": null,
            "newLineBehavior": "Auto"
        }
    },    
    "codeGenerators": {
        "openApiToCSharpClient": {
            "generateClientClasses": true,
            "suppressClientClassesOutput": false,
            "generateClientInterfaces": true,
            "suppressClientInterfacesOutput": false,
            "generateDtoTypes": true,
            "injectHttpClient": true,
            "disposeHttpClient": true,
            "generateExceptionClasses": true,
            "exceptionClass": "ServiceException",
            "wrapDtoExceptions": false,
            "useHttpClientCreationMethod": false,
            "httpClientType": "System.Net.Http.HttpClient",
            "useHttpRequestMessageCreationMethod": true,
            "useBaseUrl": true,
            "generateBaseUrlProperty": true,
            "generateSyncMethods": false,
            "exposeJsonSerializerSettings": false,
            "clientClassAccessModifier": "public",
            "clientBaseClass": "MainApp.Services.BaseService",
            "typeAccessModifier": "public",
            "generateContractsOutput": true,
            "contractsNamespace": "MainApp.Services.SampleService.Contracts",
            "contractsOutputFilePath": "GENERATEDCONTRACTS.cs",
            "parameterDateTimeFormat": "s",
            "generateUpdateJsonSerializerSettingsMethod": true,
            "serializeTypeInformation": false,
            "queryNullValue": "",
            "className": "SampleService",
            "operationGenerationMode": "MultipleClientsFromOperationId",
            "includedOperationIds": [ "SampleOperationId" ],
            "excludedOperationIds": [],
            "excludeDeprecated": false,
            "generateOptionalParameters": false,
            "generateJsonMethods": true,
            "parameterArrayType": "System.Collections.Generic.IEnumerable",
            "parameterDictionaryType": "System.Collections.Generic.IDictionary",
            "responseArrayType": "System.Collections.ObjectModel.ObservableCollection",
            "responseDictionaryType": "System.Collections.Generic.Dictionary",
            "wrapResponses": false,
            "generateResponseClasses": true,
            "responseClass": "SwaggerResponse",
            "namespace": "MainApp.Services.SampleService",
            "requiredPropertiesMustBeDefined": true,
            "dateType": "System.DateTime",
            "dateTimeType": "System.DateTime",
            "timeType": "System.TimeSpan",
            "timeSpanType": "System.TimeSpan",
            "arrayType": "System.Collections.ObjectModel.ObservableCollection",
            "arrayInstanceType": "System.Collections.ObjectModel.ObservableCollection",            
            "dictionaryType": "System.Collections.Generic.Dictionary",
            "arrayBaseType": "System.Collections.ObjectModel.ObservableCollection",
            "dictionaryBaseType": "System.Collections.Generic.Dictionary",
            "classStyle": "poco",
            "generateDefaultValues": true,
            "generateDataAnnotations": false,
            "excludedTypeNames": [],
            "handleReferences": false,
            "generateImmutableArrayProperties": false,
            "generateImmutableDictionaryProperties": false,
            "output": "GENERATEDCODE.cs"
        }
    }
}
```
