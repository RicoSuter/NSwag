using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NJsonSchema.Annotations;
using NSwag.Annotations;

namespace NSwag.SwaggerGeneration.WebApi.Tests.Nullability
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
            /// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            /// Act
            var document = await generator.GenerateForControllerAsync<NotNullResponseTestController>();

            /// Assert
            var operationAbc = document.Operations.Single(o => o.Path.Contains("Abc"));
            var responseAbc = operationAbc.Operation.Responses.First().Value;

            var operationDef = document.Operations.Single(o => o.Path.Contains("Def"));
            var responseDef = operationDef.Operation.Responses.First().Value;

            var operationGhi = document.Operations.Single(o => o.Path.Contains("Ghi"));
            var responseGhi = operationGhi.Operation.Responses.First().Value;

            Assert.IsTrue(responseAbc.IsNullable(SchemaType.Swagger2));
            Assert.IsFalse(responseDef.IsNullable(SchemaType.Swagger2));
            Assert.IsFalse(responseGhi.IsNullable(SchemaType.Swagger2));
        }
    }
}