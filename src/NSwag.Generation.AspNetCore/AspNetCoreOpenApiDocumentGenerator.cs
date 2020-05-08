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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
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
        public async Task<OpenApiDocument> GenerateAsync(object serviceProvider)
        {
            var typedServiceProvider = (IServiceProvider)serviceProvider;

            var mvcOptions = typedServiceProvider.GetRequiredService<IOptions<MvcOptions>>();
            var settings = GetJsonSerializerSettings(typedServiceProvider) ?? GetSystemTextJsonSettings(typedServiceProvider);

            Settings.ApplySettings(settings, mvcOptions.Value);

            var apiDescriptionGroupCollectionProvider = typedServiceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
            return await GenerateAsync(apiDescriptionGroupCollectionProvider.ApiDescriptionGroups);
        }

        /// <summary>Loads the <see cref="GetJsonSerializerSettings"/> from the given service provider.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The settings.</returns>
        public static JsonSerializerSettings GetJsonSerializerSettings(IServiceProvider serviceProvider)
        {
            dynamic options = null;
            try
            {
#if NETCOREAPP3_0
                options = new Func<dynamic>(() => serviceProvider?.GetRequiredService(typeof(IOptions<MvcNewtonsoftJsonOptions>)) as dynamic)();
#else
                options = new Func<dynamic>(() => serviceProvider?.GetRequiredService(typeof(IOptions<MvcJsonOptions>)) as dynamic)();
#endif
            }
            catch
            {
                try
                {
                    // Try load ASP.NET Core 3 options
                    var optionsAssembly = Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.NewtonsoftJson"));
                    var optionsType = typeof(IOptions<>).MakeGenericType(optionsAssembly.GetType("Microsoft.AspNetCore.Mvc.MvcNewtonsoftJsonOptions", true));
                    options = serviceProvider?.GetService(optionsType) as dynamic;
                }
                catch
                {
                    // Newtonsoft.JSON not available, see GetSystemTextJsonSettings()
                    return null;
                }
            }

            return (JsonSerializerSettings)options?.Value?.SerializerSettings;
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
                .Where(apiDescription => apiDescription.ActionDescriptor is ControllerActionDescriptor)
                .ToArray();

            var document = await CreateDocumentAsync().ConfigureAwait(false);
            var schemaResolver = new OpenApiSchemaResolver(document, Settings);

            var apiGroups = apiDescriptions
                .Select(apiDescription => new Tuple<ApiDescription, ControllerActionDescriptor>(apiDescription, (ControllerActionDescriptor)apiDescription.ActionDescriptor))
                .GroupBy(item => item.Item2.ControllerTypeInfo.AsType())
                .ToArray();

            var usedControllerTypes = GenerateForControllers(document, apiGroups, schemaResolver);

            document.GenerateOperationIds();

            var controllerTypes = apiGroups.Select(k => k.Key).ToArray();
            foreach (var processor in Settings.DocumentProcessors)
            {
                processor.Process(new DocumentProcessorContext(document, controllerTypes, usedControllerTypes, schemaResolver, Settings.SchemaGenerator, Settings));
            }

            Settings.PostProcess?.Invoke(document);
            return document;
        }

        /// <summary>Gets the default serializer settings representing System.Text.Json.</summary>
        /// <returns>The settings.</returns>
        public static JsonSerializerSettings GetSystemTextJsonSettings(IServiceProvider serviceProvider)
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
                   
                    var options = serviceProvider?.GetService(optionsType) as dynamic;
                    var jsonOptions = (object)options?.Value?.JsonSerializerOptions;
                    if (jsonOptions != null && jsonOptions.GetType().FullName == "System.Text.Json.JsonSerializerOptions")
                    {
                        return SystemTextJsonUtilities.ConvertJsonOptionsToNewtonsoftSettings(jsonOptions);
                    }
                }
                catch
                {
                }
            }

            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        private List<Type> GenerateForControllers(
            OpenApiDocument document,
            IGrouping<Type, Tuple<ApiDescription, ControllerActionDescriptor>>[] apiGroups,
            OpenApiSchemaResolver schemaResolver)
        {
            var usedControllerTypes = new List<Type>();
            var swaggerGenerator = new OpenApiDocumentGenerator(Settings, schemaResolver);

            var allOperations = new List<Tuple<OpenApiOperationDescription, ApiDescription, MethodInfo>>();
            foreach (var controllerApiDescriptionGroup in apiGroups)
            {
                var controllerType = controllerApiDescriptionGroup.Key;

                var hasIgnoreAttribute = controllerType.GetTypeInfo()
                    .GetCustomAttributes()
                    .GetAssignableToTypeName("SwaggerIgnoreAttribute", TypeNameStyle.Name)
                    .Any();

                if (!hasIgnoreAttribute)
                {
                    var operations = new List<Tuple<OpenApiOperationDescription, ApiDescription, MethodInfo>>();
                    foreach (var item in controllerApiDescriptionGroup)
                    {
                        var apiDescription = item.Item1;
                        var method = item.Item2.MethodInfo;

                        var actionHasIgnoreAttribute = method.GetCustomAttributes().GetAssignableToTypeName("SwaggerIgnoreAttribute", TypeNameStyle.Name).Any();
                        if (actionHasIgnoreAttribute)
                        {
                            continue;
                        }

                        var path = apiDescription.RelativePath;
                        if (!path.StartsWith("/", StringComparison.Ordinal))
                        {
                            path = "/" + path;
                        }

                        var controllerActionDescriptor = (ControllerActionDescriptor)apiDescription.ActionDescriptor;
                        var httpMethod = apiDescription.HttpMethod?.ToLowerInvariant() ?? OpenApiOperationMethod.Get;

                        var operationDescription = new OpenApiOperationDescription
                        {
                            Path = path,
                            Method = httpMethod,
                            Operation = new OpenApiOperation
                            {
                                IsDeprecated = IsOperationDeprecated(item.Item1, controllerActionDescriptor, method),
                                OperationId = GetOperationId(document, controllerActionDescriptor, method),
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

                    var addedOperations = AddOperationDescriptionsToDocument(document, controllerType, operations, swaggerGenerator, schemaResolver);
                    if (addedOperations.Any())
                    {
                        usedControllerTypes.Add(controllerApiDescriptionGroup.Key);
                    }

                    allOperations.AddRange(addedOperations);
                }
            }

            UpdateConsumesAndProduces(document, allOperations);
            return usedControllerTypes;
        }

        private bool IsOperationDeprecated(ApiDescription apiDescription, ControllerActionDescriptor controllerActionDescriptor, MethodInfo methodInfo)
        {
            if (methodInfo.GetCustomAttribute<ObsoleteAttribute>() != null)
            {
                return true;
            }

            dynamic apiVersionModel = controllerActionDescriptor?
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
            var allOperations = operations.Select(t => t.Item1).ToList();
            foreach (var tuple in operations)
            {
                var operation = tuple.Item1;
                var apiDescription = tuple.Item2;
                var method = tuple.Item3;

                var addOperation = RunOperationProcessors(document, apiDescription, controllerType, method, operation,
                    allOperations, swaggerGenerator, schemaResolver);
                if (addOperation)
                {
                    var path = operation.Path.Replace("//", "/");
                    if (!document.Paths.ContainsKey(path))
                    {
                        document.Paths[path] = new OpenApiPathItem();
                    }

                    if (document.Paths[path].ContainsKey(operation.Method))
                    {
                        throw new InvalidOperationException($"The method '{operation.Method}' on path '{path}' is registered multiple times.");
                    }

                    document.Paths[path][operation.Method] = operation.Operation;
                    addedOperations.Add(tuple);
                }
            }

            return addedOperations;
        }

        private void UpdateConsumesAndProduces(OpenApiDocument document,
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

            foreach (var tuple in allOperations)
            {
                var consumes = tuple.Item1.Operation.Consumes.Distinct().ToArray();
                tuple.Item1.Operation.Consumes = consumes.Any(c => !document.Consumes.Contains(c)) ? consumes.ToList() : null;

                var produces = tuple.Item1.Operation.Produces.Distinct().ToArray();
                tuple.Item1.Operation.Produces = produces.Any(c => !document.Produces.Contains(c)) ? produces.ToList() : null;
            }
        }

        private async Task<OpenApiDocument> CreateDocumentAsync()
        {
            var document = !string.IsNullOrEmpty(Settings.DocumentTemplate) ?
                await OpenApiDocument.FromJsonAsync(Settings.DocumentTemplate).ConfigureAwait(false) :
                new OpenApiDocument();

            document.Generator = $"NSwag v{OpenApiDocument.ToolchainVersion} (NJsonSchema v{JsonSchema.ToolchainVersion})";
            document.SchemaType = Settings.SchemaType;

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

        private bool RunOperationProcessors(OpenApiDocument document, ApiDescription apiDescription, Type controllerType, MethodInfo methodInfo, OpenApiOperationDescription operationDescription, List<OpenApiOperationDescription> allOperations, OpenApiDocumentGenerator swaggerGenerator, OpenApiSchemaResolver schemaResolver)
        {
            // 1. Run from settings
            var operationProcessorContext = new AspNetCoreOperationProcessorContext(document, operationDescription, controllerType, methodInfo, swaggerGenerator, Settings.SchemaGenerator, schemaResolver, Settings, allOperations)
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

            return true;
        }

        private string GetOperationId(OpenApiDocument document, ControllerActionDescriptor actionDescriptor, MethodInfo method)
        {
            string operationId;

            dynamic swaggerOperationAttribute = method
                .GetCustomAttributes()
                .FirstAssignableToTypeNameOrDefault("SwaggerOperationAttribute", TypeNameStyle.Name);

            if (swaggerOperationAttribute != null && !string.IsNullOrEmpty(swaggerOperationAttribute.OperationId))
            {
                operationId = swaggerOperationAttribute.OperationId;
            }
            else if (Settings.UseRouteNameAsOperationId && !string.IsNullOrEmpty(actionDescriptor.AttributeRouteInfo.Name))
            {
                operationId = actionDescriptor.AttributeRouteInfo.Name;
            }
            else
            {
                dynamic openApiControllerAttribute = actionDescriptor
                    .ControllerTypeInfo?
                    .GetCustomAttributes()?
                    .FirstAssignableToTypeNameOrDefault("OpenApiControllerAttribute", TypeNameStyle.Name);

                var controllerName =
                    openApiControllerAttribute != null && !string.IsNullOrEmpty(openApiControllerAttribute.Name) ?
                    openApiControllerAttribute.Name :
                    actionDescriptor.ControllerName;

                operationId = controllerName + "_" + GetActionName(actionDescriptor.ActionName);
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
