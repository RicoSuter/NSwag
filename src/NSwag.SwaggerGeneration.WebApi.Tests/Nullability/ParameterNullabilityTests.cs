using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NJsonSchema.Annotations;

namespace NSwag.SwaggerGeneration.WebApi.Tests.Nullability
{
    [TestClass]
    public class ParameterNullabilityTests
    {
        public class NotNullParameterTestController : Controller
        {
            [Route("Abc")]
            public object Abc([NotNull]object foo, object bar)
            {
                return null;
            }
        }

        [TestMethod]
        public async Task When_parameter_has_NotNullAttribute_then_it_is_not_nullable()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = await generator.GenerateForControllerAsync<NotNullParameterTestController>();
            var json = document.ToJson();

            //// Assert 
            Assert.IsFalse(document.Operations.First().Operation.Parameters[0].IsNullable(NullHandling.Swagger));
            Assert.IsTrue(document.Operations.First().Operation.Parameters[1].IsNullable(NullHandling.Swagger));
        }
    }
}