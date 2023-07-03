using System.Linq;
using System.Threading.Tasks;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters;
using Xunit;

namespace NSwag.Generation.AspNetCore.Tests.Parameters
{
    public class BodyParametersTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_primitive_body_parameter_has_no_default_value_then_it_is_required()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(BodyParametersController));

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "BodyParameters_RequiredPrimitive").Operation;

            Assert.True(operation.ActualParameters.First().IsRequired);
        }

        [Fact(
#if !NET7_0_OR_GREATER
            Skip = "Failing before .Net 6"
#endif
        )]
        public async Task When_primitive_body_parameter_has_default_value_then_it_is_optional()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(BodyParametersController));

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "BodyParameters_RequiredPrimitiveWithDefault").Operation;

            Assert.False(operation.ActualParameters.First().IsRequired);
        }
        
        [Fact(
#if NET7_0_OR_GREATER
            Skip = "Wrong in .Net 7"
#endif
        )]
        public async Task When_primitive_body_parameter_has_default_value_then_it_is_required_before_net7()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(BodyParametersController));

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "BodyParameters_RequiredPrimitiveWithDefault").Operation;

            Assert.True(operation.ActualParameters.First().IsRequired);
        }

        [Fact]
        public async Task When_complex_body_parameter_has_no_default_value_then_it_is_required()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(BodyParametersController));

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "BodyParameters_RequiredComplex").Operation;

            Assert.True(operation.ActualParameters.First().IsRequired);
        }

        [Fact(
#if !NET7_0_OR_GREATER
            Skip = "Failing before .Net 6"
#endif
        )]
        public async Task When_complex_body_parameter_has_default_value_then_it_is_optional()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(BodyParametersController));

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "BodyParameters_RequiredComplexWithDefault").Operation;

            Assert.False(operation.ActualParameters.First().IsRequired);
        }
        
        [Fact(
#if NET7_0_OR_GREATER
            Skip = "Wrong in .Net 7"
#endif
        )]
        public async Task When_complex_body_parameter_has_default_value_then_it_is_required_before_net7()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(BodyParametersController));

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "BodyParameters_RequiredComplexWithDefault").Operation;

            Assert.True(operation.ActualParameters.First().IsRequired);
        }
    }
}
