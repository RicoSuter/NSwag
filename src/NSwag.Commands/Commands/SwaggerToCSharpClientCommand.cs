using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using NConsole;
using NSwag.CodeGeneration.CodeGenerators;
using NSwag.CodeGeneration.CodeGenerators.CSharp;

namespace NSwag.Commands
{
    //[Display(Description = "Generates CSharp client code from a Swagger specification.")]
    public class SwaggerToCSharpClientCommand : SwaggerToCSharpCommand<SwaggerToCSharpClientGeneratorSettings>
    {
        public SwaggerToCSharpClientCommand() : base(new SwaggerToCSharpClientGeneratorSettings())
        {
        }

        [Display(Description = "The client base class (empty for no base class).")]
        [Argument(Name = "ClientBaseClass", IsRequired = false)]
        public string ClientBaseClass
        {
            get { return Settings.ClientBaseClass; }
            set { Settings.ClientBaseClass = value; }
        }

        [Display(Description = "The configuration class. The setting ClientBaseClass must be set. (empty for no configuration class).")]
        [Argument(Name = "ConfigurationClass", IsRequired = false)]
        public string ConfigurationClass
        {
            get { return Settings.ConfigurationClass; }
            set { Settings.ConfigurationClass = value; }
        }

        [Display(Description = "The exception class (default 'SwaggerException', may use '{controller}' placeholder).")]
        [Argument(Name = "ExceptionClass", IsRequired = false)]
        public string ExceptionClass
        {
            get { return Settings.ExceptionClass; }
            set { Settings.ExceptionClass = value; }
        }

        [Display(Description = "Specifies whether generate client classes.")]
        [Argument(Name = "GenerateClientClasses", IsRequired = false)]
        public bool GenerateClientClasses
        {
            get { return Settings.GenerateClientClasses; }
            set { Settings.GenerateClientClasses = value; }
        }

        [Display(Description = "Specifies whether generate interfaces for the client classes.")]
        [Argument(Name = "GenerateClientInterfaces", IsRequired = false)]
        public bool GenerateClientInterfaces
        {
            get { return Settings.GenerateClientInterfaces; }
            set { Settings.GenerateClientInterfaces = value; }
        }

        [Display(Description = "Specifies whether to generate DTO classes.")]
        [Argument(Name = "GenerateDtoTypes", IsRequired = false)]
        public bool GenerateDtoTypes
        {
            get { return Settings.GenerateDtoTypes; }
            set { Settings.GenerateDtoTypes = value; }
        }

        [Display(Description = "Specifies whether to call CreateHttpClientAsync on the base class to create a new HttpClient.")]
        [Argument(Name = "UseHttpClientCreationMethod", IsRequired = false)]
        public bool UseHttpClientCreationMethod
        {
            get { return Settings.UseHttpClientCreationMethod; }
            set { Settings.UseHttpClientCreationMethod = value; }
        }
        
        [Display(Description = "Specifies the custom Json.NET converter types (optional, comma separated).")]
        [Argument(Name = "JsonConverters", IsRequired = false)]
        public string[] JsonConverters
        {
            get { return Settings.CSharpGeneratorSettings.JsonConverters; }
            set { Settings.CSharpGeneratorSettings.JsonConverters = value; }
        }

        [Display(Description = "Specifies whether to call CreateHttpRequestMessageAsync on the base class to create a new HttpRequestMethod.")]
        [Argument(Name = "UseHttpRequestMessageCreationMethod", IsRequired = false)]
        public bool UseHttpRequestMessageCreationMethod
        {
            get { return Settings.UseHttpRequestMessageCreationMethod; }
            set { Settings.UseHttpRequestMessageCreationMethod = value; }
        }

        [Display(Description = "Specifies whether to generate contracts output (interface and models in a separate file set with the ContractsOutput parameter).")]
        [Argument(Name = "GenerateContractsOutput", IsRequired = false)]
        public bool GenerateContractsOutput { get; set; }

        [Display(Description = "The contracts .NET namespace.")]
        [Argument(Name = "ContractsNamespace", IsRequired = false)]
        public string ContractsNamespace { get; set; }

        [Display(Description = "The contracts output file path (optional, if no path is set then a single file with the implementation and contracts is generated).")]
        [Argument(Name = "ContractsOutput", IsRequired = false)]
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
                var clientGenerator = new SwaggerToCSharpClientGenerator(InputSwaggerService, Settings);

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