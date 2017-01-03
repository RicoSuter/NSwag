using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi.Integration
{
    [TestClass]
    public class SwashbuckleAnnotationsTests
    {
        // https://github.com/NSwag/NSwag/issues/494

        public class JsonDate
        {

        }

        public class LockedFlight
        {
            public string Name { get; set; }

            public JsonDate Date { get; set; }
        }

        public class MyController : ApiController
        {
            [HttpPost]

            [NSwag.Annotations.SwaggerResponse(200, typeof(LockedFlight), Description = "OK")]
            [NSwag.Annotations.SwaggerResponse(201, typeof(LockedFlight), Description = "Created")]
            [NSwag.Annotations.SwaggerOperation("AddFlightLock")]

            [Swashbuckle.Swagger.Annotations.SwaggerResponse(200, "OK", typeof(LockedFlight))]
            [Swashbuckle.Swagger.Annotations.SwaggerResponse(201, "Created", typeof(LockedFlight))]
            [Swashbuckle.Swagger.Annotations.SwaggerOperation("AddFlightLock")]

            [System.Web.Http.Description.ResponseType(typeof(LockedFlight))]
            public async Task<IHttpActionResult> Post(string id)
            {
                return null;
            }
        }

        [TestMethod]
        public async Task When_redundant_attributes_are_available_then_output_is_correct()
        {
            //// Arrange
            var settings = new WebApiToSwaggerGeneratorSettings
            {
                TypeMappers =
                {
                    new ObjectTypeMapper(typeof(JsonDate), new JsonSchema4
                    {
                        Type = JsonObjectType.String,
                        Format = "date"
                    })
                },
                AddMissingPathParameters = false,
                NullHandling = NullHandling.Swagger,
                GenerateKnownTypes = true,
                FlattenInheritanceHierarchy = true,
                DefaultEnumHandling = EnumHandling.String,
                IsAspNetCore = false,
            };
            var generator = new WebApiToSwaggerGenerator(settings);

            //// Act
            var document = await generator.GenerateForControllerAsync<MyController>();
            var json = document.ToJson();

            //// Assert
            Assert.IsTrue(json.Contains("\"$ref\": \"#/definitions/LockedFlight\""));
        }
    }
}
