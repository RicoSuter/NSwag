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
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;
using NSwag.SwaggerGeneration.WebApi.Infrastructure;

namespace NSwag.SwaggerGeneration.WebApi
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
                .Where(t => t.GetTypeInfo().ImplementedInterfaces.All(i => i.FullName != "System.Web.Mvc.IController")) // no MVC controllers (legacy ASP.NET)
                .Where(t =>
                {
                    return t.GetTypeInfo().GetCustomAttributes()
                        .SingleOrDefault(a => a.GetType().Name == "ApiExplorerSettingsAttribute")?
                        .TryGetPropertyValue("IgnoreApi", false) != true;

                });
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public WebApiToSwaggerGeneratorSettings Settings { get; }

        /// <summary>Generates a Swagger specification for the given controller type.</summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <returns>The <see cref="SwaggerDocument" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public Task<SwaggerDocument> GenerateForControllerAsync<TController>()
        {
            return GenerateForControllersAsync(new[] { typeof(TController) });
        }

        /// <summary>Generates a Swagger specification for the given controller type.</summary>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>The <see cref="SwaggerDocument" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public Task<SwaggerDocument> GenerateForControllerAsync(Type controllerType)
        {
            return GenerateForControllersAsync(new[] { controllerType });
        }

        /// <summary>Generates a Swagger specification for the given controller types.</summary>
        /// <param name="controllerTypes">The types of the controller.</param>
        /// <returns>The <see cref="SwaggerDocument" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public async Task<SwaggerDocument> GenerateForControllersAsync(IEnumerable<Type> controllerTypes)
        {
            var document = await CreateDocumentAsync(Settings).ConfigureAwait(false);
            var schemaResolver = new SwaggerSchemaResolver(document, Settings);

            foreach (var controllerType in controllerTypes)
                await GenerateForControllerAsync(document, controllerType, new SwaggerGenerator(_schemaGenerator, Settings, schemaResolver), schemaResolver).ConfigureAwait(false);

            document.GenerateOperationIds();

            foreach (var processor in Settings.DocumentProcessors)
                await processor.ProcessAsync(new DocumentProcessorContext(document, controllerTypes, schemaResolver, _schemaGenerator));

            return document;
        }

        private async Task<SwaggerDocument> CreateDocumentAsync(WebApiToSwaggerGeneratorSettings settings)
        {
            var document = !string.IsNullOrEmpty(settings.DocumentTemplate) ?
                await SwaggerDocument.FromJsonAsync(settings.DocumentTemplate).ConfigureAwait(false) :
                new SwaggerDocument();

            document.Generator = "NSwag v" + SwaggerDocument.ToolchainVersion + " (NJsonSchema v" + JsonSchema4.ToolchainVersion + ")";
            document.Consumes = new List<string> { "application/json" };
            document.Produces = new List<string> { "application/json" };

            if (document.Info == null)
                document.Info = new SwaggerInfo();

            if (!string.IsNullOrEmpty(settings.Title))
                document.Info.Title = settings.Title;
            if (!string.IsNullOrEmpty(settings.Description))
                document.Info.Description = settings.Description;
            if (!string.IsNullOrEmpty(settings.Version))
                document.Info.Version = settings.Version;

            return document;
        }

        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        private async Task GenerateForControllerAsync(SwaggerDocument document, Type controllerType, SwaggerGenerator swaggerGenerator, SwaggerSchemaResolver schemaResolver)
        {
            var hasIgnoreAttribute = controllerType.GetTypeInfo()
                .GetCustomAttributes()
                .Any(a => a.GetType().Name == "SwaggerIgnoreAttribute");

            if (!hasIgnoreAttribute)
            {
                var operations = new List<Tuple<SwaggerOperationDescription, MethodInfo>>();

                var currentControllerType = controllerType;
                while (currentControllerType != null)
                {
                    foreach (var method in GetActionMethods(currentControllerType))
                    {
                        var httpPaths = GetHttpPaths(controllerType, method).ToList();
                        var httpMethods = GetSupportedHttpMethods(method).ToList();

                        foreach (var httpPath in httpPaths)
                        {
                            foreach (var httpMethod in httpMethods)
                            {
                                var isPathAlreadyDefinedInInheritanceHierarchy =
                                    operations.Any(o => o.Item1.Path == httpPath && 
                                                        o.Item1.Method == httpMethod && 
                                                        o.Item2.DeclaringType != currentControllerType &&
                                                        o.Item2.DeclaringType.IsAssignableTo(currentControllerType.FullName, TypeNameStyle.FullName));

                                if (isPathAlreadyDefinedInInheritanceHierarchy == false)
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
                    }

                    currentControllerType = currentControllerType.GetTypeInfo().BaseType;
                }

                await AddOperationDescriptionsToDocumentAsync(document, controllerType, operations, swaggerGenerator, schemaResolver).ConfigureAwait(false);
            }
        }

        private async Task AddOperationDescriptionsToDocumentAsync(SwaggerDocument document, Type controllerType, List<Tuple<SwaggerOperationDescription, MethodInfo>> operations, SwaggerGenerator swaggerGenerator, SwaggerSchemaResolver schemaResolver)
        {
            var allOperation = operations.Select(t => t.Item1).ToList();
            foreach (var tuple in operations)
            {
                var operation = tuple.Item1;
                var method = tuple.Item2;

                var addOperation = await RunOperationProcessorsAsync(document, controllerType, method, operation, allOperation, swaggerGenerator, schemaResolver).ConfigureAwait(false);
                if (addOperation)
                {
                    var path = operation.Path.Replace("//", "/");

                    if (!document.Paths.ContainsKey(path))
                        document.Paths[path] = new SwaggerPathItem();

                    if (document.Paths[path].ContainsKey(operation.Method))
                    {
                        throw new InvalidOperationException("The method '" + operation.Method + "' on path '" + path + "' is registered multiple times " +
                            "(check the DefaultUrlTemplate setting [default for Web API: 'api/{controller}/{id}'; for MVC projects: '{controller}/{action}/{id?}']).");
                    }

                    document.Paths[path][operation.Method] = operation.Operation;
                }
            }
        }

        private async Task<bool> RunOperationProcessorsAsync(SwaggerDocument document, Type controllerType, MethodInfo methodInfo, SwaggerOperationDescription operationDescription, List<SwaggerOperationDescription> allOperations, SwaggerGenerator swaggerGenerator, SwaggerSchemaResolver schemaResolver)
        {
            var context = new OperationProcessorContext(document, operationDescription, controllerType,
                methodInfo, swaggerGenerator, _schemaGenerator, schemaResolver, allOperations);

            // 1. Run from settings
            foreach (var operationProcessor in Settings.OperationProcessors)
            {
                if (await operationProcessor.ProcessAsync(context).ConfigureAwait(false) == false)
                    return false;
            }

            // 2. Run from class attributes
            var operationProcessorAttribute = methodInfo.DeclaringType.GetTypeInfo()
                .GetCustomAttributes()
            // 3. Run from method attributes
                .Concat(methodInfo.GetCustomAttributes())
                .Where(a => a.GetType().IsAssignableTo("SwaggerOperationProcessorAttribute", TypeNameStyle.Name));

            foreach (dynamic attribute in operationProcessorAttribute)
            {
                var operationProcessor = ReflectionExtensions.HasProperty(attribute, "Parameters") ?
                    (IOperationProcessor)Activator.CreateInstance(attribute.Type, attribute.Parameters) :
                    (IOperationProcessor)Activator.CreateInstance(attribute.Type);

                if (await operationProcessor.ProcessAsync(context).ConfigureAwait(false) == false)
                    return false;
            }

            return true;
        }

        private static IEnumerable<MethodInfo> GetActionMethods(Type controllerType)
        {
            var methods = controllerType.GetRuntimeMethods().Where(m => m.IsPublic);
            return methods.Where(m =>
                m.IsSpecialName == false && // avoid property methods
                m.DeclaringType == controllerType && // no inherited methods (handled in GenerateForControllerAsync)
                m.DeclaringType != typeof(object) &&
                m.IsStatic == false &&
                m.GetCustomAttributes().All(a => a.GetType().Name != "SwaggerIgnoreAttribute" && a.GetType().Name != "NonActionAttribute") &&
                m.DeclaringType.FullName.StartsWith("Microsoft.AspNet") == false && // .NET Core (Web API & MVC)
                m.DeclaringType.FullName != "System.Web.Http.ApiController" &&
                m.DeclaringType.FullName != "System.Web.Mvc.Controller")
                .Where(m =>
                {
                    return m.GetCustomAttributes()
                        .SingleOrDefault(a => a.GetType().Name == "ApiExplorerSettingsAttribute")?
                        .TryGetPropertyValue("IgnoreApi", false) != true;
                });
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

                operationId = controllerName + "_" + GetActionName(method);
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
            var routeAttributeOnClass = GetRouteAttribute(controllerType);
            var routePrefixAttribute = GetRoutePrefixAttribute(controllerType);

            if (routeAttributes.Any())
            {
                foreach (var attribute in routeAttributes)
                {
                    if (attribute.Template.StartsWith("~/")) // ignore route prefixes
                        httpPaths.Add(attribute.Template.Substring(1));
                    else if (routePrefixAttribute != null)
                        httpPaths.Add(routePrefixAttribute.Prefix + "/" + attribute.Template);
                    else if (routeAttributeOnClass != null)
                    {
                        if (attribute.Template.StartsWith("/"))
                            httpPaths.Add(attribute.Template);
                        else
                            httpPaths.Add(routeAttributeOnClass.Template + "/" + attribute.Template);
                    }
                    else
                        httpPaths.Add(attribute.Template);
                }
            }
            else if (routePrefixAttribute != null && routeAttributeOnClass != null)
                httpPaths.Add(routePrefixAttribute.Prefix + "/" + routeAttributeOnClass.Template);
            else if (routePrefixAttribute != null)
                httpPaths.Add(routePrefixAttribute.Prefix);
            else if (routeAttributeOnClass != null)
                httpPaths.Add(routeAttributeOnClass.Template);
            else
                httpPaths.Add(Settings.DefaultUrlTemplate ?? string.Empty);

            var actionName = GetActionName(method);
            return httpPaths
                .SelectMany(p => ExpandOptionalHttpPathParameters(p, method))
                .Select(p =>
                    "/" + p
                        .Replace("[", "{")
                        .Replace("]", "}")
                        .Replace("{controller}", controllerName)
                        .Replace("{action}", actionName)
                        .Replace("{*", "{") // wildcard path parameters are not supported in Swagger
                        .Trim('/'))
                .Distinct()
                .ToList();
        }

        private IEnumerable<string> ExpandOptionalHttpPathParameters(string path, MethodInfo method)
        {
            var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                if (segment.EndsWith("?}"))
                {
                    // Only expand if optional parameter is available in action method
                    if (method.GetParameters().Any(p => segment.StartsWith("{" + p.Name + ":") || segment.StartsWith("{" + p.Name + "?")))
                    {
                        foreach (var p in ExpandOptionalHttpPathParameters(string.Join("/", segments.Take(i).Concat(new[] { segment.Replace("?", "") }).Concat(segments.Skip(i + 1))), method))
                            yield return p;
                    }
                    else
                    {
                        foreach (var p in ExpandOptionalHttpPathParameters(string.Join("/", segments.Take(i).Concat(segments.Skip(i + 1))), method))
                            yield return p;
                    }

                    yield break;
                }
            }

            yield return path;
        }

        private RouteAttributeFacade GetRouteAttribute(Type type)
        {
            do
            {
                var attributes = type.GetTypeInfo().GetCustomAttributes(false).Cast<Attribute>();

                var attribute = GetRouteAttributes(attributes).SingleOrDefault();
                if (attribute != null)
                    return attribute;

                type = type.GetTypeInfo().BaseType;
            } while (type != null);

            return null;
        }

        private RoutePrefixAttributeFacade GetRoutePrefixAttribute(Type type)
        {
            do
            {
                var attributes = type.GetTypeInfo().GetCustomAttributes(false).Cast<Attribute>();

                var attribute = GetRoutePrefixAttributes(attributes).SingleOrDefault();
                if (attribute != null)
                    return attribute;

                type = type.GetTypeInfo().BaseType;
            } while (type != null);

            return null;
        }

        private IEnumerable<RouteAttributeFacade> GetRouteAttributes(IEnumerable<Attribute> attributes)
        {
            return attributes.Select(RouteAttributeFacade.TryMake).Where(a => a?.Template != null);
        }

        private IEnumerable<RoutePrefixAttributeFacade> GetRoutePrefixAttributes(IEnumerable<Attribute> attributes)
        {
            return attributes.Select(RoutePrefixAttributeFacade.TryMake).Where(a => a != null);
        }

        private string GetActionName(MethodInfo method)
        {
            dynamic actionNameAttribute = method.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "ActionNameAttribute");

            if (actionNameAttribute != null)
                return actionNameAttribute.Name;

            var methodName = method.Name;
            if (methodName.EndsWith("Async"))
                methodName = methodName.Substring(0, methodName.Length - 5);

            return methodName;
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
                if (actionName.StartsWith("Get", StringComparison.OrdinalIgnoreCase))
                    yield return SwaggerOperationMethod.Get;
                else if (actionName.StartsWith("Post", StringComparison.OrdinalIgnoreCase))
                    yield return SwaggerOperationMethod.Post;
                else if (actionName.StartsWith("Put", StringComparison.OrdinalIgnoreCase))
                    yield return SwaggerOperationMethod.Put;
                else if (actionName.StartsWith("Delete", StringComparison.OrdinalIgnoreCase))
                    yield return SwaggerOperationMethod.Delete;
                else if (actionName.StartsWith("Patch", StringComparison.OrdinalIgnoreCase))
                    yield return SwaggerOperationMethod.Patch;
                else if (actionName.StartsWith("Options", StringComparison.OrdinalIgnoreCase))
                    yield return SwaggerOperationMethod.Options;
                else if (actionName.StartsWith("Head", StringComparison.OrdinalIgnoreCase))
                    yield return SwaggerOperationMethod.Head;
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
