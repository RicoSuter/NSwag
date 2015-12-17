using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.ClientGenerators.Models;
using NSwag.CodeGeneration.ClientGenerators.TypeScript;

namespace NSwag.CodeGeneration.ClientGenerators
{
    /// <summary>The client generator base.</summary>
    public abstract class ClientGeneratorBase : GeneratorBase
    {
        internal abstract ClientGeneratorBaseSettings BaseSettings { get; }

        internal abstract string RenderFile(string clientCode);

        internal abstract string RenderClientCode(string controllerName, IEnumerable<OperationModel> operations);

        internal abstract string GetType(JsonSchema4 schema, string typeNameHint);

        internal abstract string GetExceptionType(SwaggerOperation operation);

        internal abstract string GetResultType(SwaggerOperation operation);

        internal string GetResultDescription(SwaggerOperation operation)
        {
            var response = GetOkResponse(operation);
            if (response != null)
                return RemoveLineBreaks(response.Description);

            return null;
        }

        internal string GenerateFile<TGenerator>(SwaggerService service, TypeResolverBase<TGenerator> resolver)
            where TGenerator : TypeGeneratorBase
        {
            var operations = GetOperations(service, resolver);
            var clients = string.Empty;

            if (BaseSettings.OperationGenerationMode == OperationGenerationMode.MultipleClientsFromPathSegments)
            {
                foreach (var controllerOperations in operations.GroupBy(o => o.MvcControllerName))
                    clients += RenderClientCode(controllerOperations.Key, controllerOperations);
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
                    var pathSegments = tuple.Path.Split('/').Where(p => !p.Contains("{")).Reverse().ToArray();

                    var mvcControllerName = pathSegments.Length >= 2 ? pathSegments[1] : "Unknown";
                    var mvcActionName = pathSegments.Length >= 1 ? pathSegments[0] : "Unknown";

                    var operation = tuple.Operation;
                    var responses = operation.Responses.Select(r => new ResponseModel
                    {
                        StatusCode = r.Key,
                        IsSuccess = HttpUtilities.IsSuccessStatusCode(r.Key),
                        Type = GetType(r.Value.Schema, "Response"),
                        TypeIsDate = GetType(r.Value.Schema, "Response") == "Date"
                    }).ToList();

                    var defaultResponse = responses.SingleOrDefault(r => r.StatusCode == "default");
                    if (defaultResponse != null)
                        responses.Remove(defaultResponse);

                    return new OperationModel
                    {
                        Id = operation.OperationId,

                        Path = tuple.Path,

                        HttpMethodUpper = ConvertToUpperStartIdentifier(tuple.HttpMethod.ToString()),
                        HttpMethodLower = ConvertToLowerStartIdentifier(tuple.HttpMethod.ToString()),

                        IsGetOrDelete = tuple.HttpMethod == SwaggerOperationMethod.Get || tuple.HttpMethod == SwaggerOperationMethod.Delete,

                        Summary = RemoveLineBreaks(operation.Summary),

                        MvcActionName = mvcActionName,
                        MvcControllerName = mvcControllerName,

                        OperationNameLower =
                            ConvertToLowerStartIdentifier(BaseSettings.OperationGenerationMode == OperationGenerationMode.MultipleClientsFromPathSegments
                                ? mvcActionName
                                : operation.OperationId),
                        OperationNameUpper =
                            ConvertToUpperStartIdentifier(BaseSettings.OperationGenerationMode == OperationGenerationMode.MultipleClientsFromPathSegments
                                ? mvcActionName
                                : operation.OperationId),

                        ResultType = GetResultType(operation),
                        ResultDescription = GetResultDescription(operation),

                        ExceptionType = GetExceptionType(operation),

                        Responses = responses,
                        DefaultResponse = defaultResponse,

                        Parameters = operation.Parameters.Select(p =>
                        {
                            if (p.ActualSchema.Type == JsonObjectType.File)
                                p.ActualSchema.Type = JsonObjectType.String; // TODO: Implement File type handling

                            return new ParameterModel
                            {
                                Name = p.Name,
                                Type = resolver.Resolve(p.ActualSchema, p.IsRequired, p.Name),
                                IsLast = operation.Parameters.LastOrDefault() == p,
                                Description = RemoveLineBreaks(p.Description)
                            };
                        }).ToList(),

                        ContentParameter =
                            operation.Parameters.Where(p => p.Kind == SwaggerParameterKind.Body)
                                .Select(p => new ParameterModel { Name = p.Name })
                                .SingleOrDefault(),

                        PlaceholderParameters =
                            operation.Parameters.Where(p => p.Kind == SwaggerParameterKind.Path).Select(p => new ParameterModel
                            {
                                Name = p.Name,
                                IsDate = p.Format == JsonFormatStrings.DateTime,
                                Description = RemoveLineBreaks(p.Description)
                            }),

                        QueryParameters =
                            operation.Parameters.Where(p => p.Kind == SwaggerParameterKind.Query).Select(p => new ParameterModel
                            {
                                Name = p.Name,
                                IsDate = p.Format == JsonFormatStrings.DateTime,
                                Description = RemoveLineBreaks(p.Description)
                            }).ToList(),
                    };
                }).ToList();
            return operations;
        }

        internal SwaggerResponse GetOkResponse(SwaggerOperation operation)
        {
            if (operation.Responses.Any(r => r.Key == "200"))
                return operation.Responses.Single(r => r.Key == "200").Value;

            var response = operation.Responses.FirstOrDefault(r => HttpUtilities.IsSuccessStatusCode(r.Key)).Value;
            if (response == null)
                return operation.Responses.First().Value;

            return response;
        }
    }
}