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
using NSwag.CodeGeneration.Infrastructure;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Generates a <see cref="SwaggerService"/> object for the given Web API class type. </summary>
    public class WebApiToSwaggerGenerator
    {
        private SwaggerService _service;
        private readonly string _defaultRouteTemplate;
        private Type _serviceType;
        private JsonSchema4 _exceptionType;

        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerGenerator"/> class.</summary>
        /// <param name="defaultRouteTemplate">The default route template.</param>
        public WebApiToSwaggerGenerator(string defaultRouteTemplate)
        {
            _defaultRouteTemplate = defaultRouteTemplate;
        }

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
        /// <returns>The <see cref="SwaggerService"/>.</returns>
        /// <exception cref="InvalidOperationException">The parameter cannot be an object or array. </exception>
        public SwaggerService Generate(Type controllerType, string excludedMethodName = "Swagger")
        {
            _service = new SwaggerService();
            _serviceType = controllerType;

            _exceptionType = new JsonSchema4();
            _exceptionType.TypeName = "SwaggerException";
            _exceptionType.Properties.Add("ExceptionType", new JsonProperty { Type = JsonObjectType.String });
            _exceptionType.Properties.Add("Message", new JsonProperty { Type = JsonObjectType.String });
            _exceptionType.Properties.Add("StackTrace", new JsonProperty { Type = JsonObjectType.String });

            _service.Definitions[_exceptionType.TypeName] = _exceptionType;

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
                operation.Description = descriptionAttribute.Description;
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
                httpPath = _defaultRouteTemplate
                    .Replace("{controller}", _serviceType.Name.Replace("Controller", string.Empty))
                    .Replace("{action}", method.Name);
            }

            foreach (var match in Regex.Matches(httpPath, "\\{(.*?)\\}").OfType<Match>())
            {
                var parameterName = match.Groups[1].Value.Split(':').First(); // first segment is parameter name in constrained route (e.g. "[Route("users/{id:int}"]")
                var parameter = parameters.SingleOrDefault(p => p.Name == parameterName);
                if (parameter != null)
                {
                    var operationParameter = CreatePrimitiveParameter(parameter, schemaResolver);
                    operationParameter.Kind = SwaggerParameterKind.path;

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

        private IEnumerable<SwaggerOperationMethod> GetSupportedHttpMethods(MethodInfo method)
        {
            // See http://www.asp.net/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api

            var methodName = method.Name;

            var httpMethods = GetSupportedHttpMethodsFromAttributes(method).ToArray(); 
            foreach (var httpMethod in httpMethods)
                yield return httpMethod;

            if (httpMethods.Length == 0)
            {
                if (methodName.StartsWith("Get"))
                    yield return SwaggerOperationMethod.get;
                else if (methodName.StartsWith("Post"))
                    yield return SwaggerOperationMethod.post;
                else if(methodName.StartsWith("Put"))
                    yield return SwaggerOperationMethod.put;
                else if (methodName.StartsWith("Delete"))
                    yield return SwaggerOperationMethod.delete;
                else
                    yield return SwaggerOperationMethod.post;
            }
        }

        private IEnumerable<SwaggerOperationMethod> GetSupportedHttpMethodsFromAttributes(MethodInfo method)
        {
            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpGetAttribute"))
                yield return SwaggerOperationMethod.get;

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpPostAttribute"))
                yield return SwaggerOperationMethod.post;

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpPutAttribute"))
                yield return SwaggerOperationMethod.put;

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpDeleteAttribute"))
                yield return SwaggerOperationMethod.delete;

            if (method.GetCustomAttributes().Any(a => a.GetType().Name == "HttpOptionsAttribute"))
                yield return SwaggerOperationMethod.options;
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
                        if (operation.Parameters.Any(p => p.Kind == SwaggerParameterKind.body))
                            throw new InvalidOperationException("The parameter '" + parameter.Name + "' cannot be an object or array. ");

                        var operationParameter = CreateBodyParameter(parameter, schemaResolver);
                        operation.Parameters.Add(operationParameter);
                    }
                    else
                    {
                        var operationParameter = CreatePrimitiveParameter(parameter, schemaResolver);
                        operationParameter.Kind = SwaggerParameterKind.query;

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
            operationParameter.Kind = SwaggerParameterKind.body;
            return operationParameter;
        }

        private void LoadReturnType(SwaggerOperation operation, MethodInfo method, ISchemaResolver schemaResolver)
        {
            if (method.ReturnType.FullName != "System.Void")
            {
                operation.Responses["200"] = new SwaggerResponse
                {
                    Schema = CreateAndAddSchema<JsonSchema4>(method.ReturnType, schemaResolver)
                };
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

            var info = JsonObjectTypeDescription.FromType(type);

            if (info.Type.HasFlag(JsonObjectType.Object))
            {
                if (!schemaResolver.HasSchema(type))
                {
                    var schemaGenerator = new RootTypeJsonSchemaGenerator(_service);
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

            var generator = new RootTypeJsonSchemaGenerator(_service);
            return generator.Generate<TSchemaType>(type, schemaResolver);
        }

        /// <exception cref="InvalidOperationException">The parameter cannot be an object or array. </exception>
        private SwaggerParameter CreatePrimitiveParameter(ParameterInfo parameter, ISchemaResolver schemaResolver)
        {
            var parameterType = parameter.ParameterType;

            var info = JsonObjectTypeDescription.FromType(parameterType);
            if (info.Type.HasFlag(JsonObjectType.Object) || info.Type.HasFlag(JsonObjectType.Array))
                throw new InvalidOperationException("The parameter '" + parameter.Name + "' cannot be an object or array.");

            var parameterGenerator = new RootTypeJsonSchemaGenerator(_service);

            var segmentParameter = parameterGenerator.Generate<SwaggerParameter>(parameter.ParameterType, schemaResolver);
            segmentParameter.Name = parameter.Name;
            return segmentParameter;
        }
    }
}