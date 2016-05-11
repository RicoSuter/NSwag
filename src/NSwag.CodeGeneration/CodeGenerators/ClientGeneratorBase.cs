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
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators
{
    /// <summary>The client generator base.</summary>
    public abstract class ClientGeneratorBase : GeneratorBase
    {
        internal abstract CodeGeneratorBaseSettings BaseSettings { get; }

        internal abstract string RenderFile(string clientCode);

        internal abstract string RenderClientCode(string controllerName, IEnumerable<OperationModel> operations);

        internal abstract string GetType(JsonSchema4 schema, string typeNameHint);

        internal abstract string GetExceptionType(SwaggerOperation operation);

        internal abstract string GetResultType(SwaggerOperation operation);

        internal bool HasResultType(SwaggerOperation operation)
        {
            var response = GetSuccessResponse(operation);
            return response != null && response.Schema != null;
        }

        internal string GetResultDescription(SwaggerOperation operation)
        {
            var response = GetSuccessResponse(operation);
            if (response != null)
                return ConversionUtilities.TrimWhiteSpaces(response.Description);
            return null;
        }

        internal string GenerateFile<TGenerator>(SwaggerService service, TypeResolverBase<TGenerator> resolver)
            where TGenerator : TypeGeneratorBase
        {
            var operations = GetOperations(service, resolver);
            var clients = string.Empty;

            if (BaseSettings.OperationNameGenerator.SupportsMultipleClients)
            {
                foreach (var controllerOperations in operations.GroupBy(o => BaseSettings.OperationNameGenerator.GetClientName(service, o.Path, o.HttpMethod, o.Operation)))
                    clients += RenderClientCode(controllerOperations.Key, controllerOperations) + "\n\n";
            }
            else
                clients = RenderClientCode(string.Empty, operations);

            return RenderFile(clients)
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
                    var responses = operation.Responses.Select(r => new ResponseModel(this)
                    {
                        StatusCode = r.Key,
                        Schema = r.Value.Schema?.ActualSchema
                    }).ToList();

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
                        HasFormParameters = operation.Parameters.Any(p => p.Kind == SwaggerParameterKind.FormData),
                        Responses = responses,
                        DefaultResponse = defaultResponse,
                        Parameters = operation.Parameters.Select(p =>
                        {
                            if (p.ActualSchema.Type == JsonObjectType.File)
                                p.ActualSchema.Type = JsonObjectType.String; // TODO: Implement File type handling

                            return new ParameterModel
                            {
                                Schema = p.ActualSchema, 
                                Name = p.Name,
                                VariableNameLower = ConversionUtilities.ConvertToLowerCamelCase(p.Name.Replace("-", "_").Replace(".", "_")), 
                                Kind = p.Kind,
                                IsRequired = p.IsRequired, 
                                Type = resolver.Resolve(p.ActualSchema, p.Type.HasFlag(JsonObjectType.Null), p.Name),
                                IsLast = operation.Parameters.LastOrDefault() == p,
                                Description = ConversionUtilities.TrimWhiteSpaces(p.Description)
                            };
                        }).ToList(),                           
                    };
                }).ToList();
            return operations;
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