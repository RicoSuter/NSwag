using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NSwag.CodeGeneration.CodeGenerators.CSharp;

namespace NSwag.CodeGeneration.Tests.ClientGeneration
{
    [TestClass]
    public class FormParameterTests
    {
        [TestMethod]
        public void When_form_parameters_are_defined_then_MultipartFormDataContent_is_generated()
        {
            //// Arrange
            var service = new SwaggerService();
            service.Paths["foo/bar"] = new SwaggerOperations
            {
                {
                    SwaggerOperationMethod.Post,
                    new SwaggerOperation
                    {
                        Parameters = new List<SwaggerParameter>
                        {
                            new SwaggerParameter
                            {
                                Name = "foo",
                                Kind = SwaggerParameterKind.FormData,
                                Type = JsonObjectType.String
                            },
                            new SwaggerParameter
                            {
                                Name = "bar",
                                IsRequired = true,
                                IsNullableRaw = false, 
                                Kind = SwaggerParameterKind.FormData,
                                Type = JsonObjectType.String
                            }
                        }
                    }
                }
            };

            //// Act
            var generator = new SwaggerToCSharpClientGenerator(service, new SwaggerToCSharpClientGeneratorSettings
            {

            });
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("new MultipartFormDataContent"));
            Assert.IsTrue(code.Contains("if (foo != null)"));
            Assert.IsTrue(code.Contains("throw new ArgumentNullException(\"bar\");"));
        }
    }
}