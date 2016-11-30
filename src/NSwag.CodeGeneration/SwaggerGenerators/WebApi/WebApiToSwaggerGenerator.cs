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
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors.Contexts;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Generates a <see cref="SwaggerDocument"/> object for the given Web API class type. </summary>
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
                            t.InheritsFrom("ControllerBase", TypeNameStyle.Name)) // in ASP.NET Core, a Web API controller inherits from Controller
                .Where(t => t.GetTypeInfo().ImplementedInterfaces.All(i => i.FullName != "System.Web.Mvc.IController")); // no MVC controllers (legacy ASP.NET)
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public WebApiToSwaggerGeneratorSettings Settings { get; }

        /// <summary>Generates a Swagger specification for the given controller type.</summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <returns>The <see cref="SwaggerDocument" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public SwaggerDocument GenerateForController<TController>()
        {
            return GenerateForControllers(new[] { typeof(TController) });
        }

        /// <summary>Generates a Swagger specification for the given controller type.</summary>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>The <see cref="SwaggerDocument" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public SwaggerDocument GenerateForController(Type controllerType)
        {
            return GenerateForControllers(new[] { controllerType });
        }

        /// <summary>Generates a Swagger specification for the given controller types.</summary>
        /// <param name="controllerTypes">The types of the controller.</param>
        /// <returns>The <see cref="SwaggerDocument" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public SwaggerDocument GenerateForControllers(IEnumerable<Type> controllerTypes)
        {
            var document = CreateDocument(Settings);
            var schemaResolver = new SwaggerSchemaResolver(document, Settings);

            foreach (var controllerType in controllerTypes)
                GenerateForController(document, controllerType, new SwaggerGenerator(_schemaGenerator, Settings, schemaResolver));

            document.GenerateOperationIds();

            foreach (var processor in Settings.DocumentProcessors)
                processor.Process(new DocumentProcessorContext(document, controllerTypes, schemaResolver, _schemaGenerator));

            return document;
        }

        private SwaggerDocument CreateDocument(WebApiToSwaggerGeneratorSettings settings)
        {
            var document = !string.IsNullOrEmpty(settings.DocumentTemplate) ? SwaggerDocument.FromJson(settings.DocumentTemplate) : new SwaggerDocument();

            document.Consumes = new List<string> { "application/json" };
            document.Produces = new List<string> { "application/json" };
            document.Info = new SwaggerInfo
            {
                Title = settings.Title,
                Description = settings.Description,
                Version = settings.Version
            };

            return document;
        }

        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        private void GenerateForController(SwaggerDocument document, Type controllerType, SwaggerGenerator swaggerGenerator)
        {
            var hasIgnoreAttribute = controllerType.GetTypeInfo()
                .GetCustomAttributes()
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
                            var operationDescription = new SwaggerOperationDescription
                            {
                                Path = httpPath,
                                Method = httpMethod,
                                Operation = new SwaggerOperation
                                {
                                    IsDeprecated = method.GetCustomAttribute<ObsoleteAttribute>() != null,
                                    OperationId = GetOperationId(document, controllerType.Name, method)
                                }
                            };

                            operations.Add(new Tuple<SwaggerOperationDescription, MethodInfo>(operationDescription, method));
                        }
                    }
                }

                AddOperationDescriptionsToDocument(document, operations, swaggerGenerator);
            }
        }

        private void AddOperationDescriptionsToDocument(SwaggerDocument document, List<Tuple<SwaggerOperationDescription, MethodInfo>> operations, SwaggerGenerator swaggerGenerator)
        {
            var allOperation = operations.Select(t => t.Item1).ToList();
            foreach (var tuple in operations)
            {
                var operation = tuple.Item1;
                var method = tuple.Item2;

                var addOperation = RunOperationProcessors(document, method, operation, allOperation, swaggerGenerator);
                if (addOperation)
                {
                    if (!document.Paths.ContainsKey(operation.Path))
                        document.Paths[operation.Path] = new SwaggerOperations();

                    if (document.Paths[operation.Path].ContainsKey(operation.Method))
                        throw new InvalidOperationException("The method '" + operation.Method + "' on path '" + operation.Path + "' is registered multiple times.");

                    document.Paths[operation.Path][operation.Method] = operation.Operation;
                }
            }
        }

        private bool RunOperationProcessors(SwaggerDocument document, MethodInfo methodInfo,
            SwaggerOperationDescription operationDescription, List<SwaggerOperationDescription> allOperations, SwaggerGenerator swaggerGenerator)
        {
            // 1. Run from settings
            foreach (var operationProcessor in Settings.OperationProcessors)
            {
                if (operationProcessor.Process(new OperationProcessorContext(document, operationDescription, methodInfo, swaggerGenerator, allOperations)) == false)
                    return false;
            }

            // 2. Run from class attributes
            var operationProcessorAttribute = methodInfo.DeclaringType.GetTypeInfo()
                .GetCustomAttributes()
            // 3. Run from method attributes
                .Concat(methodInfo.GetCustomAttributes())
                .Where(a => a.GetType().Name == "SwaggerOperationProcessorAttribute");

            foreach (dynamic attribute in operationProcessorAttribute)
            {
                var operationProcessor = Activator.CreateInstance(attribute.Type);
                if (operationProcessor.Process(methodInfo, operationDescription, swaggerGenerator, allOperations) == false)
                    return false;
            }

            return true;
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

        private string GetOperationId(SwaggerDocument document, string controllerName, MethodInfo method)
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
            while (document.Operations.Any(o => o.Operation.OperationId == operationId + (number > 1 ? "_" + number : string.Empty)))
                number++;

            return operationId + (number > 1 ? number.ToString() : string.Empty);
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
    }
}
