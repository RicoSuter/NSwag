namespace NSwag.SwaggerGeneration.WebApi.Versioned.Processors
{
    using System;
    using System.Drawing.Text;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Remoting.Contexts;
    using System.Threading.Tasks;
    using System.Web.Http.Description;
    using Annotations;
    using NJsonSchema;
    using NJsonSchema.Infrastructure;
    using SwaggerGeneration.Processors;
    using SwaggerGeneration.Processors.Contexts;

    public class OperationParameterProcessor : IOperationProcessor
    {
        private readonly VersionedWebApiToSwaggerGeneratorSettings _settings;

        public OperationParameterProcessor( VersionedWebApiToSwaggerGeneratorSettings settings )
        {
            _settings = settings;
        }
        
        public async Task<bool> ProcessAsync( OperationProcessorContext operationProcessorContext )
        {
            if ( !( operationProcessorContext is VersionedOperationProcessorContext context) )
            {
                return false;
            }

            foreach ( var parameter in context.ApiDescription.ParameterDescriptions )
            {
                if ( parameter.Source == ApiParameterSource.FromUri )
                {
                    var operationParameter = await CreatePrimitiveParameterAsync( context, parameter ).ConfigureAwait( false );
                    operationParameter.Kind = SwaggerParameterKind.Path;
                }
                else if ( parameter.Source == ApiParameterSource.FromBody )
                {
                    if ( IsFileParameter( context, parameter ) )
                    {
                        //TODO: generate for file
                    }
                    else
                    {
                        await AddBodyParametersAsync( context, parameter ).ConfigureAwait( false );
                    }
                }
            }

            return true;
        }

        private async Task AddBodyParametersAsync( OperationProcessorContext context,
            ApiParameterDescription parameter )
        {
            var operation = context.OperationDescription.Operation;
            var parameterType = parameter.ParameterDescriptor.ParameterType;
            var attributes = parameterType.GetCustomAttributes().ToList();

            var typeDescription = _settings.ReflectionService.GetDescription( parameterType, attributes, _settings );
            
            var operationParameter = new SwaggerParameter
            {
                Name = parameter.Name,
                Kind = SwaggerParameterKind.Body,
                IsRequired = true,
                IsNullableRaw = typeDescription.IsNullable,
                Schema = await context.SchemaGenerator.GenerateWithReferenceAndNullabilityAsync<JsonSchema4>(parameterType, attributes, false, context.SchemaResolver ).ConfigureAwait( false )
            };
            
            operation.Parameters.Add(operationParameter);
        }

        private bool IsFileParameter( OperationProcessorContext context, ApiParameterDescription parameter )
        {
            if ( parameter.ParameterDescriptor.ParameterType.IsAssignableTo( "System.IO.Stream", TypeNameStyle.Name ))
            {
                return true;
            }

            if ( parameter.ParameterDescriptor.GetCustomAttributes<SwaggerFileAttribute>().Any() )
            {
                return true;
            }

            return false;
        }

        private async Task AddFileParametersAsync( OperationProcessorContext context,
            ApiParameterDescription parameter )
        {
            throw new NotImplementedException();
        }
        
        private async Task<SwaggerParameter> CreatePrimitiveParameterAsync(
            OperationProcessorContext context,
            ApiParameterDescription parameter)
        {
            var operationParameter = await context.SwaggerGenerator.CreatePrimitiveParameterAsync(
                parameter.Name,
                parameter.Documentation,
                parameter.ParameterDescriptor.ParameterType,
                parameter.ParameterDescriptor.GetCustomAttributes<Attribute>());

                operationParameter.Default = parameter.ParameterDescriptor.DefaultValue;
                operationParameter.IsRequired = !parameter.ParameterDescriptor.IsOptional;
            return operationParameter;
        }
        
    }
}