using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NSwag.Generation.WebApi.Tests.OperationProcessors
{
    [TestClass]
    public class SwaggerQueryParametersOperationParameterProcessorTests
    {
        private class ComplexType
        {
            public int? IdInComplexType { get; set; }
        }

        private class TestController
        {
            [HttpGet]
            [Route("getByPrimitive")]
            public void GetByNullablePrimitiveType([FromUri] int? primitiveId)
            {
            }

            [HttpGet]
            [Route("getByReference")]
            public void GetByReferenceTypeWithNullablePrimitiveType([FromUri] ComplexType complexType)
            {
            }
        }

        private WebApiOpenApiDocumentGenerator GetOpenApi3Generator()
        {
            WebApiOpenApiDocumentGeneratorSettings settings = new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaType = SchemaType.Swagger2
            };
            WebApiOpenApiDocumentGenerator generator = new WebApiOpenApiDocumentGenerator(settings);

            return generator;
        }

        [TestMethod]
        public async Task When_nullable_primitive_query_parameter_exists_then_parameter_IsNullableRaw_is_true()
        {
            //// Arrange
            WebApiOpenApiDocumentGenerator generator = this.GetOpenApi3Generator();

            //// Act
            OpenApiDocument document = await generator.GenerateForControllerAsync<TestController>();

            OpenApiOperationDescription operationDescription = document.Operations.Single(o =>
                o.Operation.OperationId.EndsWith(nameof(TestController.GetByNullablePrimitiveType)));

            OpenApiOperation operation = operationDescription.Operation;

            //// Assert
            Assert.IsNotNull(operation.Parameters.Single().IsNullableRaw);
            Assert.IsTrue(operation.Parameters.Single().IsNullableRaw.Value);
        }

        [TestMethod]
        public async Task When_complex_type_with_nullable_primitive_query_parameter_exists_then_parameter_IsNullableRaw_is_true()
        {
            //// Arrange
            WebApiOpenApiDocumentGenerator generator = this.GetOpenApi3Generator();

            //// Act
            OpenApiDocument document = await generator.GenerateForControllerAsync<TestController>();

            OpenApiOperationDescription operationDescription = document.Operations.Single(o =>
                o.Operation.OperationId.EndsWith(nameof(TestController.GetByReferenceTypeWithNullablePrimitiveType)));

            OpenApiOperation operation = operationDescription.Operation;

            //// Assert
            Assert.IsNotNull(operation.Parameters.Single().IsNullableRaw);
            Assert.IsTrue(operation.Parameters.Single().IsNullableRaw.Value);
        }
    }
}
