//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpClientCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NConsole;
using NSwag.CodeGeneration.CodeGenerators;
using NSwag.CodeGeneration.CodeGenerators.CSharp;

#pragma warning disable 1591

namespace NSwag.Commands
{
    [Command(Name = "swagger2csclient", Description = "Generates CSharp client code from a Swagger specification.")]
    public class SwaggerToCSharpClientCommand : SwaggerToCSharpCommand<SwaggerToCSharpClientGeneratorSettings>
    {
        public SwaggerToCSharpClientCommand() : base(new SwaggerToCSharpClientGeneratorSettings())
        {
        }

        [Argument(Name = "ClientBaseClass", IsRequired = false, Description = "The client base class (empty for no base class).")]
        public string ClientBaseClass
        {
            get { return Settings.ClientBaseClass; }
            set { Settings.ClientBaseClass = value; }
        }

        [Argument(Name = "ConfigurationClass", IsRequired = false, Description = "The configuration class. The setting ClientBaseClass must be set. (empty for no configuration class).")]
        public string ConfigurationClass
        {
            get { return Settings.ConfigurationClass; }
            set { Settings.ConfigurationClass = value; }
        }

        [Argument(Name = "ExceptionClass", IsRequired = false, Description = "The exception class (default 'SwaggerException', may use '{controller}' placeholder).")]
        public string ExceptionClass
        {
            get { return Settings.ExceptionClass; }
            set { Settings.ExceptionClass = value; }
        }

        [Argument(Name = "GenerateClientClasses", IsRequired = false, Description = "Specifies whether generate client classes.")]
        public bool GenerateClientClasses
        {
            get { return Settings.GenerateClientClasses; }
            set { Settings.GenerateClientClasses = value; }
        }

        [Argument(Name = "GenerateClientInterfaces", IsRequired = false, Description = "Specifies whether generate interfaces for the client classes.")]
        public bool GenerateClientInterfaces
        {
            get { return Settings.GenerateClientInterfaces; }
            set { Settings.GenerateClientInterfaces = value; }
        }

        [Argument(Name = "GenerateDtoTypes", IsRequired = false, Description = "Specifies whether to generate DTO classes.")]
        public bool GenerateDtoTypes
        {
            get { return Settings.GenerateDtoTypes; }
            set { Settings.GenerateDtoTypes = value; }
        }

        [Argument(Name = "UseHttpClientCreationMethod", IsRequired = false, Description = "Specifies whether to call CreateHttpClientAsync on the base class to create a new HttpClient.")]
        public bool UseHttpClientCreationMethod
        {
            get { return Settings.UseHttpClientCreationMethod; }
            set { Settings.UseHttpClientCreationMethod = value; }
        }
        
        [Argument(Name = "JsonConverters", IsRequired = false, Description = "Specifies the custom Json.NET converter types (optional, comma separated).")]
        public string[] JsonConverters
        {
            get { return Settings.CSharpGeneratorSettings.JsonConverters; }
            set { Settings.CSharpGeneratorSettings.JsonConverters = value; }
        }

        [Argument(Name = "UseHttpRequestMessageCreationMethod", IsRequired = false, 
                  Description = "Specifies whether to call CreateHttpRequestMessageAsync on the base class to create a new HttpRequestMethod.")]
        public bool UseHttpRequestMessageCreationMethod
        {
            get { return Settings.UseHttpRequestMessageCreationMethod; }
            set { Settings.UseHttpRequestMessageCreationMethod = value; }
        }

        [Argument(Name = "GenerateContractsOutput", IsRequired = false,
                  Description = "Specifies whether to generate contracts output (interface and models in a separate file set with the ContractsOutput parameter).")]
        public bool GenerateContractsOutput { get; set; }

        [Argument(Name = "ContractsNamespace", IsRequired = false, Description = "The contracts .NET namespace.")]
        public string ContractsNamespace { get; set; }

        [Argument(Name = "ContractsOutput", IsRequired = false, 
                  Description = "The contracts output file path (optional, if no path is set then a single file with the implementation and contracts is generated).")]
        public string ContractsOutputFilePath { get; set; }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var result = await RunAsync();
            foreach (var pair in result)
                TryWriteFileOutput(pair.Key, host, () => pair.Value);
            return result;
        }

        public async Task<Dictionary<string, string>> RunAsync()
        {
            return await Task.Run(() =>
            {
                var clientGenerator = new SwaggerToCSharpClientGenerator(InputSwaggerDocument, Settings);

                if (GenerateContractsOutput)
                {
                    var result = new Dictionary<string, string>();
                    GenerateContracts(result, clientGenerator);
                    GenerateImplementation(result, clientGenerator);
                    return result;
                }
                else
                {
                    return new Dictionary<string, string>
                    {
                        { OutputFilePath ?? "Full", clientGenerator.GenerateFile(ClientGeneratorOutputType.Full) }
                    };
                }
            });
        }

        private void GenerateImplementation(Dictionary<string, string> result, SwaggerToCSharpClientGenerator clientGenerator)
        {
            var savedAdditionalNamespaceUsages = Settings.AdditionalNamespaceUsages?.ToArray();
            Settings.AdditionalNamespaceUsages =
                Settings.AdditionalNamespaceUsages?.Concat(new[] { ContractsNamespace }).ToArray() ?? new[] { ContractsNamespace };
            result[OutputFilePath ?? "Implementation"] = clientGenerator.GenerateFile(ClientGeneratorOutputType.Implementation);
            Settings.AdditionalNamespaceUsages = savedAdditionalNamespaceUsages;
        }

        private void GenerateContracts(Dictionary<string, string> result, SwaggerToCSharpClientGenerator clientGenerator)
        {
            var savedNamespace = Settings.CSharpGeneratorSettings.Namespace;
            Settings.CSharpGeneratorSettings.Namespace = ContractsNamespace;
            result[ContractsOutputFilePath ?? "Contracts"] = clientGenerator.GenerateFile(ClientGeneratorOutputType.Contracts);
            Settings.CSharpGeneratorSettings.Namespace = savedNamespace;
        }
    }
}