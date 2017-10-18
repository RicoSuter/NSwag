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
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using NSwag.Annotations;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests
{
    public class AspNetCoreToSwaggerGenerationTests
    {
        [Fact]
        public async Task When_generating_swagger_all_apidescriptions_are_discovered()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = GetApiDescriptionGroups(typeof(TestController));

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);

            //// Assert
            var operationDescription = Assert.Single(document.Operations);
            Assert.Equal("/test", operationDescription.Path);
            Assert.Equal(SwaggerOperationMethod.Get, operationDescription.Method);

            var operation = operationDescription.Operation;
            Assert.Equal("Test_FindModel", operation.OperationId);

            var parameter = Assert.Single(operation.Parameters);
            Assert.Equal("id", parameter.Name);
            Assert.Equal(SwaggerParameterKind.Path, parameter.Kind);
            Assert.True(parameter.IsRequired);
            Assert.Equal(NJsonSchema.JsonObjectType.Integer, parameter.Type);

            Assert.Single(operation.Responses);
            var response = operation.Responses["200"];
            var definition = document.Definitions.First(f => f.Value == response.ActualResponseSchema);
            Assert.Equal(nameof(TestModel), definition.Key);
        }

        [Fact]
        public async Task When_generating_swagger_all_apidescriptions_are_discovered_for_2_1_applications()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = Get2_1_ApiDescriptionGroups(typeof(TestController));

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);

            //// Assert
            var operationDescription = Assert.Single(document.Operations);
            Assert.Equal("/test", operationDescription.Path);
            Assert.Equal(SwaggerOperationMethod.Get, operationDescription.Method);

            var operation = operationDescription.Operation;
            Assert.Equal("Test_FindModel", operation.OperationId);

            var parameter = Assert.Single(operation.Parameters);
            Assert.Equal("id", parameter.Name);
            Assert.Equal(SwaggerParameterKind.Path, parameter.Kind);
            Assert.True(parameter.IsRequired);
            Assert.Equal(NJsonSchema.JsonObjectType.Integer, parameter.Type);

            Assert.Equal(2, operation.Responses.Count);
            var response = operation.Responses["200"];
            var definition = document.Definitions.First(f => f.Value == response.ActualResponseSchema);
            Assert.Equal(nameof(TestModel), definition.Key);

            response = operation.Responses["default"];
            definition = document.Definitions.First(f => f.Value == response.ActualResponseSchema);
            Assert.Equal(nameof(ProblemDetails), definition.Key);
        }

        [Fact]
        public async Task ControllersWithSwaggerIgnoreAttribute_AreIgnored()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = GetApiDescriptionGroups(typeof(ControllerWithSwaggerIgnoreAttribute));

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);

            //// Assert
            Assert.Empty(document.Operations);
        }

        [Fact]
        public async Task ActionsWithSwaggerIgnoreAttribute_AreIgnored()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = GetApiDescriptionGroups(typeof(ActionWithSwaggerIgnoreAttribute));

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);

            //// Assert
            var operationDescription = Assert.Single(document.Operations);
            Assert.Equal("/test1", operationDescription.Path);
            Assert.Equal(SwaggerOperationMethod.Get, operationDescription.Method);
        }

        [Fact]
        public async Task ParametersWithSwaggerIgnoreAttribute_AreIgnored()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = GetApiDescriptionGroups(typeof(ParameterWithSwaggerIgnoreAttribute));

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);

            //// Assert
            var operationDescription = Assert.Single(document.Operations);
            Assert.Equal("/{id}", operationDescription.Path);
            Assert.Equal(SwaggerOperationMethod.Post, operationDescription.Method);

            var parameter = Assert.Single(operationDescription.Operation.Parameters);
            Assert.Equal("id", parameter.Name);
            Assert.Equal(SwaggerParameterKind.Path, parameter.Kind);
        }

        [Fact]
        public async Task SwaggerOperationMethods_AreParsed()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = GetApiDescriptionGroups(typeof(HttpMethodsController));

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);

            //// Assert
            Assert.Collection(
                document.Operations.OrderBy(o => o.Method.ToString()),
                operation =>
                {
                    Assert.Equal(SwaggerOperationMethod.Delete, operation.Method);
                    Assert.Equal("HttpMethods_HttpDelete", operation.Operation.OperationId);
                },
                operation =>
                {
                    Assert.Equal(SwaggerOperationMethod.Head, operation.Method);
                    Assert.Equal("HttpMethods_HttpHead", operation.Operation.OperationId);
                },
                operation =>
                {
                    Assert.Equal(SwaggerOperationMethod.Options, operation.Method);
                    Assert.Equal("HttpMethods_HttpOptions", operation.Operation.OperationId);
                },
                operation =>
                {
                    Assert.Equal(SwaggerOperationMethod.Patch, operation.Method);
                    Assert.Equal("HttpMethods_HttpPatch", operation.Operation.OperationId);
                });
        }

        [Fact]
        public async Task SwaggerOperationAttribute_AreUsedToCalculateOperationId_IfPresent()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = GetApiDescriptionGroups(typeof(ActionWithSwaggerOperationAttribute));

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);

            //// Assert
            var operation = Assert.Single(document.Operations);
            Assert.Equal("CustomOperationId", operation.Operation.OperationId);
        }

        [Fact]
        public async Task SwaggerOperationProcessorAttributesOnControllerTypes_AreDiscoveredAndExecuted()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = GetApiDescriptionGroups(typeof(ControllerWithSwaggerOperationProcessor));

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);

            //// Assert
            Assert.Equal("Hello from controller", document.Info.Title);
        }

        [Fact]
        public async Task SwaggerOperationProcessorAttributesOnActions_AreDiscoveredAndExecuted()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = GetApiDescriptionGroups(typeof(ActionWithSwaggerOperationProcessor));

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);

            //// Assert
            Assert.Equal("Hello from action", document.Info.Title);
        }

        [Fact]
        public async Task SwaggerResponseAttributesOnControllersAreDiscovered()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = GetApiDescriptionGroups(typeof(ControllerWithSwaggerResponseTypeAttribute));

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);

            //// Assert
            var operation = Assert.Single(document.Operations);
            Assert.Single(operation.Operation.Responses);
            var response = operation.Operation.Responses["202"];
            var definition = document.Definitions.First(f => f.Value == response.ActualResponseSchema);
            Assert.Equal(nameof(TestModel), definition.Key);
        }

        [Fact]
        public async Task SwaggerResponseAttributesOnActionsAreDiscovered()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = GetApiDescriptionGroups(typeof(ActionWithSwaggerResponseAttribute));

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);

            //// Assert
            var operation = Assert.Single(document.Operations);
            Assert.Single(operation.Operation.Responses);
            var response = operation.Operation.Responses["201"];
            var definition = document.Definitions.First(f => f.Value == response.ActualResponseSchema);
            Assert.Equal(nameof(TestModel), definition.Key);
        }

        #region Controllers

        private class TestController : ControllerBase
        {
            [HttpGet("/test")]
            public Task<TestModel> FindModel([FromRoute] int id) => null;
        }

        [SwaggerIgnore]
        private class ControllerWithSwaggerIgnoreAttribute
        {
            [HttpGet("/test")]
            public Task<TestModel> FindModel([FromRoute] int id) => null;
        }

        private class ActionWithSwaggerIgnoreAttribute
        {
            [HttpGet("/test1")]
            public Task<TestModel> PublicApi() => null;

            [SwaggerIgnore]
            [HttpGet("/test2")]
            public Task<TestModel> SecretApi([FromRoute] int id) => null;
        }

        private class ParameterWithSwaggerIgnoreAttribute
        {
            [HttpPost("{id}/{name?}")]
            public Task<TestModel> GetModel([FromRoute] int id, [FromRoute] [SwaggerIgnore] string name) => null;
        }

        [Route("[controller]/[action]")]
        public class HttpMethodsController
        {
            [AcceptVerbs("Delete")]
            public void HttpDelete() { }

            [AcceptVerbs("pAtch")]
            public void HttpPatch() { }

            [HttpHead]
            public void HttpHead() { }

            [AcceptVerbs("OPTIONS")]
            public void HttpOptions() { }
        }

        public class ActionWithSwaggerOperationAttribute
        {
            [HttpGet("/test")]
            [SwaggerOperation("CustomOperationId")]
            public void TestAction() { }
        }

        [SwaggerOperationProcessor(typeof(TestSwaggerOperationProcessor), "Hello from controller")]
        public class ControllerWithSwaggerOperationProcessor
        {
            [HttpGet("/test")]
            public void TestAction() { }
        }

        public class ActionWithSwaggerOperationProcessor
        {
            [SwaggerOperationProcessor(typeof(TestSwaggerOperationProcessor), "Hello from action")]
            [HttpGet("/test")]
            public void TestAction() { }
        }

        private class TestSwaggerOperationProcessor : IOperationProcessor
        {
            public TestSwaggerOperationProcessor(string value)
            {
                Value = value;
            }

            public string Value { get; }

            public Task<bool> ProcessAsync(OperationProcessorContext context)
            {
                context.Document.Info.Title = Value;
                return Task.FromResult(true);
            }
        }

        [SwaggerResponse(202, typeof(TestModel))]
        private class ControllerWithSwaggerResponseTypeAttribute
        {
            [HttpPut("/test1")]
            public IActionResult UpdateModel(int id) => null;
        }

        private class ActionWithSwaggerResponseAttribute
        {
            [SwaggerResponse(201, typeof(TestModel))]
            [HttpPut("/test1")]
            public IActionResult CreateModel(int id) => null;
        }

        private class TestModel { }

        #endregion

        #region ApiDescription generation

        private static ApiDescriptionGroupCollection GetApiDescriptionGroups(Type controllerType)
        {
            var options = Mock.Of<IOptions<MvcOptions>>(m => m.Value == new MvcOptions());
            var applicationPartManager = new ApplicationPartManager();
            applicationPartManager.FeatureProviders.Add(new TestControllerFeatureProvider(controllerType));

            var actionDescriptorProvider = new ControllerActionDescriptorProvider(
                applicationPartManager,
                new IApplicationModelProvider[] { new DefaultApplicationModelProvider(options), new MakeApiExplorerVisibleProvider(), },
                options);

            var actionDescriptorProviderContext = new ActionDescriptorProviderContext();
            actionDescriptorProvider.OnProvidersExecuting(actionDescriptorProviderContext);

            var apiDescriptionProvider = new DefaultApiDescriptionProvider(
                options,
                Mock.Of<IInlineConstraintResolver>(),
                new EmptyModelMetadataProvider());
            var apiDescriptionProviderContext = new ApiDescriptionProviderContext(actionDescriptorProviderContext.Results.ToArray());
            apiDescriptionProvider.OnProvidersExecuting(apiDescriptionProviderContext);
            var groups = apiDescriptionProviderContext.Results.GroupBy(a => a.GroupName)
                .Select(g => new ApiDescriptionGroup(g.Key, g.ToArray()))
                .ToArray();

            return new ApiDescriptionGroupCollection(groups, version: 1);
        }

        private static ApiDescriptionGroupCollection Get2_1_ApiDescriptionGroups(Type controllerType)
        {
            var apiDescriptionGroup = GetApiDescriptionGroups(controllerType);
            foreach (var apiDescription in apiDescriptionGroup.Items.SelectMany(c => c.Items))
            {
                apiDescription.SupportedResponseTypes.Add(new ApiResponseType2_1
                {
                    StatusCode = 0,
                    Type = typeof(ProblemDetails),
                    IsDefaultResponse = true,
                });
            }

            return apiDescriptionGroup;
        }

        private class TestControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
        {
            private readonly Type _controllerType;

            public TestControllerFeatureProvider(Type controllerType)
            {
                _controllerType = controllerType;
            }

            public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
            {
                feature.Controllers.Add(_controllerType.GetTypeInfo());
            }
        }

        private class MakeApiExplorerVisibleProvider : IApplicationModelProvider
        {
            public int Order => 0;

            public void OnProvidersExecuted(ApplicationModelProviderContext context)
            {
            }

            public void OnProvidersExecuting(ApplicationModelProviderContext context)
            {
                context.Result.ApiExplorer.IsVisible = true;
            }
        }

        private class ProblemDetails { }

        private class ApiResponseType2_1 : ApiResponseType
        {
            public bool IsDefaultResponse { get; set; }
        }
        #endregion
    }
}
