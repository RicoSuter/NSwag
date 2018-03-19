using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    [TestClass]
    public class ParameterTests
    {
        [TestMethod]
        public void When_parameters_have_same_name_then_they_are_renamed()
        {
            //// Arrange
            var document = new SwaggerDocument();
            document.Paths["foo"] = new SwaggerPathItem
            {
                {
                    SwaggerOperationMethod.Get, new SwaggerOperation
                    {
                        Parameters =
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


        [TestMethod]
        public void When_parent_parameters_have_same_kind_then_they_are_included()
        {
            //// Arrange
            var swagger = @"{
  ""swagger"" : ""2.0"",
  ""info"" : {
    ""version"" : ""1.0.2"",
    ""title"" : ""Test API""
  },
  ""host"" : ""localhost:8080"",
  ""basePath"" : ""/"",
  ""tags"" : [ {
    ""name"" : ""api""
  } ],
  ""schemes"" : [ ""http"" ],
  ""paths"" : {
     ""/removeElement"" : {

""parameters"": [
                {
                ""name"": ""SecureToken"",
                    ""in"": ""header"",
                    ""description"": ""cookie"",
                    ""required"": true,
                    ""type"": ""string""
                }
            ],

      ""delete"" : {
        ""tags"" : [ ""api"" ],
        ""summary"" : ""Removes elements"",
        ""description"" : ""Removes elements"",
        ""operationId"" : ""removeElement"",
        ""consumes"" : [ ""application/json"" ],
        ""produces"" : [ ""application/json"" ],
        ""parameters"" : [ {
          ""name"" : ""X-User"",
          ""in"" : ""header"",
          ""description"" : ""User identifier"",
          ""required"" : true,
          ""type"" : ""string""
        }, {
          ""name"" : ""elementId"",
          ""in"" : ""query"",
          ""description"" : ""The ids of existing elements that should be removed"",
          ""required"" : false,
          ""type"" : ""array"",
          ""items"" : {
            ""type"" : ""integer"",
            ""format"" : ""int64""
          },
        } ],
        ""responses"" : {
          ""default"" : {
            ""description"" : ""successful operation""
          }
        }
      }
    }
  },
    ""definitions"" : { }
}
";
            var document = SwaggerDocument.FromJsonAsync(swagger).Result;

            //// Act
            var generator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());
            var code = generator.GenerateFile();
            Assert.IsTrue(code.Contains("RemoveElementAsync(string x_User, System.Collections.Generic.IEnumerable<long> elementId, string secureToken)"));          
        }
    }
}
