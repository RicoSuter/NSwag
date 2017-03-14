using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    [TestClass]
    public class CSharpClientSettingsTests
    {
        public class FooController : ApiController
        {
            public object GetPerson(bool @override = false)
            {
                return null;
            }
        }

        [TestMethod]
        public async Task When_ConfigurationClass_is_set_then_correct_ctor_is_generated()
        {
            //// Arrange
            var swaggerGenerator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();

            var generator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                ConfigurationClass = "MyConfig",
                ClientBaseClass = "MyBaseClass"
            });

            //// Act
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("public FooClient(MyConfig configuration) : base(configuration)"));
        }

        [TestMethod]
        public async Task When_UseHttpRequestMessageCreationMethod_is_set_then_CreateRequestMessage_is_generated()
        {
            //// Arrange
            var swaggerGenerator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();

            var generator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                ConfigurationClass = "MyConfig",
                ClientBaseClass = "MyBaseClass",
                UseHttpRequestMessageCreationMethod = true
            });

            //// Act
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("var request_ = await CreateHttpRequestMessageAsync(cancellationToken).ConfigureAwait(false)"));
        }

        [TestMethod]
        public async Task When_parameter_name_is_reserved_keyword_then_it_is_appended_with_at()
        {
            //// Arrange
            var swaggerGenerator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();

            var generator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());

            //// Act
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("Task<object> GetPersonAsync(bool? @override, "));
        }

        [TestMethod]
        public async Task When_code_is_generated_then_by_default_the_system_httpclient_is_used()
        {
            //// Arrange
            var swaggerGenerator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();

            var generator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());

            //// Act
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("var client_ = new System.Net.Http.HttpClient();"));
        }

        [TestMethod]
        public async Task When_custom_http_client_type_is_specified_then_an_instance_of_that_type_is_used()
        {
            //// Arrange
            var swaggerGenerator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();

            var generator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                HttpClientType = "CustomNamespace.CustomHttpClient"
            });

            //// Act
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("var client_ = new CustomNamespace.CustomHttpClient();"));
        }
    }
}