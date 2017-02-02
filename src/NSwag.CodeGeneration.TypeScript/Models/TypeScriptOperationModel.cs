using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.Models;

namespace NSwag.CodeGeneration.TypeScript.Models
{
    /// <summary>The TypeScript operation model.</summary>
    public class TypeScriptOperationModel : OperationModelBase<TypeScriptParameterModel, TypeScriptResponseModel>
    {
        private readonly ClientGeneratorBaseSettings _settings;
        private readonly SwaggerToTypeScriptClientGenerator _generator;
        private readonly SwaggerOperation _operation;

        /// <summary>Initializes a new instance of the <see cref="TypeScriptOperationModel" /> class.</summary>
        /// <param name="operation">The operation.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="resolver">The resolver.</param>
        public TypeScriptOperationModel(
            SwaggerOperation operation, 
            ClientGeneratorBaseSettings settings, 
            SwaggerToTypeScriptClientGenerator generator, 
            ITypeResolver resolver)
            : base(null, operation, resolver, generator, settings)
        {
            _operation = operation;
            _settings = settings;
            _generator = generator;

            // TODO: Duplicated code
            Parameters = _operation.ActualParameters.Select(parameter =>
                new TypeScriptParameterModel(ResolveParameterType(parameter), _operation, parameter, parameter.Name,
                    GetParameterVariableName(parameter, _operation.Parameters), (SwaggerToTypeScriptClientGeneratorSettings)_settings,
                    _generator, (TypeScriptTypeResolver)resolver))
                .ToList();
        }

        /// <summary>Gets or sets the type of the result.</summary>
        public override string ResultType => UnwrappedResultType;

        /// <summary>Gets or sets the type of the exception.</summary>
        public override string ExceptionType
        {
            get
            {
                if (_operation.Responses.Count(r => !HttpUtilities.IsSuccessStatusCode(r.Key)) == 0)
                    return "string";

                return string.Join(" | ", _operation.Responses
                    .Where(r => !HttpUtilities.IsSuccessStatusCode(r.Key) && r.Value.ActualResponseSchema != null)
                    .Select(r => _generator.GetTypeName(r.Value.ActualResponseSchema, r.Value.IsNullable(_settings.CodeGeneratorSettings.NullHandling), "Exception"))
                    .Concat(new[] { "string" }));
            }
        }

        /// <summary>Resolves the type of the parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The parameter type name.</returns>
        protected override string ResolveParameterType(SwaggerParameter parameter)
        {
            var schema = parameter.ActualSchema;
            if (schema.Type == JsonObjectType.File)
            {
                if (parameter.CollectionFormat == SwaggerParameterCollectionFormat.Multi && !schema.Type.HasFlag(JsonObjectType.Array))
                    return "FileParameter[]";

                return "FileParameter";
            }

            return base.ResolveParameterType(parameter);
        }

        /// <summary>Creates the response model.</summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="response">The response.</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        protected override TypeScriptResponseModel CreateResponseModel(string statusCode, SwaggerResponse response, JsonSchema4 exceptionSchema, IClientGenerator generator, ClientGeneratorBaseSettings settings)
        {
            return new TypeScriptResponseModel(statusCode, response, response == GetSuccessResponse(), exceptionSchema, generator, (TypeScriptGeneratorSettings) settings.CodeGeneratorSettings);
        }
    }
}
