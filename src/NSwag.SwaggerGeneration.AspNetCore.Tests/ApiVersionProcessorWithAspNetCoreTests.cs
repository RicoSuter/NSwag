using System.Linq;
using System.Threading.Tasks;
using NSwag.SwaggerGeneration.Processors;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests
{
    public class ApiVersionProcessorWithAspNetCoreTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_generating_v1_then_only_v1_operations_are_included()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();
            settings.OperationProcessors.TryGet<ApiVersionProcessor>().IncludedVersions = new[] { "1" };

            // Act
            var document = await GenerateDocumentAsync(settings);
            var json = document.ToJson();

            // Assert
            var operations = GetControllerOperations(document, "VersionedValues")
                .Concat(GetControllerOperations(document, "VersionedV3Values"))
                .ToArray();

            Assert.Equal(4, operations.Count());
            Assert.True(operations.All(o => o.Path.Contains("/v1/")));

            // VersionedIgnoredValues tag should not be in json document
            Assert.Equal(1, document.Tags.Count);
        }

        [Fact]
        public async Task When_generating_v2_then_only_v2_operations_are_included()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();
            settings.OperationProcessors.TryGet<ApiVersionProcessor>().IncludedVersions = new[] { "2" };

            // Act
            var document = await GenerateDocumentAsync(settings);

            // Assert
            var operations = GetControllerOperations(document, "VersionedValues")
                .Concat(GetControllerOperations(document, "VersionedV3Values"))
                .ToArray();

            Assert.Equal(2, operations.Count());
            Assert.True(operations.All(o => o.Path.Contains("/v2/")));

            // VersionedIgnoredValues tag should not be in json document
            Assert.Equal(1, document.Tags.Count);
        }

        [Fact]
        public async Task When_generating_v3_then_only_v3_operations_are_included()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();
            settings.OperationProcessors.TryGet<ApiVersionProcessor>().IncludedVersions = new[] { "3" };

            // Act
            var document = await GenerateDocumentAsync(settings);

            // Assert
            var operations = GetControllerOperations(document, "VersionedValues")
                .Concat(GetControllerOperations(document, "VersionedV3Values"))
                .ToArray();

            Assert.Equal(5, operations.Count());
            Assert.True(operations.All(o => o.Path.Contains("/v3/")));

            // VersionedIgnoredValues tag should not be in json document
            Assert.Equal(1, document.Tags.Count);
        }

        [Fact]
        public async Task When_generating_versioned_controllers_then_version_path_parameter_is_not_present()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings{};
            settings.OperationProcessors.TryGet<ApiVersionProcessor>().IncludedVersions = new[] { "3" };

            // Act
            var document = await GenerateDocumentAsync(settings);
            var json = document.ToJson();

            // Assert
            var operation = GetControllerOperations(document, "VersionedValues")
                .Concat(GetControllerOperations(document, "VersionedV3Values"))
                .First();

            // check that implict unused path parameter is not in the spec
            Assert.DoesNotContain(operation.Operation.ActualParameters, p => p.Name == "version");
        }
    }
}
