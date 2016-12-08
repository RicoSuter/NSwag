//-----------------------------------------------------------------------
// <copyright file="ClientGeneratorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.CSharp;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators
{
    /// <summary>The client generator base.</summary>
    public abstract class ClientGeneratorBase
    {
        /// <summary>Initializes a new instance of the <see cref="ClientGeneratorBase" /> class.</summary>
        /// <param name="resolver">The type resolver.</param>
        /// <param name="codeGeneratorSettings">The code generator settings.</param>
        protected ClientGeneratorBase(ITypeResolver resolver, CodeGeneratorSettingsBase codeGeneratorSettings)
        {
            Resolver = resolver;
            codeGeneratorSettings.NullHandling = NullHandling.Swagger; // Enforce Swagger null handling 
        }

        /// <summary>Generates the the whole file containing all needed types.</summary>
        /// <returns>The code</returns>
        public abstract string GenerateFile();

        /// <summary>Gets the type resolver.</summary>
        protected ITypeResolver Resolver { get; }

        internal abstract ClientGeneratorBaseSettings BaseSettings { get; }

        internal abstract string GenerateFile(string clientCode, IEnumerable<string> clientClasses, ClientGeneratorOutputType outputType);

        internal abstract string GenerateClientClass(string controllerName, string controllerClassName, IList<OperationModel> operations, ClientGeneratorOutputType outputType);

        internal abstract string GetType(JsonSchema4 schema, bool isNullable, string typeNameHint);

        internal abstract string GetExceptionType(SwaggerOperation operation);

        internal abstract string GetResultType(SwaggerOperation operation);

        internal virtual string GetParameterVariableName(SwaggerParameter parameter)
        {
            return ConversionUtilities.ConvertToLowerCamelCase(parameter.Name
                .Replace("-", "_")
                .Replace(".", "_")
                .Replace("$", string.Empty), true);
        }

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

        internal string GenerateFile(SwaggerDocument document, ClientGeneratorOutputType type)
        {
            var clientCode = string.Empty;
            var operations = GetOperations(document);
            var clientClasses = new List<string>();

            if (BaseSettings.OperationNameGenerator.SupportsMultipleClients)
            {
                foreach (var controllerOperations in operations.GroupBy(o => BaseSettings.OperationNameGenerator.GetClientName(document, o.Path, o.HttpMethod, o.Operation)))
                {
                    var controllerName = controllerOperations.Key;
                    var controllerClassName = GetClassName(controllerOperations.Key);
                    clientCode += GenerateClientClass(controllerName, controllerClassName, controllerOperations.ToList(), type) + "\n\n";
                    clientClasses.Add(controllerClassName);
                }
            }
            else
            {
                var controllerName = string.Empty;
                var controllerClassName = GetClassName(controllerName);
                clientCode = GenerateClientClass(controllerName, controllerClassName, operations, type);
                clientClasses.Add(controllerClassName);
            }

            return GenerateFile(clientCode, clientClasses, type)
                .Replace("\r", string.Empty)
                .Replace("\n\n\n\n", "\n\n")
                .Replace("\n\n\n", "\n\n");
        }

        internal List<OperationModel> GetOperations(SwaggerDocument document)
        {
            document.GenerateOperationIds();

            var operations = document.Paths
                .SelectMany(pair => pair.Value.Select(p => new { Path = pair.Key.Trim('/'), HttpMethod = p.Key, Operation = p.Value }))
                .Select(tuple =>
                {
                    var operation = tuple.Operation;
                    var exceptionSchema = (Resolver as SwaggerToCSharpTypeResolver)?.ExceptionSchema;
                    var responses = operation.Responses.Select(response => new ResponseModel(response, exceptionSchema, this)).ToList();

                    var defaultResponse = responses.SingleOrDefault(r => r.StatusCode == "default");
                    if (defaultResponse != null)
                        responses.Remove(defaultResponse);

                    return new OperationModel
                    {
                        Path = tuple.Path,
                        HttpMethod = tuple.HttpMethod,
                        Operation = tuple.Operation,
                        OperationName = BaseSettings.OperationNameGenerator.GetOperationName(document, tuple.Path, tuple.HttpMethod, tuple.Operation),

                        ResultType = GetResultType(operation),
                        HasResultType = HasResultType(operation),
                        ResultDescription = GetResultDescription(operation),

                        ExceptionType = GetExceptionType(operation),
                        HasFormParameters = operation.ActualParameters.Any(p => p.Kind == SwaggerParameterKind.FormData),
                        Responses = responses,
                        DefaultResponse = defaultResponse,
                        Parameters = operation.ActualParameters.Select(p => new ParameterModel(
                            ResolveParameterType(p), operation, p, p.Name, GetParameterVariableName(p), BaseSettings.CodeGeneratorSettings, this)).ToList(),
                    };
                }).ToList();
            return operations;
        }

        /// <summary>Resolves the type of the parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The parameter type name.</returns>
        protected virtual string ResolveParameterType(SwaggerParameter parameter)
        {
            var schema = parameter.ActualSchema;

            if (parameter.CollectionFormat == SwaggerParameterCollectionFormat.Multi && !schema.Type.HasFlag(JsonObjectType.Array))
                schema = new JsonSchema4 { Type = JsonObjectType.Array, Item = schema };

            var typeNameHint = ConversionUtilities.ConvertToUpperCamelCase(parameter.Name, true);
            return Resolver.Resolve(schema, parameter.IsRequired == false || parameter.IsNullable(BaseSettings.CodeGeneratorSettings.NullHandling), typeNameHint);
        }

        private string GetClassName(string controllerName)
        {
            return BaseSettings.ClassName.Replace("{controller}", ConversionUtilities.ConvertToUpperCamelCase(controllerName, false));
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