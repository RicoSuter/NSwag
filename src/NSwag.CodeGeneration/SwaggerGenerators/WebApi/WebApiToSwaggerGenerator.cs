//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.CodeGeneration.Infrastructure;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Generates a <see cref="SwaggerService"/> object for the given Web API class type. </summary>
    public class WebApiToSwaggerGenerator
    {
        private SwaggerService _service;
        private readonly string _defaultRouteTemplate;
        private Type _serviceType;

        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerGenerator" /> class.</summary>
        /// <param name="defaultRouteTemplate">The default route template.</param>
        public WebApiToSwaggerGenerator(string defaultRouteTemplate) : this (defaultRouteTemplate, new JsonSchemaGeneratorSettings())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerGenerator" /> class.</summary>
        /// <param name="defaultRouteTemplate">The default route template.</param>
        /// <param name="jsonSchemaGeneratorSettings">The JSON Schema generator settings.</param>
        public WebApiToSwaggerGenerator(string defaultRouteTemplate, JsonSchemaGeneratorSettings jsonSchemaGeneratorSettings)
        {
            _defaultRouteTemplate = defaultRouteTemplate;
            JsonSchemaGeneratorSettings = jsonSchemaGeneratorSettings; 
        }

        /// <summary>Gets or sets the JSON Schema generator settings.</summary>
        public JsonSchemaGeneratorSettings JsonSchemaGeneratorSettings { get; set; }
        
        /// <summary>Generates the service description.</summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="excludedMethodName">The name of the excluded method name.</param>
        /// <returns>The <see cref="SwaggerService" />.</returns>
        /// <exception cref="InvalidOperationException">The parameter cannot be an object or array.</exception>
        public SwaggerService Generate<TController>(string excludedMethodName = "Swagger")
        {
            return Generate(typeof(TController), excludedMethodName);
        }

        /// <summary>Generates the service description.</summary>
        /// <param name="controllerType">The type of the controller.</param>
        /// <param name="excludedMethodName">The name of the excluded method name.</param>
        /// <returns>The <see cref="SwaggerService" />.</returns>
        /// <exception cref="InvalidOperationException">The parameter cannot be an object or array.</exception>
        public SwaggerService Generate(Type controllerType, string excludedMethodName = "Swagger")
        {
            _service = new SwaggerService();
            _serviceType = controllerType;

            var schemaResolver = new SchemaResolver();
            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods.Where(m => m.Name != excludedMethodName))
            {
                var parameters = method.GetParameters().ToList();
                var methodName = method.Name;

                var operation = new SwaggerOperation();
                operation.OperationId = methodName;

                var httpPath = GetHttpPath(operation, method, parameters, schemaResolver);

                LoadParameters(operation, parameters, schemaResolver);
                LoadReturnType(operation, method, schemaResolver);
                LoadMetaData(operation, method);

                foreach (var httpMethod in GetSupportedHttpMethods(method))
                {
                    if (!_service.Paths.ContainsKey(httpPath))
                    {
                        var path = new SwaggerOperations();
                        _service.Paths[httpPath] = path;
                    }

                    _service.Paths[httpPath][httpMethod] = operation;
                }
            }

            foreach (var schema in schemaResolver.Schemes)
                _service.Definitions[schema.TypeName] = schema;

            _service.GenerateOperationIds();
            return _service;
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

        private string GetHttpPath(SwaggerOperation operation, MethodInfo method, List<ParameterInfo> parameters, ISchemaResolver schemaResolver)
        {
            string httpPath;

            dynamic routeAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "RouteAttribute");

            if (routeAttribute != null)
            {
                dynamic routePrefixAttribute = _serviceType.GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "RoutePrefixAttribute");

                if (routePrefixAttribute != null)
                    httpPath = routePrefixAttribute.Prefix + "/" + routeAttribute.Template;
                else
                    httpPath = routeAttribute.Template;
            }
            else
            {
                var actionName = GetActionName(method);
                httpPath = _defaultRouteTemplate
                    .Replace("{controller}", _serviceType.Name.Replace("Controller", string.Empty))
                    .Replace("{action}", actionName);
            }

            foreach (var match in Regex.Matches(httpPath, "\\{(.*?)\\}").OfType<Match>())
            {
                var parameterName = match.Groups[1].Value.Split(':').First(); // first segment is parameter name in constrained route (e.g. "[Route("users/{id:int}"]")
                var parameter = parameters.SingleOrDefault(p => p.Name == parameterName);
                if (parameter != null)
                {
                    var operationParameter = CreatePrimitiveParameter(parameter, schemaResolver);
                    operationParameter.Kind = SwaggerParameterKind.Path;

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
                foreach (var verb in ((ICollection<string>)acceptVerbsAttribute.Verbs).Select(v => v.ToLowerInvariant()))
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

        /// <exception cref="InvalidOperationException">The parameter cannot be an object or array. </exception>
        private void LoadParameters(SwaggerOperation operation, List<ParameterInfo> parameters, ISchemaResolver schemaResolver)
        {
            foreach (var parameter in parameters)
            {
                dynamic fromBodyAttribute = parameter.GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "FromBodyAttribute");

                if (fromBodyAttribute != null)
                {
                    var operationParameter = CreateBodyParameter(parameter, schemaResolver);
                    operation.Parameters.Add(operationParameter);
                }
                else
                {
                    var info = JsonObjectTypeDescription.FromType(parameter.ParameterType);
                    if (info.Type.HasFlag(JsonObjectType.Object) || info.Type.HasFlag(JsonObjectType.Array))
                    {
                        if (operation.Parameters.Any(p => p.Kind == SwaggerParameterKind.Body))
                            throw new InvalidOperationException("The parameter '" + parameter.Name + "' cannot be an object or array. ");

                        var operationParameter = CreateBodyParameter(parameter, schemaResolver);
                        operation.Parameters.Add(operationParameter);
                    }
                    else
                    {
                        var operationParameter = CreatePrimitiveParameter(parameter, schemaResolver);
                        operationParameter.Kind = SwaggerParameterKind.Query;

                        operation.Parameters.Add(operationParameter);
                    }
                }
            }
        }

        private SwaggerParameter CreateBodyParameter(ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var operationParameter = new SwaggerParameter();
            operationParameter.Schema = CreateAndAddSchema<SwaggerParameter>(parameter.ParameterType, schemaResolver);
            operationParameter.Name = "request";
            operationParameter.Kind = SwaggerParameterKind.Body;

            var description = parameter.GetXmlDocumentation();
            if (description != string.Empty)
                operationParameter.Description = description;

            return operationParameter;
        }

        private void LoadReturnType(SwaggerOperation operation, MethodInfo method, ISchemaResolver schemaResolver)
        {
            if (method.ReturnType.FullName != "System.Void")
            {
                var description = method.ReturnParameter.GetXmlDocumentation();
                if (description == string.Empty)
                    description = null; 

                var resultTypeAttributes = method.GetCustomAttributes().Where(a => a.GetType().Name == "ResultTypeAttribute").ToList();
                if (resultTypeAttributes.Count > 0)
                {
                    foreach (var resultTypeAttribute in resultTypeAttributes)
                    {
                        dynamic dynResultTypeAttribute = resultTypeAttribute;

                        var httpStatusCode = "200";
                        if (resultTypeAttribute.GetType().GetRuntimeProperty("HttpStatusCode") != null)
                            httpStatusCode = dynResultTypeAttribute.HttpStatusCode;

                        var schema = CreateAndAddSchema<JsonSchema4>(dynResultTypeAttribute.Type, schemaResolver);
                        operation.Responses[httpStatusCode] = new SwaggerResponse
                        {
                            Description = description,
                            Schema = schema
                        };
                    }
                }
                else
                {
                    var schema = CreateAndAddSchema<JsonSchema4>(method.ReturnType, schemaResolver);
                    operation.Responses["200"] = new SwaggerResponse
                    {
                        Description = description, 
                        Schema = schema
                    };
                }
            }
            else
                operation.Responses["200"] = new SwaggerResponse();
        }

        private TSchemaType CreateAndAddSchema<TSchemaType>(Type type, ISchemaResolver schemaResolver)
            where TSchemaType : JsonSchema4, new()
        {
            if (type.Name == "Task`1")
                type = type.GenericTypeArguments[0];

            if (type.Name == "JsonResult`1")
                type = type.GenericTypeArguments[0];

            if (type.Name == "HttpResponseMessage" || type.InheritsFrom("HttpResponseMessage"))
                type = typeof(object);

            var info = JsonObjectTypeDescription.FromType(type);
            if (info.Type.HasFlag(JsonObjectType.Object))
            {
                if (type == typeof(object))
                {
                    return new TSchemaType
                    {
                        Type = JsonObjectType.Object,
                        AllowAdditionalProperties = false
                    };
                }

                if (!schemaResolver.HasSchema(type))
                {
                    var schemaGenerator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);
                    schemaGenerator.Generate<JsonSchema4>(type, schemaResolver);
                }

                return new TSchemaType
                {
                    Type = JsonObjectType.Object,
                    SchemaReference = schemaResolver.GetSchema(type)
                };
            }

            if (info.Type.HasFlag(JsonObjectType.Array))
            {
                var itemType = type.GenericTypeArguments.Length == 0 ? type.GetElementType() : type.GenericTypeArguments[0];
                return new TSchemaType
                {
                    Type = JsonObjectType.Array,
                    Item = CreateAndAddSchema<JsonSchema4>(itemType, schemaResolver)
                };
            }

            var generator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);
            return generator.Generate<TSchemaType>(type, schemaResolver);
        }

        /// <exception cref="InvalidOperationException">The parameter cannot be an object or array. </exception>
        private SwaggerParameter CreatePrimitiveParameter(ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var parameterType = parameter.ParameterType;

            var info = JsonObjectTypeDescription.FromType(parameterType);
            if (info.Type.HasFlag(JsonObjectType.Object) || info.Type.HasFlag(JsonObjectType.Array))
                throw new InvalidOperationException("The parameter '" + parameter.Name + "' cannot be an object or array.");

            var parameterGenerator = new RootTypeJsonSchemaGenerator(_service, JsonSchemaGeneratorSettings);

            var segmentParameter = parameterGenerator.Generate<SwaggerParameter>(parameter.ParameterType, schemaResolver);
            segmentParameter.Name = parameter.Name;
            segmentParameter.Description = parameter.GetXmlDocumentation();
            return segmentParameter;
        }
    }
}