//-----------------------------------------------------------------------
// <copyright file="AspNetCoreToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.AspNetCore
{
    /// <summary>Generates a <see cref="OpenApiDocument"/> using <see cref="ApiDescription"/>. </summary>
    public class AspNetCoreOpenApiDocumentGenerator
    {
        /// <summary>Initializes a new instance of the <see cref="AspNetCoreOpenApiDocumentGenerator" /> class.</summary>
        /// <param name="settings">The settings.</param>
        public AspNetCoreOpenApiDocumentGenerator(AspNetCoreOpenApiDocumentGeneratorSettings settings)
        {
            Settings = settings;
        }

        /// <summary>Gets the generator settings.</summary>
        public AspNetCoreOpenApiDocumentGeneratorSettings Settings { get; }

        /// <summary>Generates the <see cref="OpenApiDocument"/> with services from the given service provider.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The document</returns>
        public Task<OpenApiDocument> GenerateAsync(object serviceProvider)
        {
            var typedServiceProvider = (IServiceProvider)serviceProvider;
            var apiDescriptionGroupCollectionProvider = typedServiceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
            return GenerateAsync(apiDescriptionGroupCollectionProvider.ApiDescriptionGroups);
        }

        /// <summary>Loads the <see cref="GetJsonSerializerSettings"/> from the given service provider.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The settings.</returns>
        public static JsonSerializerSettings GetJsonSerializerSettings(IServiceProvider serviceProvider)
        {
            dynamic GetJsonOptionsWithReflection(IServiceProvider sp)
            {
                try
                {
                    // Try to load ASP.NET Core 3 options
                    var optionsAssembly = Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.NewtonsoftJson"));
                    var optionsType = typeof(IOptions<>).MakeGenericType(optionsAssembly.GetType("Microsoft.AspNetCore.Mvc.MvcNewtonsoftJsonOptions", true));
                    return sp?.GetService(optionsType);
                }
                catch
                {
                    // Newtonsoft.JSON not available, use GetSystemTextJsonSettings()
                    return null;
                }
            }

#if NETCOREAPP3_1_OR_GREATER
            dynamic options = GetJsonOptionsWithReflection(serviceProvider);
#else
            object options = null;
            try
            {
                options = new Func<dynamic>(() => serviceProvider?.GetRequiredService(typeof(IOptions<MvcJsonOptions>)))();
            }
            catch
            {
                options = GetJsonOptionsWithReflection(serviceProvider);
            }
#endif

            try
            {
                return (JsonSerializerSettings)((dynamic)options.GetType().GetProperty("Value")?.GetValue(options))?.SerializerSettings;
            }
            catch
            {
                // Newtonsoft.JSON not available, use GetSystemTextJsonSettings()
                return null;
            }
        }

        /// <summary>Generates a Swagger specification for the given <see cref="ApiDescriptionGroupCollection"/>.</summary>
        /// <param name="apiDescriptionGroups">The <see cref="ApiDescriptionGroupCollection"/>.</param>
        /// <returns>The <see cref="OpenApiDocument" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public async Task<OpenApiDocument> GenerateAsync(ApiDescriptionGroupCollection apiDescriptionGroups)
        {
            var apiDescriptions = apiDescriptionGroups.Items
                .Where(group =>
                    Settings.ApiGroupNames == null ||
                    Settings.ApiGroupNames.Length == 0 ||
                    Settings.ApiGroupNames.Contains(group.GroupName))
                .SelectMany(g => g.Items)
                .ToArray();

            var document = await CreateDocumentAsync().ConfigureAwait(false);
            var schemaResolver = new OpenApiSchemaResolver(document, Settings.SchemaSettings);

            var apiGroups = apiDescriptions
                .Select(apiDescription => new Tuple<ApiDescription, ActionDescriptor>(apiDescription, apiDescription.ActionDescriptor))
                .GroupBy(item => (item.Item2 as ControllerActionDescriptor)?.ControllerTypeInfo.AsType())
                .ToArray();

            var generator = new OpenApiDocumentGenerator(Settings, schemaResolver);
            var usedControllerTypes = GenerateApiGroups(generator, document, apiGroups, schemaResolver);

            document.GenerateOperationIds();

            var controllerTypes = apiGroups
                .Select(k => k.Key)
                .Where(t => t != null)
                .ToArray();

            foreach (var processor in Settings.DocumentProcessors)
            {
                processor.Process(new DocumentProcessorContext(document, controllerTypes, usedControllerTypes, schemaResolver, generator.SchemaGenerator, Settings));
            }

            Settings.PostProcess?.Invoke(document);
            return document;
        }

        /// <summary>Gets the default serializer settings representing System.Text.Json.</summary>
        /// <returns>The settings.</returns>
        public static JsonSerializerOptions GetSystemTextJsonSettings(IServiceProvider serviceProvider)
        {
            // If the ASP.NET Core website does not use Newtonsoft.JSON we need to provide a
            // contract resolver which reflects best the System.Text.Json behavior.
            // See https://github.com/RicoSuter/NSwag/issues/2243

            if (serviceProvider != null)
            {
                try
                {
                    var optionsAssembly = Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.Core"));
                    var optionsType = typeof(IOptions<>).MakeGenericType(optionsAssembly.GetType("Microsoft.AspNetCore.Mvc.JsonOptions", true));

                    var options = serviceProvider.GetService(optionsType);
                    var value = optionsType.GetProperty("Value")?.GetValue(options);
                    var jsonOptions = value?.GetType().GetProperty("JsonSerializerOptions")?.GetValue(value);
                    if (jsonOptions is JsonSerializerOptions)
                    {
                        return (JsonSerializerOptions)jsonOptions;
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        private List<Type> GenerateApiGroups(
            OpenApiDocumentGenerator generator,
            OpenApiDocument document,
            IGrouping<Type, Tuple<ApiDescription, ActionDescriptor>>[] apiGroups,
            OpenApiSchemaResolver schemaResolver)
        {
            var usedControllerTypes = new List<Type>();
            var allOperations = new List<Tuple<OpenApiOperationDescription, ApiDescription, MethodInfo>>();
            foreach (var apiGroup in apiGroups)
            {
                var controllerType = apiGroup.Key;

                var hasIgnoreAttribute = controllerType != null && controllerType
                    .GetTypeInfo()
                    .GetCustomAttributes()
                    .GetAssignableToTypeName("SwaggerIgnoreAttribute", TypeNameStyle.Name)
                    .Any();

                if (!hasIgnoreAttribute)
                {
                    var operations = new List<Tuple<OpenApiOperationDescription, ApiDescription, MethodInfo>>();
                    foreach (var item in apiGroup)
                    {
                        var apiDescription = item.Item1;
                        if (apiDescription.RelativePath == null)
                        {
                            continue;
                        }

                        var method = (item.Item2 as ControllerActionDescriptor)?.MethodInfo;
                        if (method != null)
                        {
                            var actionHasIgnoreAttribute = method.GetCustomAttributes().GetAssignableToTypeName("SwaggerIgnoreAttribute", TypeNameStyle.Name).Any();
                            if (actionHasIgnoreAttribute)
                            {
                                continue;
                            }
                        }

                        var path = apiDescription.RelativePath;
                        if (!path.StartsWith("/", StringComparison.Ordinal))
                        {
                            path = "/" + path;
                        }

                        var httpMethod = apiDescription.HttpMethod?.ToLowerInvariant();
                        if (httpMethod == null)
                        {
                            httpMethod = apiDescription.ParameterDescriptions.Any(p => p.Source == BindingSource.Body)
                                ? OpenApiOperationMethod.Post
                                : OpenApiOperationMethod.Get;
                        }

                        var operationDescription = new OpenApiOperationDescription
                        {
                            Path = path,
                            Method = httpMethod,
                            Operation = new OpenApiOperation
                            {
                                IsDeprecated = IsOperationDeprecated(item.Item1, apiDescription.ActionDescriptor, method),
                                OperationId = GetOperationId(document, apiDescription, method, httpMethod),
                                Consumes = apiDescription.SupportedRequestFormats
                                   .Select(f => f.MediaType)
                                   .Distinct()
                                   .ToList(),
                                Produces = apiDescription.SupportedResponseTypes
                                   .SelectMany(t => t.ApiResponseFormats.Select(f => f.MediaType))
                                   .Distinct()
                                   .ToList()
                            }
                        };

                        operations.Add(new Tuple<OpenApiOperationDescription, ApiDescription, MethodInfo>(operationDescription, apiDescription, method));
                    }

                    var addedOperations = AddOperationDescriptionsToDocument(document, controllerType, operations, generator, schemaResolver);
                    if (addedOperations.Any() && apiGroup.Key != null)
                    {
                        usedControllerTypes.Add(apiGroup.Key);
                    }

                    allOperations.AddRange(addedOperations);
                }
            }

            UpdateConsumesAndProduces(document, allOperations);
            return usedControllerTypes;
        }

        private bool IsOperationDeprecated(ApiDescription apiDescription, ActionDescriptor actionDescriptor, MethodInfo methodInfo)
        {
            if (methodInfo?.GetCustomAttribute<ObsoleteAttribute>() != null)
            {
                return true;
            }

            dynamic apiVersionModel = actionDescriptor?
                .Properties
                .FirstOrDefault(p => p.Key is Type type && type.Name == "ApiVersionModel")
                .Value;

            var isDeprecated = (bool)(apiVersionModel != null) &&
                ((IEnumerable)apiVersionModel.DeprecatedApiVersions).OfType<object>()
                .Any(v => v.ToString() == apiDescription.GroupName);

            return isDeprecated;
        }

        private List<Tuple<OpenApiOperationDescription, ApiDescription, MethodInfo>> AddOperationDescriptionsToDocument(
            OpenApiDocument document, Type controllerType,
            List<Tuple<OpenApiOperationDescription, ApiDescription, MethodInfo>> operations,
            OpenApiDocumentGenerator swaggerGenerator, OpenApiSchemaResolver schemaResolver)
        {
            var addedOperations = new List<Tuple<OpenApiOperationDescription, ApiDescription, MethodInfo>>();
            var allOperations = new List<OpenApiOperationDescription>(operations.Count);
            allOperations.AddRange(operations.Select(t => t.Item1));

            foreach (var tuple in operations)
            {
                var operation = tuple.Item1;
                var apiDescription = tuple.Item2;
                var method = tuple.Item3;

                var addOperation = RunOperationProcessors(
                    document,
                    apiDescription,
                    controllerType,
                    method,
                    operation,
                    allOperations,
                    swaggerGenerator,
                    schemaResolver);

                if (addOperation)
                {
                    var path = operation.Path.Replace("//", "/");
                    if (!document.Paths.TryGetValue(path, out var pathItem))
                    {
                        document.Paths[path] = pathItem = new OpenApiPathItem();
                    }

                    if (pathItem.ContainsKey(operation.Method))
                    {
                        throw new InvalidOperationException($"The method '{operation.Method}' on path '{path}' is registered multiple times.");
                    }

                    pathItem[operation.Method] = operation.Operation;
                    addedOperations.Add(tuple);
                }
            }

            return addedOperations;
        }

        private static void UpdateConsumesAndProduces(
            OpenApiDocument document,
            List<Tuple<OpenApiOperationDescription, ApiDescription, MethodInfo>> allOperations)
        {
            // TODO: Move to SwaggerGenerator class?

            document.Consumes = allOperations
                .SelectMany(s => s.Item1.Operation.Consumes)
                .Where(m => allOperations.All(o => o.Item1.Operation.Consumes.Contains(m)))
                .Distinct()
                .ToArray();

            document.Produces = allOperations
                .SelectMany(s => s.Item1.Operation.Produces)
                .Where(m => allOperations.All(o => o.Item1.Operation.Produces.Contains(m)))
                .Distinct()
                .ToArray();

            foreach (var operation in allOperations)
            {
                var description = operation.Item1;

                List<string> consumes = null;
                if (description.Operation.Consumes.Count > 0
                    && (document.Consumes.Count == 0 || description.Operation.Consumes.Any(c => !document.Consumes.Contains(c))))
                {
                    consumes = description.Operation.Consumes.Distinct().ToList();
                }
                description.Operation.Consumes = consumes;

                List<string> produces = null;
                if (description.Operation.Produces.Count > 0
                    && (document.Produces.Count == 0 || description.Operation.Produces.Any(c => !document.Produces.Contains(c))))
                {
                    produces = description.Operation.Produces.Distinct().ToList();
                }
                description.Operation.Produces = produces;
            }
        }

        private async Task<OpenApiDocument> CreateDocumentAsync()
        {
            var document = !string.IsNullOrEmpty(Settings.DocumentTemplate) ?
                await OpenApiDocument.FromJsonAsync(Settings.DocumentTemplate).ConfigureAwait(false) :
                new OpenApiDocument();

            document.Generator = $"NSwag v{OpenApiDocument.ToolchainVersion} (NJsonSchema v{JsonSchema.ToolchainVersion})";
            document.SchemaType = Settings.SchemaSettings.SchemaType;

            if (document.Info == null)
            {
                document.Info = new OpenApiInfo();
            }

            if (string.IsNullOrEmpty(Settings.DocumentTemplate))
            {
                if (!string.IsNullOrEmpty(Settings.Title))
                {
                    document.Info.Title = Settings.Title;
                }

                if (!string.IsNullOrEmpty(Settings.Description))
                {
                    document.Info.Description = Settings.Description;
                }

                if (!string.IsNullOrEmpty(Settings.Version))
                {
                    document.Info.Version = Settings.Version;
                }
            }

            return document;
        }

        private bool RunOperationProcessors(OpenApiDocument document, ApiDescription apiDescription, Type controllerType, MethodInfo methodInfo, OpenApiOperationDescription operationDescription, List<OpenApiOperationDescription> allOperations, OpenApiDocumentGenerator generator, OpenApiSchemaResolver schemaResolver)
        {
            // 1. Run from settings
            var operationProcessorContext = new AspNetCoreOperationProcessorContext(document, operationDescription, controllerType, methodInfo, generator, schemaResolver, Settings, allOperations)
            {
                ApiDescription = apiDescription,
            };

            foreach (var operationProcessor in Settings.OperationProcessors)
            {
                if (operationProcessor.Process(operationProcessorContext) == false)
                {
                    return false;
                }
            }

            if (methodInfo != null)
            {
                // 2. Run from class attributes
                var operationProcessorAttribute = methodInfo.DeclaringType.GetTypeInfo()
                    .GetCustomAttributes()
                    // 3. Run from method attributes
                    .Concat(methodInfo.GetCustomAttributes())
                    .Where(a => a.GetType().IsAssignableToTypeName("SwaggerOperationProcessorAttribute", TypeNameStyle.Name));

                foreach (dynamic attribute in operationProcessorAttribute)
                {
                    var operationProcessor = ObjectExtensions.HasProperty(attribute, "Parameters") ?
                        (IOperationProcessor)Activator.CreateInstance(attribute.Type, attribute.Parameters) :
                        (IOperationProcessor)Activator.CreateInstance(attribute.Type);

                    if (operationProcessor.Process(operationProcessorContext) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private string GetOperationId(OpenApiDocument document, ApiDescription apiDescription, MethodInfo method, string httpMethod)
        {
            string operationId;

            dynamic swaggerOperationAttribute = method?
                .GetCustomAttributes()
                .FirstAssignableToTypeNameOrDefault("SwaggerOperationAttribute", TypeNameStyle.Name);

            dynamic httpAttribute = null;
            if (!string.IsNullOrWhiteSpace(httpMethod))
            {
                var attributeName = Char.ToUpperInvariant(httpMethod[0]) + httpMethod.Substring(1).ToLowerInvariant();
                var typeName = string.Format("Microsoft.AspNetCore.Mvc.Http{0}Attribute", attributeName);
                httpAttribute = method?
                    .GetCustomAttributes()
                    .FirstAssignableToTypeNameOrDefault(typeName);
            }

            if (swaggerOperationAttribute != null && !string.IsNullOrEmpty(swaggerOperationAttribute.OperationId))
            {
                operationId = swaggerOperationAttribute.OperationId;
            }
            else if (Settings.UseHttpAttributeNameAsOperationId && httpAttribute != null && !string.IsNullOrWhiteSpace(httpAttribute.Name))
            {
                operationId = httpAttribute.Name;
            }
            else if (Settings.UseRouteNameAsOperationId && !string.IsNullOrEmpty(apiDescription.ActionDescriptor.AttributeRouteInfo.Name))
            {
                operationId = apiDescription.ActionDescriptor.AttributeRouteInfo.Name;
            }
            else if (apiDescription.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                dynamic openApiControllerAttribute = controllerActionDescriptor
                    .ControllerTypeInfo?
                    .GetCustomAttributes()?
                    .FirstAssignableToTypeNameOrDefault("OpenApiControllerAttribute", TypeNameStyle.Name);

                var controllerName =
                    openApiControllerAttribute != null && !string.IsNullOrEmpty(openApiControllerAttribute.Name) ?
                    openApiControllerAttribute.Name :
                    controllerActionDescriptor.ControllerName;

                operationId = controllerName + "_" + GetActionName(controllerActionDescriptor.ActionName);
            }
            else
            {
#if NET5_0_OR_GREATER
                var routeName = apiDescription
                    .ActionDescriptor
                    .EndpointMetadata?
                    .OfType<RouteNameMetadata>()
                    .FirstOrDefault()?
                    .RouteName;

                if (routeName != null)
                {
                    return routeName;
                }
#endif

                // From HTTP method and route
                operationId =
                    httpMethod[0].ToString().ToUpperInvariant() + httpMethod.Substring(1) +
                    string.Join("", apiDescription.RelativePath
                        .Split('/', '\\', '}', ']', '-', '_')
                        .Where(t => !t.StartsWith("{"))
                        .Where(t => !t.StartsWith("["))
                        .Select(t => t.Length > 1 ? t[0].ToString().ToUpperInvariant() + t.Substring(1) : t.ToUpperInvariant()));
            }

            var number = 1;
            while (document.Operations.Any(o => o.Operation.OperationId == operationId + (number > 1 ? "_" + number : string.Empty)))
            {
                number++;
            }

            return operationId + (number > 1 ? number.ToString() : string.Empty);
        }

        private static string GetActionName(string actionName)
        {
            if (actionName.EndsWith("Async"))
            {
                actionName = actionName.Substring(0, actionName.Length - 5);
            }

            return actionName;
        }
    }
}
