using System.Net;
using System.Linq;
using NJsonSchema;
using System.Web.Http;
using NSwag.Annotations;
using System.Threading.Tasks;
using NJsonSchema.Generation;
using NJsonSchema.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSwag.Generation.WebApi.Nullable.Tests.Nullability
{
    [TestClass]
    public class ResponseNullabilityTests
    {
        public class NotNullResponseTestController : ApiController
        {
            [Route("Abc")]
            [SwaggerResponse(HttpStatusCode.OK, typeof(object))]
            public object Abc()
            {
                return null;
            }

            [Route("Def")]
            [SwaggerResponse(HttpStatusCode.OK, typeof(object), IsNullable = false)]
            public object Def()
            {
                return null;
            }

            [Route("Ghi")]
            [return: NotNull]
            public object Ghi()
            {
                return null;
            }

        }

        [TestMethod]
        public async Task When_response_is_not_nullable_then_nullable_is_false_in_spec()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.Null
            });

            // Act
            var document = await generator.GenerateForControllerAsync<NotNullResponseTestController>();

            // Assert
            var operationAbc = document.Operations.Single(o => o.Path.Contains("Abc"));
            var responseAbc = operationAbc.Operation.ActualResponses.First().Value;

            var operationDef = document.Operations.Single(o => o.Path.Contains("Def"));
            var responseDef = operationDef.Operation.ActualResponses.First().Value;

            var operationGhi = document.Operations.Single(o => o.Path.Contains("Ghi"));
            var responseGhi = operationGhi.Operation.ActualResponses.First().Value;

            Assert.IsTrue(responseAbc.IsNullable(SchemaType.Swagger2));
            Assert.IsFalse(responseDef.IsNullable(SchemaType.Swagger2));
            Assert.IsFalse(responseGhi.IsNullable(SchemaType.Swagger2));
        }

        public class NullableReferenceTypesResponseTestController : ApiController
        {
            [Route("Abc")]        
            public Task<string?> Abc()
            {
                return Task.FromResult<string?>(null);
            }

            [Route("Def")]        
            public Task<object> Def()
            {
                return Task.FromResult(new object());
            }
        }

        [TestMethod]
        public async Task When_response_is_not_nullable_then_nullable_is_false_in_spec_respecting_nullable_reference_types()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.Null
            });

            // Act
            var document = await generator.GenerateForControllerAsync<NullableReferenceTypesResponseTestController>();

            // Assert
            var operationAbc = document.Operations.Single(o => o.Path.Contains("Abc"));
            var responseAbc = operationAbc.Operation.ActualResponses.First().Value;

            var operationDef = document.Operations.Single(o => o.Path.Contains("Def"));
            var responseDef = operationDef.Operation.ActualResponses.First().Value;

            Assert.IsTrue(responseAbc.IsNullable(SchemaType.Swagger2));
            Assert.IsFalse(responseDef.IsNullable(SchemaType.Swagger2));
            
            Assert.IsTrue(responseAbc.IsNullable(SchemaType.OpenApi3));
            Assert.IsFalse(responseDef.IsNullable(SchemaType.OpenApi3));
        }
    }
}