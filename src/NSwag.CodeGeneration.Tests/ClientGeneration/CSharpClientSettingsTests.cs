using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.CodeGenerators.CSharp;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.ClientGeneration
{
    [TestClass]
    public class CSharpClientSettingsTests
    {
        public class FooController : ApiController
        {
            public object GetPerson()
            {
                return null; 
            }
        }

        [TestMethod]
        public void When_ConfigurationClass_is_set_then_correct_ctor_is_generated()
        {
            //// Arrange
            var swaggerGenerator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var service = swaggerGenerator.GenerateForController<FooController>();

            var generator = new SwaggerToCSharpClientGenerator(service, new SwaggerToCSharpClientGeneratorSettings
            {
                ConfigurationClass = "MyConfig", 
                ClientBaseClass = "MyBaseClass"
            });

            //// Act
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("public FooClient(string baseUrl, MyConfig configuration) : base(configuration)"));
        }

        [TestMethod]
        public void When_UseHttpRequestMessageCreationMethod_is_set_then_CreateRequestMessage_is_generated()
        {
            //// Arrange
            var swaggerGenerator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var service = swaggerGenerator.GenerateForController<FooController>();

            var generator = new SwaggerToCSharpClientGenerator(service, new SwaggerToCSharpClientGeneratorSettings
            {
                ConfigurationClass = "MyConfig",
                ClientBaseClass = "MyBaseClass",
                UseHttpRequestMessageCreationMethod = true
            });

            //// Act
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("var request_ = await CreateHttpRequestMessageAsync(cancellationToken).ConfigureAwait(false);"));
        }
    }
}