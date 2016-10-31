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
using System.Threading;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.Infrastructure;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Generates a <see cref="SwaggerService"/> object for the given Web API class type. </summary>
    public class WebApiToSwaggerGenerator
    {
        private readonly SwaggerJsonSchemaGenerator _schemaGenerator;

        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerGenerator" /> class.</summary>
        /// <param name="settings">The settings.</param>
        public WebApiToSwaggerGenerator(WebApiToSwaggerGeneratorSettings settings)
            : this(settings, new SwaggerJsonSchemaGenerator(settings))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerGenerator" /> class.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        public WebApiToSwaggerGenerator(WebApiToSwaggerGeneratorSettings settings, SwaggerJsonSchemaGenerator schemaGenerator)
        {
            Settings = settings;
            _schemaGenerator = schemaGenerator;
        }

        /// <summary>Gets all controller class types of the given assembly.</summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The controller classes.</returns>
        public static IEnumerable<Type> GetControllerClasses(Assembly assembly)
        {
            // TODO: Move to IControllerClassLoader interface
            return assembly.ExportedTypes
                .Where(t => t.GetTypeInfo().IsAbstract == false)
                .Where(t => t.Name.EndsWith("Controller") ||
                            t.InheritsFrom("ApiController", TypeNameStyle.Name) ||
                            t.InheritsFrom("Controller", TypeNameStyle.Name)) // in ASP.NET Core, a Web API controller inherits from Controller
                .Where(t => t.GetTypeInfo().ImplementedInterfaces.All(i => i.FullName != "System.Web.Mvc.IController")); // no MVC controllers (legacy ASP.NET)
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public WebApiToSwaggerGeneratorSettings Settings { get; set; }

        /// <summary>Generates a Swagger specification for the given controller type.</summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <returns>The <see cref="SwaggerService" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public SwaggerService GenerateForController<TController>()
        {
            return GenerateForControllers(new[] { typeof(TController) });
        }

        /// <summary>Generates a Swagger specification for the given controller type.</summary>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>The <see cref="SwaggerService" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public SwaggerService GenerateForController(Type controllerType)
        {
            return GenerateForControllers(new[] { controllerType });
        }

        /// <summary>Generates a Swagger specification for the given controller types.</summary>
        /// <param name="controllerTypes">The types of the controller.</param>
        /// <returns>The <see cref="SwaggerService" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public SwaggerService GenerateForControllers(IEnumerable<Type> controllerTypes)
        {
            var service = CreateDocument(Settings);

            var schemaResolver = new SchemaResolver();
            var schemaDefinitionAppender = new SwaggerServiceSchemaDefinitionAppender(service, Settings.TypeNameGenerator);

            foreach (var controllerType in controllerTypes)
                GenerateForController(service, controllerType, schemaResolver, new SwaggerGenerator(_schemaGenerator, Settings, schemaResolver, schemaDefinitionAppender));

            service.GenerateOperationIds();

            foreach (var processor in Settings.DocumentProcessors)
                processor.Process(service);

            return service;
        }

        private static SwaggerService CreateDocument(WebApiToSwaggerGeneratorSettings settings)
        {
            var service = !string.IsNullOrEmpty(settings.DocumentTemplate) ? SwaggerService.FromJson(settings.DocumentTemplate) : new SwaggerService();

            service.Consumes = new List<string> { "application/json" };
            service.Produces = new List<string> { "application/json" };
            service.Info = new SwaggerInfo
            {
                Title = settings.Title,
                Description = settings.Description,
                Version = settings.Version
            };

            return service;
        }

        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        private void GenerateForController(SwaggerService service, Type controllerType, ISchemaResolver schemaResolver, SwaggerGenerator swaggerGenerator)
        {
            var hasIgnoreAttribute = controllerType.GetTypeInfo().GetCustomAttributes()
                .Any(a => a.GetType().Name == "SwaggerIgnoreAttribute");

            if (!hasIgnoreAttribute)
            {
                var operations = new List<Tuple<SwaggerOperationDescription, MethodInfo>>();
                foreach (var method in GetActionMethods(controllerType))
                {
                    var httpPaths = GetHttpPaths(controllerType, method).ToList();
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

                            LoadParameters(operation, parameters, httpPath, swaggerGenerator);
                            LoadReturnType(operation, method, swaggerGenerator);
                            LoadMetaData(operation, method);
                            LoadOperationTags(method, operation, controllerType);

                            var operationDescription = new SwaggerOperationDescription
                            {
                                Path = Regex.Replace(httpPath, "{(.*?)(:(.*?))?}", match =>
                                {
                                    var parameterName = match.Groups[1].Value.TrimEnd('?');
                                    if (operation.ActualParameters.Any(p => p.Kind == SwaggerParameterKind.Path && p.Name == parameterName))
                                        return "{" + parameterName + "}";
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

        private void AddOperationDescriptionsToDocument(SwaggerService service, List<Tuple<SwaggerOperationDescription, MethodInfo>> operations, ISchemaResolver schemaResolver)
        {
            var allOperation = operations.Select(t => t.Item1).ToList();
            foreach (var tuple in operations)
            {
                var operation = tuple.Item1;
                var method = tuple.Item2;

                var addOperation = RunOperationProcessors(method, operation, allOperation, schemaResolver);
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

        private bool RunOperationProcessors(MethodInfo method, SwaggerOperationDescription operation, List<SwaggerOperationDescription> allOperations, ISchemaResolver schemaResolver)
        {
            // 1. Run from settings
            foreach (var operationProcessor in Settings.OperationProcessors)
            {
                if (operationProcessor.Process(operation, method, schemaResolver, allOperations) == false)
                    return false;
            }

            // 2. Run from class attributes
            var operationProcessorAttribute = method.DeclaringType.GetTypeInfo()
                .GetCustomAttributes()
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

        private void AppendRequiredSchemasToDefinitions(SwaggerService service, ISchemaResolver schemaResolver)
        {
            foreach (var schema in schemaResolver.Schemas)
            {
                if (!service.Definitions.Values.Contains(schema))
                {
                    var typeName = schema.GetTypeName(Settings.TypeNameGenerator, string.Empty);

                    if (!service.Definitions.ContainsKey(typeName))
                        service.Definitions[typeName] = schema;
                    else
                        service.Definitions["ref_" + Guid.NewGuid().ToString().Replace("-", "_")] = schema;
                }
            }
        }

        private static IEnumerable<MethodInfo> GetActionMethods(Type controllerType)
        {
            var methods = controllerType.GetRuntimeMethods().Where(m => m.IsPublic);
            return methods.Where(m =>
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
            var controllerName = controllerType.Name.Replace("Controller", string.Empty);

            var routeAttributes = GetRouteAttributes(method.GetCustomAttributes()).ToList();

            // .NET Core: RouteAttribute on class level
            dynamic routeAttributeOnClass = GetRouteAttributes(controllerType.GetTypeInfo().GetCustomAttributes()).SingleOrDefault();
            dynamic routePrefixAttribute = GetRoutePrefixAttributes(controllerType.GetTypeInfo().GetCustomAttributes()).SingleOrDefault();

            if (routeAttributes.Any())
            {
                foreach (dynamic attribute in routeAttributes)
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
            // TODO: Check if this is correct
            else if (routePrefixAttribute != null && ((method.GetParameters().Length == 0 && method.Name == "Get") ||
                                                      method.Name == "Post" ||
                                                      method.Name == "Put" ||
                                                      method.Name == "Delete"))
            {
                httpPaths.Add(routePrefixAttribute.Prefix);
            }
            else if (routeAttributeOnClass != null)
                httpPaths.Add(routeAttributeOnClass.Template);
            else
                httpPaths.Add(Settings.DefaultUrlTemplate ?? string.Empty);

            var actionName = GetActionName(method);
            return httpPaths
                .Select(p =>
                    "/" + p
                        .Replace("[", "{")
                        .Replace("]", "}")
                        .Replace("{controller}", controllerName)
                        .Replace("{action}", actionName)
                        .Trim('/'))
                .SelectMany(p => ExpandOptionalHttpPathParameters(p, method))
                .Distinct()
                .ToList();
        }

        private IEnumerable<string> ExpandOptionalHttpPathParameters(string path, MethodInfo method)
        {
            var segments = path.Split('/');
            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                if (segment.EndsWith("?}"))
                {
                    foreach (var p in ExpandOptionalHttpPathParameters(string.Join("/", segments.Take(i).Concat(segments.Skip(i + 1))), method))
                        yield return p;

                    // Only expand if optional parameter is available in action method
                    if (method.GetParameters().Any(p => segment.StartsWith("{" + p.Name + ":") || segment.StartsWith("{" + p.Name + "?")))
                    {
                        foreach (var p in ExpandOptionalHttpPathParameters(string.Join("/", segments.Take(i).Concat(new[] { segment.Replace("?", "") }).Concat(segments.Skip(i + 1))), method))
                            yield return p;
                    }

                    yield break;
                }
            }
            yield return path;
        }

        private IEnumerable<Attribute> GetRouteAttributes(IEnumerable<Attribute> attributes)
        {
            return attributes.Where(a => a.GetType().Name == "RouteAttribute" ||
                                         a.GetType().GetTypeInfo().ImplementedInterfaces.Any(t => t.Name == "IHttpRouteInfoProvider") ||
                                         a.GetType().GetTypeInfo().ImplementedInterfaces.Any(t => t.Name == "IRouteTemplateProvider")) // .NET Core
                             .Where((dynamic a) => a.Template != null)
                             .OfType<Attribute>();
        }

        private IEnumerable<Attribute> GetRoutePrefixAttributes(IEnumerable<Attribute> attributes)
        {
            return attributes.Where(a => a.GetType().Name == "RoutePrefixAttribute" ||
                                         a.GetType().GetTypeInfo().ImplementedInterfaces.Any(t => t.Name == "IRoutePrefix"));
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

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpPatchAttribute"))
                yield return SwaggerOperationMethod.Patch;

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpHeadAttribute"))
                yield return SwaggerOperationMethod.Head;

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
        private void LoadParameters(SwaggerOperation operation, List<ParameterInfo> parameters, string httpPath, SwaggerGenerator swaggerGenerator)
        {
            // TODO: Also check other attributes (e.g. FromHeader, ...) 
            // https://docs.asp.net/en/latest/mvc/models/model-binding.html#customize-model-binding-behavior-with-attributes

            foreach (var parameter in parameters.Where(p => p.ParameterType != typeof(CancellationToken) &&
                                                            p.GetCustomAttributes().All(a => a.GetType().Name != "FromServicesAttribute") &&
                                                            p.GetCustomAttributes().All(a => a.GetType().Name != "BindNeverAttribute")))
            {
                var nameLower = parameter.Name.ToLowerInvariant();
                if (httpPath.ToLowerInvariant().Contains("{" + nameLower + "}") ||
                    httpPath.ToLowerInvariant().Contains("{" + nameLower + ":")) // path parameter
                {
                    var operationParameter = swaggerGenerator.CreatePrimitiveParameter(parameter.Name, parameter);
                    operationParameter.Kind = SwaggerParameterKind.Path;
                    operationParameter.IsNullableRaw = false;
                    operationParameter.IsRequired = true; // Path is always required => property not needed

                    operation.Parameters.Add(operationParameter);
                }
                else
                {
                    var parameterInfo = JsonObjectTypeDescription.FromType(parameter.ParameterType, parameter.GetCustomAttributes(), Settings.DefaultEnumHandling);
                    if (TryAddFileParameter(parameterInfo, operation, parameter, swaggerGenerator) == false)
                    {
                        dynamic fromBodyAttribute = parameter.GetCustomAttributes()
                            .SingleOrDefault(a => a.GetType().Name == "FromBodyAttribute");

                        dynamic fromUriAttribute = parameter.GetCustomAttributes()
                            .SingleOrDefault(a => a.GetType().Name == "FromUriAttribute" || a.GetType().Name == "FromQueryAttribute");

                        var bodyParameterName = TryGetStringPropertyValue(fromBodyAttribute, "Name") ?? parameter.Name;
                        var uriParameterName = TryGetStringPropertyValue(fromUriAttribute, "Name") ?? parameter.Name;

                        if (parameterInfo.IsComplexType)
                        {
                            if (fromBodyAttribute != null || (fromUriAttribute == null && Settings.IsAspNetCore == false))
                                AddBodyParameter(bodyParameterName, parameter, operation, swaggerGenerator);
                            else
                                AddPrimitiveParametersFromUri(uriParameterName, operation, parameter, parameterInfo, swaggerGenerator);
                        }
                        else
                        {
                            if (fromBodyAttribute != null)
                                AddBodyParameter(bodyParameterName, parameter, operation, swaggerGenerator);
                            else
                                AddPrimitiveParameter(uriParameterName, operation, parameter, swaggerGenerator);
                        }
                    }
                }
            }

            if (Settings.AddMissingPathParameters)
            {
                foreach (Match match in Regex.Matches(httpPath, "{(.*?)(:(.*?))?}"))
                {
                    var parameterName = match.Groups[1].Value;
                    if (operation.Parameters.All(p => p.Name != parameterName))
                    {
                        var parameterType = match.Groups.Count == 4 ? match.Groups[3].Value : "string";
                        var operationParameter = swaggerGenerator.CreatePathParameter(parameterName, parameterType);
                        operation.Parameters.Add(operationParameter);
                    }
                }
            }

            if (operation.ActualParameters.Any(p => p.Type == JsonObjectType.File))
                operation.Consumes = new List<string> { "multipart/form-data" };

            if (operation.ActualParameters.Count(p => p.Kind == SwaggerParameterKind.Body) > 1)
                throw new InvalidOperationException("The operation '" + operation.OperationId + "' has more than one body parameter.");
        }

        private string TryGetStringPropertyValue(dynamic obj, string propertyName)
        {
            return ((object)obj)?.GetType().GetRuntimeProperty(propertyName) != null && !string.IsNullOrEmpty(obj.Name) ? obj.Name : null;
        }

        private bool TryAddFileParameter(JsonObjectTypeDescription info, SwaggerOperation operation, ParameterInfo parameter, SwaggerGenerator swaggerGenerator)
        {
            var isFileArray = IsFileArray(parameter.ParameterType, info);
            if (info.Type == JsonObjectType.File || isFileArray)
            {
                AddFileParameter(parameter, isFileArray, operation, swaggerGenerator);
                return true;
            }

            return false;
        }

        private void AddFileParameter(ParameterInfo parameter, bool isFileArray, SwaggerOperation operation, SwaggerGenerator swaggerGenerator)
        {
            var attributes = parameter.GetCustomAttributes().ToList();

            // TODO: Check if there is a way to control the property name
            var operationParameter = swaggerGenerator.CreatePrimitiveParameter(parameter.Name, parameter.GetXmlDocumentation(), parameter.ParameterType, attributes);

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

        private void AddBodyParameter(string name, ParameterInfo parameter, SwaggerOperation operation, SwaggerGenerator swaggerGenerator)
        {
            var operationParameter = swaggerGenerator.CreateBodyParameter(name, parameter);
            operation.Parameters.Add(operationParameter);
        }

        private void AddPrimitiveParametersFromUri(string name, SwaggerOperation operation, ParameterInfo parameter, JsonObjectTypeDescription typeDescription, SwaggerGenerator swaggerGenerator)
        {
            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                var operationParameter = swaggerGenerator.CreatePrimitiveParameter(name,
                    parameter.GetXmlDocumentation(), parameter.ParameterType.GetEnumerableItemType(), parameter.GetCustomAttributes().ToList());

                operationParameter.Kind = SwaggerParameterKind.Query;
                operationParameter.CollectionFormat = SwaggerParameterCollectionFormat.Multi;
                operation.Parameters.Add(operationParameter);
            }
            else
            {
                foreach (var property in parameter.ParameterType.GetRuntimeProperties())
                {
                    var attributes = property.GetCustomAttributes().ToList();
                    var fromQueryAttribute = attributes.SingleOrDefault(a => a.GetType().Name == "FromQueryAttribute");

                    var propertyName = TryGetStringPropertyValue(fromQueryAttribute, "Name") ?? JsonPathUtilities.GetPropertyName(property, Settings.DefaultPropertyNameHandling);
                    var operationParameter = swaggerGenerator.CreatePrimitiveParameter(propertyName, property.GetXmlDocumentation(), property.PropertyType, attributes);

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
        }

        private static void InitializeFileParameter(SwaggerParameter operationParameter, bool isFileArray)
        {
            operationParameter.Type = JsonObjectType.File;
            operationParameter.Kind = SwaggerParameterKind.FormData;

            if (isFileArray)
                operationParameter.CollectionFormat = SwaggerParameterCollectionFormat.Multi;
        }

        private void AddPrimitiveParameter(string name, SwaggerOperation operation, ParameterInfo parameter, SwaggerGenerator swaggerGenerator)
        {
            var operationParameter = swaggerGenerator.CreatePrimitiveParameter(name, parameter);
            operationParameter.Kind = SwaggerParameterKind.Query;
            operationParameter.IsRequired = operationParameter.IsRequired || parameter.HasDefaultValue == false;
            operation.Parameters.Add(operationParameter);
        }

        private void LoadReturnType(SwaggerOperation operation, MethodInfo method, SwaggerGenerator swaggerGenerator)
        {
            var successXmlDescription = method.ReturnParameter.GetXmlDocumentation() ?? string.Empty;

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

                    var httpStatusCode = IsVoidResponse(returnType) ? GetVoidResponseStatusCode() : "200";
                    if (responseTypeAttribute.GetType().GetRuntimeProperty("HttpStatusCode") != null)
                        httpStatusCode = dynResultTypeAttribute.HttpStatusCode.ToString();

                    var description = HttpUtilities.IsSuccessStatusCode(httpStatusCode) ? successXmlDescription : string.Empty;
                    if (responseTypeAttribute.GetType().GetRuntimeProperty("Description") != null)
                    {
                        if (!string.IsNullOrEmpty(dynResultTypeAttribute.Description))
                            description = dynResultTypeAttribute.Description;
                    }

                    var typeDescription = JsonObjectTypeDescription.FromType(returnType, method.ReturnParameter?.GetCustomAttributes(), Settings.DefaultEnumHandling);
                    var response = new SwaggerResponse
                    {
                        Description = description ?? string.Empty
                    };

                    if (IsVoidResponse(returnType) == false)
                    {
                        response.IsNullableRaw = typeDescription.IsNullable;
                        response.Schema = swaggerGenerator.GenerateAndAppendSchemaFromType(returnType, typeDescription.IsNullable, null);
                    }

                    operation.Responses[httpStatusCode] = response;
                }

                foreach (dynamic producesResponseTypeAttribute in producesResponseTypeAttributes)
                {
                    var returnType = producesResponseTypeAttribute.Type;
                    var typeDescription = JsonObjectTypeDescription.FromType(returnType, method.ReturnParameter?.GetCustomAttributes(), Settings.DefaultEnumHandling);

                    var httpStatusCode = producesResponseTypeAttribute.StatusCode.ToString(CultureInfo.InvariantCulture);
                    var response = new SwaggerResponse
                    {
                        Description = HttpUtilities.IsSuccessStatusCode(httpStatusCode) ? successXmlDescription : string.Empty
                    };

                    if (IsVoidResponse(returnType) == false)
                    {
                        response.IsNullableRaw = typeDescription.IsNullable;
                        response.Schema = swaggerGenerator.GenerateAndAppendSchemaFromType(returnType, typeDescription.IsNullable, null);
                    }

                    operation.Responses[httpStatusCode] = response;
                }
            }
            else
                LoadDefaultSuccessResponse(operation, method, successXmlDescription, swaggerGenerator);
        }

        private void LoadDefaultSuccessResponse(SwaggerOperation operation, MethodInfo method, string xmlDescription, SwaggerGenerator swaggerGenerator)
        {
            var returnType = method.ReturnType;
            if (returnType == typeof(Task))
                returnType = typeof(void);
            else if (returnType.Name == "Task`1")
                returnType = returnType.GenericTypeArguments[0];

            if (IsVoidResponse(returnType))
            {
                operation.Responses[GetVoidResponseStatusCode()] = new SwaggerResponse
                {
                    Description = xmlDescription
                };
            }
            else
            {
                var typeDescription = JsonObjectTypeDescription.FromType(returnType,
                    method.ReturnParameter?.GetCustomAttributes(), Settings.DefaultEnumHandling);
                operation.Responses["200"] = new SwaggerResponse
                {
                    Description = xmlDescription,
                    IsNullableRaw = typeDescription.IsNullable,
                    Schema = swaggerGenerator.GenerateAndAppendSchemaFromType(returnType, typeDescription.IsNullable, null)
                };
            }
        }

        private bool IsVoidResponse(Type returnType)
        {
            return returnType == null ||
                   returnType.FullName == "System.Void";
        }

        private string GetVoidResponseStatusCode()
        {
            return Settings.IsAspNetCore ? "200" : "204";
        }
    }
}
