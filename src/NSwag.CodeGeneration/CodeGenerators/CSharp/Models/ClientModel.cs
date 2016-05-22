using System.Collections.Generic;
using System.Linq;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp.Models
{
    internal class ClientModel
    {
        public ClientModel(string controllerName, IEnumerable<OperationModel> operations, SwaggerService service, SwaggerToCSharpClientGeneratorSettings settings)
        {
            var hasClientBaseClass = !string.IsNullOrEmpty(settings.ClientBaseClass);

            Class = settings.ClassName.Replace("{controller}", ConversionUtilities.ConvertToUpperCamelCase(controllerName));
            BaseClass = settings.ClientBaseClass;

            HasBaseClass = hasClientBaseClass;
            HasBaseType = settings.GenerateClientInterfaces || hasClientBaseClass;

            UseHttpClientCreationMethod = settings.UseHttpClientCreationMethod;
            GenerateClientInterfaces = settings.GenerateClientInterfaces;
            BaseUrl = service.BaseUrl;

            HasOperations = operations.Any();
            Operations = operations;

            HasMissingHttpMethods = Operations.Any(o =>
                o.HttpMethod == SwaggerOperationMethod.Options ||
                o.HttpMethod == SwaggerOperationMethod.Head ||
                o.HttpMethod == SwaggerOperationMethod.Patch);
        }

        public string Class { get; }

        public string BaseClass { get; }

        public bool HasBaseClass { get; }

        public bool HasBaseType { get; }

        public bool UseHttpClientCreationMethod { get; }

        public bool GenerateClientInterfaces { get; }

        public string BaseUrl { get; }

        public bool HasOperations { get; }

        public bool HasMissingHttpMethods { get; }

        public IEnumerable<OperationModel> Operations { get; }
    }
}