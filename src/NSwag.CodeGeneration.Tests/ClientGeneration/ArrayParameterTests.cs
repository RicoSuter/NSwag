using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.ClientGenerators.CSharp;
using NSwag.CodeGeneration.ClientGenerators.TypeScript;

namespace NSwag.CodeGeneration.Tests.ClientGeneration
{
    [TestClass]
    public class ArrayParameterTests
    {
        [TestMethod]
        public void When_parameter_is_array_then_TypeScript_is_correct()
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
            var service = SwaggerService.FromJson(swagger);

            //// Act
            var settings = new SwaggerToTypeScriptGeneratorSettings { ClassName = "MyClass" };
            var generator = new SwaggerToTypeScriptGenerator(service, settings);
            var code = generator.GenerateFile();
            
            //// Assert
            Assert.IsTrue(code.Contains(@"elementId.forEach(item => { url += ""elementId="" + encodeURIComponent("""" + item) + ""&""; });"));
        }

        [TestMethod]
        public void When_parameter_is_array_then_CSharp_is_correct()
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
            var service = SwaggerService.FromJson(swagger);

            //// Act
            var settings = new SwaggerToCSharpGeneratorSettings { ClassName = "MyClass" };
            var generator = new SwaggerToCSharpGenerator(service, settings);
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains(@"foreach(var item in elementId) { url += string.Format(""elementId={0}&"", Uri.EscapeUriString(item.ToString())); }"));
        }
    }
}
