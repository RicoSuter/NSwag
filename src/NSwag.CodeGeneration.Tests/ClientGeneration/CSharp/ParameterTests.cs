using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.CodeGenerators.CSharp;

namespace NSwag.CodeGeneration.Tests.ClientGeneration.CSharp
{
    [TestClass]
    public class ParameterTests
    {
        [TestMethod]
        public void When_parameters_have_same_name_then_they_are_renamed()
        {
            //// Arrange
            var document = new SwaggerDocument();
            document.Paths["foo"] = new SwaggerOperations
            {
                {
                    SwaggerOperationMethod.Get, new SwaggerOperation
                    {
                        Parameters = new List<SwaggerParameter>
                        {
                            new SwaggerParameter
                            {
                                Kind = SwaggerParameterKind.Query,
                                Name = "foo"
                            },
                            new SwaggerParameter
                            {
                                Kind = SwaggerParameterKind.Header,
                                Name = "foo"
                            },
                        }
                    }
                }
            };

            //// Act
            var generator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("FooAsync(object fooQuery, object fooHeader, System.Threading.CancellationToken cancellationToken)"));
        }
    }
}
