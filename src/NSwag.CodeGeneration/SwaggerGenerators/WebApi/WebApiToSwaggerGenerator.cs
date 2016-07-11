//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.CodeGeneration.Infrastructure;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Generates a <see cref="SwaggerService"/> object for the given Web API class type. </summary>
    public class WebApiToSwaggerGenerator
    {
        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerGenerator" /> class.</summary>
        /// <param name="settings">The settings.</param>
        public WebApiToSwaggerGenerator(WebApiToSwaggerGeneratorSettings settings)
        {
            Settings = settings;
        }

        /// <summary>Gets all controller class types of the given assembly.</summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The controller classes.</returns>
        public static IEnumerable<Type> GetControllerClasses(Assembly assembly)
        {
            // TODO: Move to IControllerClassLoader interface
            return assembly.ExportedTypes
                .Where(t => t.Name.EndsWith("Controller") ||
                            t.InheritsFrom("ApiController") ||
                            t.InheritsFrom("Controller")) // in ASP.NET Core, a Web API controller inherits from Controller
                .Where(t => t.GetTypeInfo().ImplementedInterfaces.All(i => i.FullName != "System.Web.Mvc.IController")); // no MVC controllers (legacy ASP.NET)
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public WebApiToSwaggerGeneratorSettings Settings { get; set; }

        /// <summary>Generates a Swagger specification for the given controller type.</summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="excludedMethodName">The name of the excluded method name.</param>
        /// <returns>The <see cref="SwaggerService" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public SwaggerService GenerateForController<TController>(string excludedMethodName = "Swagger")
        {
            return GenerateForController(typeof(TController), excludedMethodName);
        }

        /// <summary>Generates a Swagger specification for the given controller type.</summary>
        /// <param name="controllerType">The type of the controller.</param>
        /// <param name="excludedMethodName">The name of the excluded method name.</param>
        /// <returns>The <see cref="SwaggerService" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public SwaggerService GenerateForController(Type controllerType, string excludedMethodName = "Swagger")
        {
            var service = CreateService(Settings);
            var schemaResolver = new SchemaResolver();

            GenerateForController(service, controllerType, excludedMethodName, schemaResolver);

            service.GenerateOperationIds();
            return service;
        }

        /// <summary>Generates a Swagger specification for the given controller types.</summary>
        /// <param name="controllerTypes">The types of the controller.</param>
        /// <param name="excludedMethodName">The name of the excluded method name.</param>
        /// <returns>The <see cref="SwaggerService" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public SwaggerService GenerateForControllers(IEnumerable<Type> controllerTypes, string excludedMethodName = "Swagger")
        {
            var service = CreateService(Settings);
            var schemaResolver = new SchemaResolver();

            foreach (var controllerType in controllerTypes)
                GenerateForController(service, controllerType, excludedMethodName, schemaResolver);

            service.GenerateOperationIds();
            return service;
        }

        private static SwaggerService CreateService(WebApiToSwaggerGeneratorSettings settings)
        {
            return new SwaggerService
            {
                Consumes = new List<string> { "application/json" },
                Produces = new List<string> { "application/json" },
                Info = new SwaggerInfo
                {
                    Title = settings.Title,
                    Version = settings.Version
                }
            };
        }

        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        private void GenerateForController(SwaggerService service, Type controllerType, string excludedMethodName, SchemaResolver schemaResolver)
        {
            var hasIgnoreAttribute = controllerType.GetTypeInfo().GetCustomAttributes()
                .Any(a => a.GetType().Name == "SwaggerIgnoreAttribute");

            if (!hasIgnoreAttribute)
            {
                var operations = new List<Tuple<SwaggerOperationDescription, MethodInfo>>();
                foreach (var method in GetActionMethods(controllerType, excludedMethodName))
                {
                    var httpPaths = GetHttpPaths(controllerType, method);
                    var httpMethods = GetSupportedHttpMethods(method).ToList();

                    foreach (var httpPath in httpPaths)
                    {
                        foreach (var httpMethod in httpMethods)
                        {
                            var operation = new SwaggerOperation
                            {
                                IsDeprecated = method.GetCustomAttribute<ObsoleteAttribute>() != null
                            };

                            var parameters = method.GetParameters().ToList();

                            LoadParameters(service, operation, parameters, httpPath, schemaResolver);
                            LoadReturnType(service, operation, method, schemaResolver);
                            LoadMetaData(operation, method);
                            LoadOperationTags(method, operation, controllerType);

                            var operationDescription = new SwaggerOperationDescription
                            {
                                Path = Regex.Replace(httpPath, "{(.*?)(:(.*?))?}", match =>
                                {
                                    if (operation.Parameters.Any(p => p.Kind == SwaggerParameterKind.Path && match.Groups[1].Value == p.Name))
                                        return match.Value;
                                    return string.Empty;
                                }).TrimEnd('/'),
                                Method = httpMethod,
                                Operation = operation
                            };

                            operationDescription.Operation.OperationId = GetOperationId(service, controllerType.Name, method);
                            operations.Add(new Tuple<SwaggerOperationDescription, MethodInfo>(operationDescription, method));
                        }
                    }
                }

                AddOperationDescriptionsToDocument(service, operations, schemaResolver);
            }

            AppendRequiredSchemasToDefinitions(service, schemaResolver);
        }

        private void AddOperationDescriptionsToDocument(SwaggerService service, List<Tuple<SwaggerOperationDescription, MethodInfo>> operations, SchemaResolver schemaResolver)
        {
            var allOperation = operations.Select(t => t.Item1).ToList();
            foreach (var tuple in operations)
            {
                var operation = tuple.Item1;
                var method = tuple.Item2;

                var addOperation = RunOperationProcessors(method, operation, schemaResolver, allOperation);
                if (addOperation)
                {
                    if (!service.Paths.ContainsKey(operation.Path))
                        service.Paths[operation.Path] = new SwaggerOperations();

                    if (service.Paths[operation.Path].ContainsKey(operation.Method))
                        throw new InvalidOperationException("The method '" + operation.Method + "' on path '" + operation.Path + "' is registered multiple times.");

                    service.Paths[operation.Path][operation.Method] = operation.Operation;
                }
            }
        }

        private bool RunOperationProcessors(MethodInfo method, SwaggerOperationDescription operation, SchemaResolver schemaResolver, List<SwaggerOperationDescription> allOperations)
        {
            // 1. Run from settings
            foreach (var operationProcessor in Settings.OperationProcessors)
            {
                if (operationProcessor.Process(operation, method, schemaResolver, allOperations) == false)
                    return false;
            }

            // 2. Run from class attributes
            var operationProcessorAttribute = method.DeclaringType.GetTypeInfo().GetCustomAttributes()
            // 3. Run from method attributes
                .Concat(method.GetCustomAttributes())
                .Where(a => a.GetType().Name == "SwaggerOperationProcessorAttribute");

            foreach (dynamic attribute in operationProcessorAttribute)
            {
                var operationProcessor = Activator.CreateInstance(attribute.Type);
                if (operationProcessor.Process(method, operation, schemaResolver, allOperations) == false)
                    return false;
            }

            return true;
        }

        private void LoadOperationTags(MethodInfo method, SwaggerOperation operation, Type controllerType)
        {
            dynamic tagsAttribute = method.GetCustomAttributes().SingleOrDefault(a => a.GetType().Name == "SwaggerTagsAttribute");
            if (tagsAttribute != null)
                operation.Tags = ((string[])tagsAttribute.Tags).ToList();
            else
                operation.Tags.Add(controllerType.Name);
        }

        private void AppendRequiredSchemasToDefinitions(SwaggerService service, SchemaResolver schemaResolver)
        {
            foreach (var schema in schemaResolver.Schemes)
            {
                if (!service.Definitions.Values.Contains(schema))
                {
                    var typeName = schema.GetTypeName(Settings.TypeNameGenerator);

                    if (!service.Definitions.ContainsKey(typeName))
                        service.Definitions[typeName] = schema;
                    else
                        service.Definitions["ref_" + Guid.NewGuid().ToString().Replace("-", "_")] = schema;
                }
            }
        }

        private static IEnumerable<MethodInfo> GetActionMethods(Type controllerType, string excludedMethodName)
        {
            var methods = controllerType.GetRuntimeMethods().Where(m => m.IsPublic);
            return methods.Where(m =>
                m.Name != excludedMethodName &&
                m.IsSpecialName == false && // avoid property methods
                m.DeclaringType != null &&
                m.DeclaringType != typeof(object) &&
                m.GetCustomAttributes().All(a => a.GetType().Name != "SwaggerIgnoreAttribute") &&
                m.DeclaringType.FullName.StartsWith("Microsoft.AspNet") == false && // .NET Core (Web API & MVC)
                m.DeclaringType.FullName != "System.Web.Http.ApiController" &&
                m.DeclaringType.FullName != "System.Web.Mvc.Controller");
        }

        private string GetOperationId(SwaggerService service, string controllerName, MethodInfo method)
        {
            string operationId;

            dynamic swaggerOperationAttribute = method.GetCustomAttributes().FirstOrDefault(a => a.GetType().Name == "SwaggerOperationAttribute");
            if (swaggerOperationAttribute != null && !string.IsNullOrEmpty(swaggerOperationAttribute.OperationId))
                operationId = swaggerOperationAttribute.OperationId;
            else
            {
                if (controllerName.EndsWith("Controller"))
                    controllerName = controllerName.Substring(0, controllerName.Length - 10);

                var methodName = method.Name;
                if (methodName.EndsWith("Async"))
                    methodName = methodName.Substring(0, methodName.Length - 5);

                operationId = controllerName + "_" + methodName;
            }

            var number = 1;
            while (service.Operations.Any(o => o.Operation.OperationId == operationId + (number > 1 ? "_" + number : string.Empty)))
                number++;

            return operationId + (number > 1 ? number.ToString() : string.Empty);
        }

        private void LoadMetaData(SwaggerOperation operation, MethodInfo method)
        {
            dynamic descriptionAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "DescriptionAttribute");

            if (descriptionAttribute != null)
                operation.Summary = descriptionAttribute.Description;
            else
            {
                var summary = method.GetXmlDocumentation();
                if (summary != string.Empty)
                    operation.Summary = summary;
            }
        }

        private IEnumerable<string> GetHttpPaths(Type controllerType, MethodInfo method)
        {
            var httpPaths = new List<string>();

            var routeAttributes = method.GetCustomAttributes()
                .Where(a => a.GetType().Name == "RouteAttribute")
                .ToList();

            // .NET Core: Http*Attribute inherits from HttpMethodAttribute with Template property
            var httpMethodAttributes = method.GetCustomAttributes()
                .Where(a => a.GetType().InheritsFrom("HttpMethodAttribute"))
                .Where((dynamic a) => !string.IsNullOrEmpty(a.Template))
                .ToList();

            // .NET Core: RouteAttribute on class level
            dynamic routeAttributeOnClass = controllerType.GetTypeInfo().GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "RouteAttribute");

            if (routeAttributes.Any() || httpMethodAttributes.Any())
            {
                dynamic routePrefixAttribute = controllerType.GetTypeInfo().GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "RoutePrefixAttribute");

                foreach (dynamic attribute in routeAttributes.Concat(httpMethodAttributes))
                {
                    if (attribute.Template.StartsWith("~/")) // ignore route prefixes
                        httpPaths.Add(attribute.Template.Substring(1));
                    else if (routePrefixAttribute != null)
                        httpPaths.Add(routePrefixAttribute.Prefix + "/" + attribute.Template);
                    else if (routeAttributeOnClass != null)
                        httpPaths.Add(routeAttributeOnClass.Template + "/" + attribute.Template);
                    else
                        httpPaths.Add(attribute.Template);
                }
            }
            else if (routeAttributeOnClass != null)
                httpPaths.Add(routeAttributeOnClass.Template);
            else
            {
                var actionName = GetActionName(method);
                var httpPath = (Settings.DefaultUrlTemplate ?? string.Empty)
                    .Replace("{controller}", controllerType.Name.Replace("Controller", string.Empty))
                    .Replace("{action}", actionName);

                httpPaths.Add(httpPath);
            }

            foreach (var httpPath in httpPaths)
                yield return "/" + httpPath
                    .Replace("[", "{")
                    .Replace("]", "}")
                    .TrimStart('/');
        }

        private string GetActionName(MethodInfo method)
        {
            dynamic actionNameAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "ActionNameAttribute");

            if (actionNameAttribute != null)
                return actionNameAttribute.Name;

            return method.Name;
        }

        private IEnumerable<SwaggerOperationMethod> GetSupportedHttpMethods(MethodInfo method)
        {
            // See http://www.asp.net/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api

            var actionName = GetActionName(method);

            var httpMethods = GetSupportedHttpMethodsFromAttributes(method).ToArray();
            foreach (var httpMethod in httpMethods)
                yield return httpMethod;

            if (httpMethods.Length == 0)
            {
                if (actionName.StartsWith("Get"))
                    yield return SwaggerOperationMethod.Get;
                else if (actionName.StartsWith("Post"))
                    yield return SwaggerOperationMethod.Post;
                else if (actionName.StartsWith("Put"))
                    yield return SwaggerOperationMethod.Put;
                else if (actionName.StartsWith("Delete"))
                    yield return SwaggerOperationMethod.Delete;
                else
                    yield return SwaggerOperationMethod.Post;
            }
        }

        private IEnumerable<SwaggerOperationMethod> GetSupportedHttpMethodsFromAttributes(MethodInfo method)
        {
            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpGetAttribute"))
                yield return SwaggerOperationMethod.Get;

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpPostAttribute"))
                yield return SwaggerOperationMethod.Post;

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpPutAttribute"))
                yield return SwaggerOperationMethod.Put;

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpDeleteAttribute"))
                yield return SwaggerOperationMethod.Delete;

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpOptionsAttribute"))
                yield return SwaggerOperationMethod.Options;

            dynamic acceptVerbsAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "AcceptVerbsAttribute");

            if (acceptVerbsAttribute != null)
            {
                foreach (var verb in ((ICollection)acceptVerbsAttribute.HttpMethods).OfType<object>().Select(v => v.ToString().ToLowerInvariant()))
                {
                    if (verb == "get")
                        yield return SwaggerOperationMethod.Get;
                    else if (verb == "post")
                        yield return SwaggerOperationMethod.Post;
                    else if (verb == "put")
                        yield return SwaggerOperationMethod.Put;
                    else if (verb == "delete")
                        yield return SwaggerOperationMethod.Delete;
                    else if (verb == "options")
                        yield return SwaggerOperationMethod.Options;
                    else if (verb == "head")
                        yield return SwaggerOperationMethod.Head;
                    else if (verb == "patch")
                        yield return SwaggerOperationMethod.Patch;
                }
            }
        }

        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        private void LoadParameters(SwaggerService service, SwaggerOperation operation, List<ParameterInfo> parameters, string httpPath, ISchemaResolver schemaResolver)
        {
            foreach (var parameter in parameters)
            {
                var nameLower = parameter.Name.ToLowerInvariant();
                if (httpPath.ToLowerInvariant().Contains("{" + nameLower + "}") ||
                    httpPath.ToLowerInvariant().Contains("{" + nameLower + ":")) // path parameter
                {
                    var operationParameter = CreatePrimitiveParameter(service, parameter, schemaResolver);
                    operationParameter.Kind = SwaggerParameterKind.Path;
                    operationParameter.IsNullableRaw = false;
                    operationParameter.IsRequired = true; // Path is always required => property not needed

                    operation.Parameters.Add(operationParameter);
                }
                else
                {
                    var parameterInfo = JsonObjectTypeDescription.FromType(parameter.ParameterType, parameter.GetCustomAttributes(), Settings.DefaultEnumHandling);
                    if (TryAddFileParameter(parameterInfo, service, operation, schemaResolver, parameter) == false)
                    {
                        // http://blogs.msdn.com/b/jmstall/archive/2012/04/16/how-webapi-does-parameter-binding.aspx

                        dynamic fromBodyAttribute = parameter.GetCustomAttributes()
                            .SingleOrDefault(a => a.GetType().Name == "FromBodyAttribute");

                        dynamic fromUriAttribute = parameter.GetCustomAttributes()
                            .SingleOrDefault(a => a.GetType().Name == "FromUriAttribute");

                        // TODO: Add support for ModelBinder attribute

                        if (parameterInfo.IsComplexType)
                        {
                            if (fromUriAttribute != null)
                                AddPrimitiveParametersFromUri(service, operation, parameter, schemaResolver);
                            else
                                AddBodyParameter(service, operation, parameter, schemaResolver);
                        }
                        else
                        {
                            if (fromBodyAttribute != null)
                                AddBodyParameter(service, operation, parameter, schemaResolver);
                            else
                                AddPrimitiveParameter(service, operation, parameter, schemaResolver);
                        }
                    }
                }
            }

            if (operation.Parameters.Any(p => p.Type == JsonObjectType.File))
                operation.Consumes = new List<string> { "multipart/form-data" };

            if (operation.Parameters.Count(p => p.Kind == SwaggerParameterKind.Body) > 1)
                throw new InvalidOperationException("The operation '" + operation.OperationId + "' has more than one body parameter.");
        }

        private bool TryAddFileParameter(JsonObjectTypeDescription info, SwaggerService service, SwaggerOperation operation, ISchemaResolver schemaResolver, ParameterInfo parameter)
        {
            var isFileArray = IsFileArray(parameter.ParameterType, info);
            if (info.Type == JsonObjectType.File || isFileArray)
            {
                AddFileParameter(parameter, isFileArray, operation, service, schemaResolver);
                return true;
            }

            return false;
        }

        private void AddFileParameter(ParameterInfo parameter, bool isFileArray, SwaggerOperation operation, SwaggerService service, ISchemaResolver schemaResolver)
        {
            var attributes = parameter.GetCustomAttributes().ToList();
            var operationParameter = CreatePrimitiveParameter( // TODO: Check if there is a way to control the property name
                service, parameter.Name, parameter.GetXmlDocumentation(), parameter.ParameterType, attributes, schemaResolver);

            InitializeFileParameter(operationParameter, isFileArray);
            operation.Parameters.Add(operationParameter);
        }

        private bool IsFileArray(Type type, JsonObjectTypeDescription typeInfo)
        {
            var isFormFileCollection = type.Name == "IFormFileCollection";
            var isFileArray = typeInfo.Type == JsonObjectType.Array && type.GenericTypeArguments.Any() &&
                JsonObjectTypeDescription.FromType(type.GenericTypeArguments[0], null, Settings.DefaultEnumHandling).Type == JsonObjectType.File;
            return isFormFileCollection || isFileArray;
        }

        private void AddBodyParameter(SwaggerService service, SwaggerOperation operation, ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var operationParameter = CreateBodyParameter(service, parameter, schemaResolver);
            operation.Parameters.Add(operationParameter);
        }

        private void AddPrimitiveParametersFromUri(SwaggerService service, SwaggerOperation operation, ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            foreach (var property in parameter.ParameterType.GetRuntimeProperties())
            {
                var attributes = property.GetCustomAttributes().ToList();
                var operationParameter = CreatePrimitiveParameter(// TODO: Check if there is a way to control the property name
                    service, JsonPathUtilities.GetPropertyName(property, Settings.DefaultPropertyNameHandling),
                        property.GetXmlDocumentation(), property.PropertyType, attributes, schemaResolver);

                // TODO: Check if required can be controlled with mechanisms other than RequiredAttribute

                var parameterInfo = JsonObjectTypeDescription.FromType(property.PropertyType, attributes, Settings.DefaultEnumHandling);
                var isFileArray = IsFileArray(property.PropertyType, parameterInfo);
                if (parameterInfo.Type == JsonObjectType.File || isFileArray)
                    InitializeFileParameter(operationParameter, isFileArray);
                else
                    operationParameter.Kind = SwaggerParameterKind.Query;

                operation.Parameters.Add(operationParameter);
            }
        }

        private static void InitializeFileParameter(SwaggerParameter operationParameter, bool isFileArray)
        {
            operationParameter.Type = JsonObjectType.File;
            operationParameter.Kind = SwaggerParameterKind.FormData;

            if (isFileArray)
                operationParameter.CollectionFormat = SwaggerParameterCollectionFormat.Multi;
        }

        private void AddPrimitiveParameter(SwaggerService service, SwaggerOperation operation, ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var operationParameter = CreatePrimitiveParameter(service, parameter, schemaResolver);
            operationParameter.Kind = SwaggerParameterKind.Query;
            operationParameter.IsRequired = operationParameter.IsRequired || parameter.HasDefaultValue == false;
            operation.Parameters.Add(operationParameter);
        }

        private SwaggerParameter CreateBodyParameter(SwaggerService service, ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var isRequired = IsParameterRequired(parameter);

            var typeDescription = JsonObjectTypeDescription.FromType(parameter.ParameterType, parameter.GetCustomAttributes(), Settings.DefaultEnumHandling);
            var operationParameter = new SwaggerParameter
            {
                Name = parameter.Name,
                Kind = SwaggerParameterKind.Body,
                IsRequired = isRequired,
                IsNullableRaw = typeDescription.IsNullable,
                Schema = CreateAndAddSchema(service, parameter.ParameterType, !isRequired, parameter.GetCustomAttributes(), schemaResolver),
            };

            var description = parameter.GetXmlDocumentation();
            if (description != string.Empty)
                operationParameter.Description = description;

            return operationParameter;
        }

        private bool IsParameterRequired(ParameterInfo parameter)
        {
            if (parameter == null)
                return false;

            if (parameter.GetCustomAttributes().Any(a => a.GetType().Name == "RequiredAttribute"))
                return true;

            if (parameter.HasDefaultValue)
                return false;

            var isNullable = Nullable.GetUnderlyingType(parameter.ParameterType) != null;
            if (isNullable)
                return false;

            return parameter.ParameterType.GetTypeInfo().IsValueType;
        }

        private SwaggerParameter CreatePrimitiveParameter(SwaggerService service, ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            return CreatePrimitiveParameter(
                service, parameter.Name, parameter.GetXmlDocumentation(),
                parameter.ParameterType, parameter.GetCustomAttributes().ToList(), schemaResolver);
        }

        private SwaggerParameter CreatePrimitiveParameter(SwaggerService service, string name, string description,
            Type type, IList<Attribute> parentAttributes, ISchemaResolver schemaResolver)
        {
            var schemaDefinitionAppender = new SwaggerServiceSchemaDefinitionAppender(service, Settings.TypeNameGenerator);
            var schemaGenerator = new RootTypeJsonSchemaGenerator(service, schemaDefinitionAppender, Settings);

            var typeDescription = JsonObjectTypeDescription.FromType(type, parentAttributes, Settings.DefaultEnumHandling);
            var parameterType = typeDescription.IsComplexType ? typeof(string) : type; // complex types must be treated as string

            var operationParameter = new SwaggerParameter();
            typeDescription.ApplyType(operationParameter);

            if (parameterType.GetTypeInfo().IsEnum)
                operationParameter.SchemaReference = schemaGenerator.Generate<JsonSchema4>(parameterType, null, parentAttributes, schemaDefinitionAppender, schemaResolver);
            else
                schemaGenerator.ApplyPropertyAnnotations(operationParameter, type, parentAttributes, typeDescription);

            operationParameter.Name = name;
            operationParameter.IsRequired = parentAttributes?.Any(a => a.GetType().Name == "RequiredAttribute") ?? false;
            operationParameter.IsNullableRaw = typeDescription.IsNullable;

            if (description != string.Empty)
                operationParameter.Description = description;

            return operationParameter;
        }

        private void LoadReturnType(SwaggerService service, SwaggerOperation operation, MethodInfo method, ISchemaResolver schemaResolver)
        {
            var xmlDescription = method.ReturnParameter.GetXmlDocumentation();
            if (xmlDescription == string.Empty)
                xmlDescription = null;

            var responseTypeAttributes = method.GetCustomAttributes()
                .Where(a => a.GetType().Name == "ResponseTypeAttribute")
                .ToList();

            var producesResponseTypeAttributes = method.GetCustomAttributes()
                    .Where(a => a.GetType().Name == "ProducesResponseTypeAttribute")
                    .ToList();

            if (responseTypeAttributes.Any() || producesResponseTypeAttributes.Any())
            {
                foreach (var responseTypeAttribute in responseTypeAttributes)
                {
                    dynamic dynResultTypeAttribute = responseTypeAttribute;
                    var returnType = dynResultTypeAttribute.ResponseType;

                    var httpStatusCode = IsVoidResponse(returnType) ? "204" : "200";
                    if (responseTypeAttribute.GetType().GetRuntimeProperty("HttpStatusCode") != null)
                        httpStatusCode = dynResultTypeAttribute.HttpStatusCode;

                    var description = xmlDescription;
                    if (responseTypeAttribute.GetType().GetRuntimeProperty("Description") != null)
                    {
                        if (!string.IsNullOrEmpty(dynResultTypeAttribute.Description))
                            description = dynResultTypeAttribute.Description;
                    }

                    var typeDescription = JsonObjectTypeDescription.FromType(returnType, method.ReturnParameter?.GetCustomAttributes(), Settings.DefaultEnumHandling);
                    operation.Responses[httpStatusCode] = new SwaggerResponse
                    {
                        Description = description ?? string.Empty,
                        IsNullableRaw = typeDescription.IsNullable,
                        Schema = CreateAndAddSchema(service, returnType, typeDescription.IsNullable, null, schemaResolver)
                    };
                }

                foreach (dynamic producesResponseTypeAttribute in producesResponseTypeAttributes)
                {
                    var returnType = producesResponseTypeAttribute.Type;
                    var typeDescription = JsonObjectTypeDescription.FromType(returnType, method.ReturnParameter?.GetCustomAttributes(), Settings.DefaultEnumHandling);

                    var httpStatusCode = producesResponseTypeAttribute.StatusCode.ToString(CultureInfo.InvariantCulture);
                    operation.Responses[httpStatusCode] = new SwaggerResponse
                    {
                        Description = xmlDescription ?? string.Empty,
                        IsNullableRaw = typeDescription.IsNullable,
                        Schema = CreateAndAddSchema(service, returnType, typeDescription.IsNullable, null, schemaResolver)
                    };
                }
            }
            else
                LoadDefaultReturnType(service, operation, method, schemaResolver, xmlDescription);
        }

        private void LoadDefaultReturnType(SwaggerService service, SwaggerOperation operation, MethodInfo method, ISchemaResolver schemaResolver, string xmlDescription)
        {
            var returnType = method.ReturnType;
            if (returnType == typeof(Task))
                returnType = typeof(void);
            else if (returnType.Name == "Task`1")
                returnType = returnType.GenericTypeArguments[0];

            if (IsVoidResponse(returnType))
            {
                operation.Responses["204"] = new SwaggerResponse
                {
                    Description = xmlDescription ?? string.Empty,
                };
            }
            else
            {
                var typeDescription = JsonObjectTypeDescription.FromType(returnType,
                    method.ReturnParameter?.GetCustomAttributes(), Settings.DefaultEnumHandling);
                operation.Responses["200"] = new SwaggerResponse
                {
                    Description = xmlDescription ?? string.Empty,
                    IsNullableRaw = typeDescription.IsNullable,
                    Schema = CreateAndAddSchema(service, returnType, typeDescription.IsNullable, null, schemaResolver)
                };
            }
        }

        private bool IsVoidResponse(Type returnType)
        {
            return returnType == null ||
                   returnType.FullName == "System.Void";
        }

        private bool IsFileResponse(Type returnType)
        {
            return returnType.Name == "IActionResult" ||
                   returnType.Name == "IHttpActionResult" ||
                   returnType.Name == "HttpResponseMessage" ||
                   returnType.InheritsFrom("ActionResult") ||
                   returnType.InheritsFrom("HttpResponseMessage");
        }

        private JsonSchema4 CreateAndAddSchema(SwaggerService service, Type type, bool mayBeNull, IEnumerable<Attribute> parentAttributes, ISchemaResolver schemaResolver)
        {
            if (type.Name == "Task`1")
                type = type.GenericTypeArguments[0];

            if (type.Name == "JsonResult`1")
                type = type.GenericTypeArguments[0];

            if (IsFileResponse(type))
                return new JsonSchema4 { Type = JsonObjectType.File };

            var schemaDefinitionAppender = new SwaggerServiceSchemaDefinitionAppender(service, Settings.TypeNameGenerator);
            var typeDescription = JsonObjectTypeDescription.FromType(type, parentAttributes, Settings.DefaultEnumHandling);
            if (typeDescription.Type.HasFlag(JsonObjectType.Object) && !typeDescription.IsDictionary)
            {
                if (type == typeof(object))
                {
                    return new JsonSchema4
                    {
                        // IsNullable is directly set on SwaggerParameter or SwaggerResponse
                        Type = Settings.NullHandling == NullHandling.JsonSchema ? JsonObjectType.Object | JsonObjectType.Null : JsonObjectType.Object,
                        AllowAdditionalProperties = false
                    };
                }

                if (!schemaResolver.HasSchema(type, false))
                {
                    var schemaGenerator = new RootTypeJsonSchemaGenerator(service, schemaDefinitionAppender, Settings);
                    schemaGenerator.Generate(type, null, null, schemaDefinitionAppender, schemaResolver);
                }

                if (mayBeNull)
                {
                    if (Settings.NullHandling == NullHandling.JsonSchema)
                    {
                        var schema = new JsonSchema4();
                        schema.OneOf.Add(new JsonSchema4 { Type = JsonObjectType.Null });
                        schema.OneOf.Add(new JsonSchema4 { SchemaReference = schemaResolver.GetSchema(type, false) });
                        return schema;
                    }
                    else
                    {
                        // IsNullable is directly set on SwaggerParameter or SwaggerResponse
                        return new JsonSchema4 { SchemaReference = schemaResolver.GetSchema(type, false) };
                    }
                }
                else
                    return new JsonSchema4 { SchemaReference = schemaResolver.GetSchema(type, false) };
            }

            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                var itemType = type.GenericTypeArguments.Length == 0 ? type.GetElementType() : type.GenericTypeArguments[0];
                return new JsonSchema4
                {
                    // IsNullable is directly set on SwaggerParameter or SwaggerResponse
                    Type = Settings.NullHandling == NullHandling.JsonSchema ? JsonObjectType.Array | JsonObjectType.Null : JsonObjectType.Array,
                    Item = CreateAndAddSchema(service, itemType, false, null, schemaResolver)
                };
            }

            var generator = new RootTypeJsonSchemaGenerator(service, schemaDefinitionAppender, Settings);
            return generator.Generate(type, null, null, schemaDefinitionAppender, schemaResolver);
        }
    }
}