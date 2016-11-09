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
            public object GetPerson(bool @override = false)
            {
                return null; 
            }
        }

        [TestMethod]
        public void When_ConfigurationClass_is_set_then_correct_ctor_is_generated()
        {
            //// Arrange
            var swaggerGenerator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = swaggerGenerator.GenerateForController<FooController>();

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
        public void When_UseHttpRequestMessageCreationMethod_is_set_then_CreateRequestMessage_is_generated()
        {
            //// Arrange
            var swaggerGenerator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = swaggerGenerator.GenerateForController<FooController>();

            var generator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
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

        [TestMethod]
        public void When_parameter_name_is_reserved_keyword_then_it_is_appended_with_at()
        {
            //// Arrange
            var swaggerGenerator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = swaggerGenerator.GenerateForController<FooController>();

            var generator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());

            //// Act
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("Task<object> GetPersonAsync(bool? @override, "));
        }
    }
}