using Xunit;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class QueryParameterTests
    {
        [Fact]
        public void When_query_parameter_is_set_to_explode_and_style_is_form_object_parameters_are_expanded()
        {
            var spec = @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""version"": ""1.0.0"",
    ""title"": ""Query params tests""
  },
  ""servers"": [
    {
      ""url"": ""http://localhost:8080""
    }
  ],
  ""paths"": {
    ""/settings"": {
      ""get"": {
        ""summary"": ""List all settings"",
        ""operationId"": ""listSettings"",
        ""parameters"": [
          {
            ""name"": ""paging"",
            ""in"": ""query"",
            ""description"": ""list setting filter"",
            ""required"": false,
            ""style"": ""form"",
            ""explode"": true,
            ""schema"": {
              ""$ref"": ""#/components/schemas/Paging""
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""A paged array of settings""
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""Paging"": {
        ""type"": ""object"",
        ""properties"": {
          ""page"": {
            ""type"": ""integer"",
            ""format"": ""int32""
          },
          ""limit"": {
            ""type"": ""integer"",
            ""format"": ""int32""
          }
        }
      }
    }
  }
}
";

            var document = OpenApiDocument.FromJsonAsync(spec).Result;

            //// Act
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = generator.GenerateFile();

            //// Assert
            Assert.Contains(
                "urlBuilder_.Append(System.Uri.EscapeDataString(\"page\") + \"=\").Append(System.Uri.EscapeDataString(ConvertToString(paging.Page, System.Globalization.CultureInfo.InvariantCulture))).Append(\"&\");",
                code);
            Assert.Contains(
                "urlBuilder_.Append(System.Uri.EscapeDataString(\"limit\") + \"=\").Append(System.Uri.EscapeDataString(ConvertToString(paging.Limit, System.Globalization.CultureInfo.InvariantCulture))).Append(\"&\");",
                code);
        }
    }
}