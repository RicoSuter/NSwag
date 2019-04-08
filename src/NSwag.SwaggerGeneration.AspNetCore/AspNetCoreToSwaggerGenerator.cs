//-----------------------------------------------------------------------
// <copyright file="AspNetCoreToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.AspNetCore
{
    /// <summary>Generates a <see cref="SwaggerDocument"/> using <see cref="ApiDescription"/>. </summary>
    public class AspNetCoreToSwaggerGenerator : ISwaggerGenerator
    {
        // Default content types to be used when no content type information is available
        private const string DefaultRequestContentType = "application/json";
        private const string DefaultResponseContentType = "application/json";

        private readonly SwaggerJsonSchemaGenerator _schemaGenerator;

        /// <summary>Initializes a new instance of the <see cref="AspNetCoreToSwaggerGenerator" /> class.</summary>
        /// <param name="settings">The settings.</param>
        public AspNetCoreToSwaggerGenerator(AspNetCoreToSwaggerGeneratorSettings settings)
            : this(settings, new SwaggerJsonSchemaGenerator(settings))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AspNetCoreToSwaggerGenerator" /> class.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        public AspNetCoreToSwaggerGenerator(AspNetCoreToSwaggerGeneratorSettings settings, SwaggerJsonSchemaGenerator schemaGenerator)
        {
            Settings = settings;
            _schemaGenerator = schemaGenerator;
        }

        /// <summary>Gets the generator settings.</summary>
        public AspNetCoreToSwaggerGeneratorSettings Settings { get; }

        /// <summary>Generates a Swagger specification for the given <see cref="ApiDescriptionGroupCollection"/>.</summary>
        /// <param name="apiDescriptionGroups">The <see cref="ApiDescriptionGroupCollection"/>.</param>
        /// <returns>The <see cref="SwaggerDocument" />.</returns>
        /// <exception cref="InvalidOperationException">The operation has more than one body parameter.</exception>
        public async Task<SwaggerDocument> GenerateAsync(ApiDescriptionGroupCollection apiDescriptionGroups)
        {
            var document = await CreateDocumentAsync().ConfigureAwait(false);
            var schemaResolver = new SwaggerSchemaResolver(document, Settings);

            var descriptorInfos = PrefilterDescriptions(apiDescriptionGroups);

            AddGlobalConsumesProducesToDocument(document, descriptorInfos);

            var apiGroups = descriptorInfos
                .GroupBy(i => i.ControllerType)
                .ToArray();

            var usedControllerTypes = new List<Type>();
            foreach (var controllerApiDescriptionGroup in apiGroups)
            {
                var generator = new SwaggerGenerator(_schemaGenerator, Settings, schemaResolver);
                var isIncluded = await GenerateForControllerAsync(document, controllerApiDescriptionGroup.Key, controllerApiDescriptionGroup.ToList(), generator, schemaResolver).ConfigureAwait(false);
                if (isIncluded)
                    usedControllerTypes.Add(controllerApiDescriptionGroup.Key);
            }

            document.GenerateOperationIds();

            var controllerTypes = apiGroups.Select(k => k.Key).ToArray();
            foreach (var processor in Settings.DocumentProcessors)
                await processor.ProcessAsync(new DocumentProcessorContext(document, controllerTypes, usedControllerTypes, schemaResolver, _schemaGenerator, Settings));

            return document;
        }

        private List<ApiDescriptorInfo> PrefilterDescriptions(ApiDescriptionGroupCollection apiDescriptionGroups)
        {
            var apiDescriptions = apiDescriptionGroups.Items
                .Where(i =>
                    Settings.ApiGroupNames == null ||
                    Settings.ApiGroupNames.Length == 0 ||
                    Settings.ApiGroupNames.Contains(i.GroupName))
                .SelectMany(g => g.Items);


            var descriptors = apiDescriptions.Where(d => d.ActionDescriptor is ControllerActionDescriptor)
                .Select(d => new ApiDescriptorInfo(d))
                .Where(i => i.ControllerType != null)
                .GroupBy(i => i.ControllerType)
                .Where(g => !ApiDescriptorInfo.HasSwaggerIgnoreAttribute(g.Key.GetTypeInfo()))
                .SelectMany(g => g)
                .Where(i => !ApiDescriptorInfo.HasSwaggerIgnoreAttribute(i.ActionDescriptor.MethodInfo))
                .ToList();

            return descriptors;
        }

        /// <summary>Generates the <see cref="SwaggerDocument"/> with services from the given service provider.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The document</returns>
        public async Task<SwaggerDocument> GenerateAsync(object serviceProvider)
        {
            var typedServiceProvider = (IServiceProvider)serviceProvider;

            var mvcOptions = typedServiceProvider.GetRequiredService<IOptions<MvcOptions>>();
            var settings = GetJsonSerializerSettings(typedServiceProvider);

            Settings.ApplySettings(settings, mvcOptions.Value);

            var apiDescriptionGroupCollectionProvider = typedServiceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
            return await GenerateAsync(apiDescriptionGroupCollectionProvider.ApiDescriptionGroups);
        }

        /// <summary>Loads the <see cref="GetJsonSerializerSettings"/> from the given service provider.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The settings.</returns>
        public static JsonSerializerSettings GetJsonSerializerSettings(IServiceProvider serviceProvider)
        {
            dynamic options;
            try
            {
                options = new Func<dynamic>(() => serviceProvider?.GetRequiredService(typeof(IOptions<MvcJsonOptions>)) as dynamic)();
            }
            catch
            {
                // Try load ASP.NET Core 3 options
                var optionsAssembly = Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.NewtonsoftJson"));
                var optionsType = typeof(IOptions<>).MakeGenericType(optionsAssembly.GetType("Microsoft.AspNetCore.Mvc.MvcNewtonsoftJsonOptions", true));
                options = serviceProvider?.GetRequiredService(optionsType) as dynamic;
            }

            var settings = (JsonSerializerSettings)options?.Value?.SerializerSettings ?? JsonConvert.DefaultSettings?.Invoke();
            return settings;
        }

        private Task<bool> GenerateForControllerAsync(SwaggerDocument document, Type controllerType,
            IEnumerable<ApiDescriptorInfo> controllerApiDescriptionGroup,
            SwaggerGenerator swaggerGenerator, SwaggerSchemaResolver schemaResolver)
        {
            var operations = new List<Tuple<SwaggerOperationDescription, ApiDescriptorInfo>>();
            foreach (var item in controllerApiDescriptionGroup)
            {
                var path = item.ApiDescription.RelativePath;
                if (!path.StartsWith("/", StringComparison.Ordinal))
                    path = "/" + path;

                var httpMethod = item.ApiDescription.HttpMethod?.ToLowerInvariant() ?? SwaggerOperationMethod.Get;

                var operationDescription = new SwaggerOperationDescription
                {
                    Path = path,
                    Method = httpMethod,
                    Operation = new SwaggerOperation
                    {
                        IsDeprecated = item.ActionDescriptor.MethodInfo.GetCustomAttribute<ObsoleteAttribute>() != null,
                        OperationId = GetOperationId(document, item.ActionDescriptor, item.ActionDescriptor.MethodInfo)
                    }
                };

                operations.Add(new Tuple<SwaggerOperationDescription, ApiDescriptorInfo>(operationDescription, item));
            }

            return AddOperationDescriptionsToDocumentAsync(document, controllerType, operations, swaggerGenerator, schemaResolver);
        }

        private async Task<bool> AddOperationDescriptionsToDocumentAsync(SwaggerDocument document, Type controllerType,
            List<Tuple<SwaggerOperationDescription, ApiDescriptorInfo>> operations,
            SwaggerGenerator swaggerGenerator, SwaggerSchemaResolver schemaResolver)
        {
            var addedOperations = 0;
            var allOperation = operations.Select(t => t.Item1).ToList();

            foreach (var tuple in operations)
            {
                var operation = tuple.Item1;
                var descriptorInfo = tuple.Item2;

                var consumes = descriptorInfo.GetSupportedRequestFormats();
                if (!consumes.SequenceEqual(document.Consumes))
                {
                    operation.Operation.Consumes = consumes.ToList();
                }

                var produces = descriptorInfo.GetSupportedResponseFormats();
                if (!produces.SequenceEqual(document.Produces))
                {
                    operation.Operation.Produces = produces.ToList();
                }

                var addOperation = await RunOperationProcessorsAsync(document, descriptorInfo.ApiDescription, controllerType, descriptorInfo.ActionDescriptor.MethodInfo, operation, allOperation, swaggerGenerator, schemaResolver).ConfigureAwait(false);
                if (addOperation)
                {
                    var path = operation.Path.Replace("//", "/");

                    if (!document.Paths.ContainsKey(path))
                        document.Paths[path] = new SwaggerPathItem();

                    if (document.Paths[path].ContainsKey(operation.Method))
                    {
                        throw new InvalidOperationException($"The method '{operation.Method}' on path '{path}' is registered multiple times.");
                    }

                    document.Paths[path][operation.Method] = operation.Operation;
                    addedOperations++;
                }
            }

            return addedOperations > 0;
        }

        private void AddGlobalConsumesProducesToDocument(SwaggerDocument document, List<ApiDescriptorInfo> descriptorInfos)
        {
            document.Consumes = descriptorInfos
                .SelectMany(i => i.ApiDescription.SupportedRequestFormats)
                .Select(f => f.MediaType)
                .Distinct()
                .DefaultIfEmpty(DefaultRequestContentType)
                .OrderBy(s => s)
                .ToList();

            document.Produces = descriptorInfos
                .SelectMany(i => i.ApiDescription.SupportedResponseTypes)
                .SelectMany(r => r.ApiResponseFormats)
                .Select(f => f.MediaType)
                .Distinct()
                .DefaultIfEmpty(DefaultResponseContentType)
                .OrderBy(s => s)
                .ToList();
        }

        private async Task<SwaggerDocument> CreateDocumentAsync()
        {
            var document = !string.IsNullOrEmpty(Settings.DocumentTemplate) ?
                await SwaggerDocument.FromJsonAsync(Settings.DocumentTemplate).ConfigureAwait(false) :
                new SwaggerDocument();

            document.Generator = $"NSwag v{SwaggerDocument.ToolchainVersion} (NJsonSchema v{JsonSchema4.ToolchainVersion})";
            document.SchemaType = Settings.SchemaType;

            if (document.Info == null)
                document.Info = new SwaggerInfo();

            if (string.IsNullOrEmpty(Settings.DocumentTemplate))
            {
                if (!string.IsNullOrEmpty(Settings.Title))
                    document.Info.Title = Settings.Title;
                if (!string.IsNullOrEmpty(Settings.Description))
                    document.Info.Description = Settings.Description;
                if (!string.IsNullOrEmpty(Settings.Version))
                    document.Info.Version = Settings.Version;
            }

            return document;
        }

        private async Task<bool> RunOperationProcessorsAsync(SwaggerDocument document, ApiDescription apiDescription, Type controllerType, MethodInfo methodInfo, SwaggerOperationDescription operationDescription, List<SwaggerOperationDescription> allOperations, SwaggerGenerator swaggerGenerator, SwaggerSchemaResolver schemaResolver)
        {
            // 1. Run from settings
            var operationProcessorContext = new AspNetCoreOperationProcessorContext(document, operationDescription, controllerType, methodInfo, swaggerGenerator, _schemaGenerator, schemaResolver, Settings, allOperations)
            {
                ApiDescription = apiDescription,
            };

            foreach (var operationProcessor in Settings.OperationProcessors)
            {
                if (await operationProcessor.ProcessAsync(operationProcessorContext).ConfigureAwait(false) == false)
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

                if (await operationProcessor.ProcessAsync(operationProcessorContext) == false)
                    return false;
            }

            return true;
        }

        private string GetOperationId(SwaggerDocument document, ControllerActionDescriptor actionDescriptor, MethodInfo method)
        {
            string operationId;

            dynamic swaggerOperationAttribute = method.GetCustomAttributes().FirstOrDefault(a => a.GetType().Name == "SwaggerOperationAttribute");
            if (swaggerOperationAttribute != null && !string.IsNullOrEmpty(swaggerOperationAttribute.OperationId))
                operationId = swaggerOperationAttribute.OperationId;
            else
            {
                operationId = actionDescriptor.ControllerName + "_" + GetActionName(actionDescriptor.ActionName);
            }

            var number = 1;
            while (document.Operations.Any(o => o.Operation.OperationId == operationId + (number > 1 ? "_" + number : string.Empty)))
                number++;

            return operationId + (number > 1 ? number.ToString() : string.Empty);
        }

        private static string GetActionName(string actionName)
        {
            if (actionName.EndsWith("Async"))
                actionName = actionName.Substring(0, actionName.Length - 5);

            return actionName;
        }

        private class ApiDescriptorInfo
        {
            public ApiDescriptorInfo(ApiDescription apiDescription)
            {
                this.ApiDescription = apiDescription;
                this.ActionDescriptor = (ControllerActionDescriptor)apiDescription.ActionDescriptor;
                this.ControllerType = this.ActionDescriptor?.ControllerTypeInfo.AsType();
            }

            public ApiDescription ApiDescription { get; }

            public ControllerActionDescriptor ActionDescriptor { get; }

            public Type ControllerType { get; }

            public IList<string> GetSupportedRequestFormats()
            {
                return this.ApiDescription.SupportedRequestFormats
                        .Select(f => f.MediaType)
                        .Distinct()
                        .DefaultIfEmpty(DefaultRequestContentType)
                        .OrderBy(s => s)
                        .ToList();
            }

            public IList<string> GetSupportedResponseFormats()
            {
                return this.ApiDescription.SupportedResponseTypes
                        .SelectMany(r => r.ApiResponseFormats)
                        .Select(f => f.MediaType)
                        .Distinct()
                        .DefaultIfEmpty(DefaultResponseContentType)
                        .OrderBy(s => s)
                        .ToList();
            }

            public static bool HasSwaggerIgnoreAttribute(ICustomAttributeProvider type)
            {
                return type.GetCustomAttributes(true).Any(a => a.GetType().Name == "SwaggerIgnoreAttribute");
            }
        }
    }
}
