//-----------------------------------------------------------------------
// <copyright file="ClientGeneratorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators
{
    /// <summary>The client generator base.</summary>
    public abstract class ClientGeneratorBase : GeneratorBase
    {
        internal abstract ClientGeneratorBaseSettings BaseSettings { get; }

        internal abstract string GenerateFile(string clientCode, IEnumerable<string> clientClasses, ClientGeneratorOutputType outputType);

        internal abstract string GenerateClientClass(string controllerName, IList<OperationModel> operations, ClientGeneratorOutputType outputType);

        internal abstract string GetType(JsonSchema4 schema, bool isNullable, string typeNameHint);

        internal abstract string GetExceptionType(SwaggerOperation operation);

        internal abstract string GetResultType(SwaggerOperation operation);

        internal bool HasResultType(SwaggerOperation operation)
        {
            var response = GetSuccessResponse(operation);
            return response?.Schema != null;
        }

        internal string GetResultDescription(SwaggerOperation operation)
        {
            var response = GetSuccessResponse(operation);
            if (response != null)
                return ConversionUtilities.TrimWhiteSpaces(response.Description);
            return null;
        }

        internal string GenerateFile<TGenerator>(SwaggerService service, TypeResolverBase<TGenerator> resolver, ClientGeneratorOutputType type)
            where TGenerator : TypeGeneratorBase
        {
            var clientCode = string.Empty;
            var operations = GetOperations(service, resolver);
            var clientClasses = new List<string>();

            if (BaseSettings.OperationNameGenerator.SupportsMultipleClients)
            {
                foreach (var controllerOperations in operations.GroupBy(o => BaseSettings.OperationNameGenerator.GetClientName(service, o.Path, o.HttpMethod, o.Operation)))
                {
                    var controllerName = GetClassName(controllerOperations.Key);
                    clientCode += GenerateClientClass(controllerName, controllerOperations.ToList(), type) + "\n\n";
                    clientClasses.Add(controllerName);
                }
            }
            else
            {
                var controllerName = GetClassName(string.Empty);
                clientCode = GenerateClientClass(controllerName, operations, type);
                clientClasses.Add(controllerName);
            }

            return GenerateFile(clientCode, clientClasses, type)
                .Replace("\r", string.Empty)
                .Replace("\n\n\n\n", "\n\n")
                .Replace("\n\n\n", "\n\n");
        }

        internal List<OperationModel> GetOperations<TGenerator>(SwaggerService service, TypeResolverBase<TGenerator> resolver)
            where TGenerator : TypeGeneratorBase
        {
            service.GenerateOperationIds();

            var operations = service.Paths
                .SelectMany(pair => pair.Value.Select(p => new { Path = pair.Key.Trim('/'), HttpMethod = p.Key, Operation = p.Value }))
                .Select(tuple =>
                {
                    var operation = tuple.Operation;
                    var responses = operation.Responses.Select(response => new ResponseModel(response, this)).ToList();

                    var defaultResponse = responses.SingleOrDefault(r => r.StatusCode == "default");
                    if (defaultResponse != null)
                        responses.Remove(defaultResponse);

                    return new OperationModel
                    {
                        Path = tuple.Path,
                        HttpMethod = tuple.HttpMethod,
                        Operation = tuple.Operation,
                        OperationName = BaseSettings.OperationNameGenerator.GetOperationName(service, tuple.Path, tuple.HttpMethod, tuple.Operation),

                        ResultType = GetResultType(operation),
                        HasResultType = HasResultType(operation),
                        ResultDescription = GetResultDescription(operation),

                        ExceptionType = GetExceptionType(operation),
                        HasFormParameters = operation.ActualParameters.Any(p => p.Kind == SwaggerParameterKind.FormData),
                        Responses = responses,
                        DefaultResponse = defaultResponse,
                        Parameters = operation.ActualParameters.Select(p => new ParameterModel(
                            ResolveParameterType(p, resolver), operation, p, BaseSettings.CodeGeneratorSettings)).ToList(),
                    };
                }).ToList();
            return operations;
        }

        /// <summary>Resolves the type of the parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>The parameter type name.</returns>
        protected virtual string ResolveParameterType(SwaggerParameter parameter, ITypeResolver resolver)
        {
            var schema = parameter.ActualSchema;

            if (parameter.CollectionFormat == SwaggerParameterCollectionFormat.Multi && !schema.Type.HasFlag(JsonObjectType.Array))
                schema = new JsonSchema4 { Type = JsonObjectType.Array, Item = schema };

            var typeNameHint = ConversionUtilities.ConvertToUpperCamelCase(parameter.Name, true);
            return resolver.Resolve(schema, parameter.IsRequired == false || parameter.IsNullable(BaseSettings.CodeGeneratorSettings.NullHandling), typeNameHint);
        }

        private string GetClassName(string operationName)
        {
            return BaseSettings.ClassName.Replace("{controller}", ConversionUtilities.ConvertToUpperCamelCase(operationName, false));
        }

        internal SwaggerResponse GetSuccessResponse(SwaggerOperation operation)
        {
            if (operation.Responses.Any(r => r.Key == "200"))
                return operation.Responses.Single(r => r.Key == "200").Value;

            var response = operation.Responses.FirstOrDefault(r => HttpUtilities.IsSuccessStatusCode(r.Key)).Value;
            if (response != null)
                return response;

            return operation.Responses.FirstOrDefault(r => r.Key == "default").Value;
        }
    }
}