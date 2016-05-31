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
            var service = CreateService();
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
            var service = CreateService();
            var schemaResolver = new SchemaResolver();

            foreach (var controllerType in controllerTypes)
                GenerateForController(service, controllerType, excludedMethodName, schemaResolver);

            service.GenerateOperationIds();
            return service;
        }

        private static SwaggerService CreateService()
        {
            return new SwaggerService
            {
                Consumes = new List<string> { "application/json" },
                Produces = new List<string> { "application/json" }
            };
        }

        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        private void GenerateForController(SwaggerService service, Type controllerType, string excludedMethodName, SchemaResolver schemaResolver)
        {
            var methods = controllerType.GetRuntimeMethods().Where(m => m.IsPublic);
            foreach (var method in methods.Where(m =>
                m.Name != excludedMethodName &&
                m.IsSpecialName == false && // avoid property methods
                m.DeclaringType != null &&
                m.DeclaringType != typeof(object) &&
                m.GetCustomAttributes().All(a => a.GetType().Name != "SwaggerIgnoreAttribute") &&
                m.DeclaringType.FullName.StartsWith("Microsoft.AspNet") == false && // .NET Core (Web API & MVC)
                m.DeclaringType.FullName != "System.Web.Http.ApiController" &&
                m.DeclaringType.FullName != "System.Web.Mvc.Controller"))
            {
                var parameters = method.GetParameters().ToList();

                var operation = new SwaggerOperation();
                operation.IsDeprecated = method.GetCustomAttribute<ObsoleteAttribute>() != null;

                var httpPath = GetHttpPath(service, operation, controllerType, method, parameters, schemaResolver);

                LoadParameters(service, operation, parameters, schemaResolver);
                LoadReturnType(service, operation, method, schemaResolver);
                LoadMetaData(operation, method);

                operation.OperationId = GetOperationId(service, controllerType.Name, method);

                foreach (var httpMethod in GetSupportedHttpMethods(method))
                {
                    if (!service.Paths.ContainsKey(httpPath))
                    {
                        var path = new SwaggerOperations();
                        service.Paths[httpPath] = path;
                    }

                    service.Paths[httpPath][httpMethod] = operation;
                }
            }

            foreach (var schema in schemaResolver.Schemes)
            {
                if (!service.Definitions.Values.Contains(schema))
                {
                    if (!service.Definitions.ContainsKey(schema.TypeName))
                        service.Definitions[schema.TypeName] = schema;
                    else
                        service.Definitions["ref_" + Guid.NewGuid().ToString().Replace("-", "_")] = schema;
                }
            }
        }

        private string GetOperationId(SwaggerService service, string controllerName, MethodInfo method)
        {
            // TODO: Implement IOperationIdGenerator

            if (controllerName.EndsWith("Controller"))
                controllerName = controllerName.Substring(0, controllerName.Length - 10);

            var methodName = method.Name;
            if (methodName.EndsWith("Async"))
                methodName = methodName.Substring(0, methodName.Length - 5);

            var operationId = controllerName + "_" + methodName;

            var number = 1;
            while (service.Operations.Any(o => o.Operation.OperationId == (operationId + (number > 1 ? "_" + number : string.Empty))))
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

        private string GetHttpPath(SwaggerService service, SwaggerOperation operation, Type controllerType, MethodInfo method, List<ParameterInfo> parameters, ISchemaResolver schemaResolver)
        {
            string httpPath;

            dynamic routeAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "RouteAttribute");

            if (routeAttribute != null)
            {
                dynamic routePrefixAttribute = controllerType.GetTypeInfo().GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "RoutePrefixAttribute");

                if (routePrefixAttribute != null)
                    httpPath = routePrefixAttribute.Prefix + "/" + routeAttribute.Template;
                else
                    httpPath = routeAttribute.Template;
            }
            else
            {
                var actionName = GetActionName(method);
                httpPath = (Settings.DefaultUrlTemplate ?? string.Empty)
                    .Replace("{controller}", controllerType.Name.Replace("Controller", string.Empty))
                    .Replace("{action}", actionName);
            }

            foreach (var match in Regex.Matches(httpPath, "\\{(.*?)\\}").OfType<Match>())
            {
                var parameterName = match.Groups[1].Value.Split(':').First(); // first segment is parameter name in constrained route (e.g. "[Route("users/{id:int}"]")
                var parameter = parameters.SingleOrDefault(p => p.Name == parameterName);
                if (parameter != null)
                {
                    var operationParameter = CreatePrimitiveParameter(service, parameter, schemaResolver, setRequiredProperty: false);
                    operationParameter.Kind = SwaggerParameterKind.Path; // Path is always required => property not needed
                    operationParameter.IsRequired = true; 

                    operation.Parameters.Add(operationParameter);
                    parameters.Remove(parameter);
                }
                else
                {
                    httpPath = httpPath
                        .Replace(match.Value, string.Empty)
                        .Replace("//", "/")
                        .Trim('/');
                }
            }

            return httpPath;
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
        private void LoadParameters(SwaggerService service, SwaggerOperation operation, List<ParameterInfo> parameters, ISchemaResolver schemaResolver)
        {
            foreach (var parameter in parameters)
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
                    service, JsonPathUtilities.GetPropertyName(property), property.GetXmlDocumentation(), property.PropertyType, attributes, schemaResolver);

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
            var operationParameter = CreatePrimitiveParameter(service, parameter, schemaResolver, setRequiredProperty: true);
            operationParameter.Kind = SwaggerParameterKind.Query;
            operation.Parameters.Add(operationParameter);
        }

        private SwaggerParameter CreateBodyParameter(SwaggerService service, ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var isRequired = IsParameterRequired(parameter);

            var operationParameter = new SwaggerParameter();
            operationParameter.Name = parameter.Name;
            operationParameter.Kind = SwaggerParameterKind.Body;
            operationParameter.IsRequired = isRequired;
            operationParameter.Schema = CreateAndAddSchema(service, parameter.ParameterType, !isRequired, parameter.GetCustomAttributes(), schemaResolver);

            var description = parameter.GetXmlDocumentation();
            if (description != string.Empty)
                operationParameter.Description = description;

            return operationParameter;
        }

        private SwaggerParameter CreatePrimitiveParameter(SwaggerService service, ParameterInfo parameter, ISchemaResolver schemaResolver, bool setRequiredProperty)
        {
            return CreatePrimitiveParameter(
                service, parameter.Name, parameter.GetXmlDocumentation(),
                parameter.ParameterType, parameter.GetCustomAttributes(), schemaResolver);
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

        private SwaggerParameter CreatePrimitiveParameter(SwaggerService service, string name, string description,
            Type type, IEnumerable<Attribute> parentAttributes, ISchemaResolver schemaResolver)
        {
            var schemaDefinitionAppender = new SwaggerServiceSchemaDefinitionAppender(service);
            var schemaGenerator = new RootTypeJsonSchemaGenerator(service, schemaDefinitionAppender, Settings);

            var typeDescription = JsonObjectTypeDescription.FromType(type, parentAttributes, Settings.DefaultEnumHandling);
            var parameterType = typeDescription.IsComplexType ? typeof(string) : type; // complex types must be treated as string

            var operationParameter = new SwaggerParameter();
            typeDescription.ApplyType(operationParameter);

            if (parameterType.GetTypeInfo().IsEnum)
                operationParameter.SchemaReference = schemaGenerator.Generate<JsonSchema4>(parameterType, null, parentAttributes, schemaDefinitionAppender, schemaResolver);
            else
                schemaGenerator.ApplyPropertyAnnotations(operationParameter, parentAttributes, typeDescription);

            operationParameter.Name = name;
            operationParameter.IsRequired = parentAttributes?.Any(a => a.GetType().Name == "RequiredAttribute") ?? false;

            if (description != string.Empty)
                operationParameter.Description = description;

            return operationParameter;
        }

        private void LoadReturnType(SwaggerService service, SwaggerOperation operation, MethodInfo method, ISchemaResolver schemaResolver)
        {
            var returnType = method.ReturnType;
            if (returnType == typeof(Task))
                returnType = typeof(void);
            else if (returnType.Name == "Task`1")
                returnType = returnType.GenericTypeArguments[0];

            var xmlDescription = method.ReturnParameter.GetXmlDocumentation();
            if (xmlDescription == string.Empty)
                xmlDescription = null;

            var mayBeNull = !IsParameterRequired(method.ReturnParameter);
            var responseTypeAttributes = method.GetCustomAttributes().Where(a => a.GetType().Name == "ResponseTypeAttribute").ToList();
            if (responseTypeAttributes.Count > 0)
            {
                foreach (var responseTypeAttribute in responseTypeAttributes)
                {
                    dynamic dynResultTypeAttribute = responseTypeAttribute;
                    returnType = dynResultTypeAttribute.ResponseType;

                    var httpStatusCode = IsVoidResponse(returnType) ? "204" : "200";
                    if (responseTypeAttribute.GetType().GetRuntimeProperty("HttpStatusCode") != null)
                        httpStatusCode = dynResultTypeAttribute.HttpStatusCode;

                    var description = xmlDescription;
                    if (responseTypeAttribute.GetType().GetRuntimeProperty("Description") != null)
                    {
                        if (!string.IsNullOrEmpty(dynResultTypeAttribute.Description))
                            description = dynResultTypeAttribute.Description;
                    }

                    operation.Responses[httpStatusCode] = new SwaggerResponse
                    {
                        Description = description,
                        Schema = CreateAndAddSchema(service, returnType, mayBeNull, null, schemaResolver)
                    };
                }
            }
            else
            {
                if (IsVoidResponse(returnType))
                    operation.Responses["204"] = new SwaggerResponse();
                else
                {
                    operation.Responses["200"] = new SwaggerResponse
                    {
                        Description = xmlDescription,
                        Schema = CreateAndAddSchema(service, returnType, mayBeNull, null, schemaResolver)
                    };
                }
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

            var schemaDefinitionAppender = new SwaggerServiceSchemaDefinitionAppender(service);
            var typeDescription = JsonObjectTypeDescription.FromType(type, parentAttributes, Settings.DefaultEnumHandling);
            if (typeDescription.Type.HasFlag(JsonObjectType.Object) && !typeDescription.IsDictionary)
            {
                if (type == typeof(object))
                {
                    return new JsonSchema4
                    {
                        Type = JsonObjectType.Object | JsonObjectType.Null,
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
                    var schema = new JsonSchema4();
                    schema.OneOf.Add(new JsonSchema4 { Type = JsonObjectType.Null });
                    schema.OneOf.Add(new JsonSchema4 { SchemaReference = schemaResolver.GetSchema(type, false) });
                    return schema;
                }
                else
                    return new JsonSchema4 { SchemaReference = schemaResolver.GetSchema(type, false) };
            }

            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                var itemType = type.GenericTypeArguments.Length == 0 ? type.GetElementType() : type.GenericTypeArguments[0];
                return new JsonSchema4
                {
                    Type = JsonObjectType.Array | JsonObjectType.Null,
                    Item = CreateAndAddSchema(service, itemType, false, null, schemaResolver)
                };
            }

            var generator = new RootTypeJsonSchemaGenerator(service, schemaDefinitionAppender, Settings);
            return generator.Generate(type, null, null, schemaDefinitionAppender, schemaResolver);
        }
    }
}