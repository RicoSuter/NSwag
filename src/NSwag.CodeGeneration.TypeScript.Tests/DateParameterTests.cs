using NJsonSchema.CodeGeneration.TypeScript;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class DateParameterTests
    {
        private const string DateParametersSpec = @"{
  ""openapi"": ""3.0.0"",
  ""info"": { ""title"": ""Dates"", ""version"": ""1"" },
  ""paths"": {
    ""/stats/{day}"": {
      ""get"": {
        ""operationId"": ""Stats_Get"",
        ""parameters"": [
          { ""name"": ""day"", ""in"": ""path"", ""required"": true, ""schema"": { ""type"": ""string"", ""format"": ""date"" } },
          { ""name"": ""startDate"", ""in"": ""query"", ""schema"": { ""type"": ""string"", ""format"": ""date"", ""nullable"": true } },
          { ""name"": ""days"", ""in"": ""query"", ""schema"": { ""type"": ""array"", ""items"": { ""type"": ""string"", ""format"": ""date"" } } },
          { ""name"": ""since"", ""in"": ""query"", ""schema"": { ""type"": ""string"", ""format"": ""date-time"", ""nullable"": true } }
        ],
        ""responses"": { ""200"": { ""description"": ""ok"" } }
      }
    }
  }
}";

        private const string DateParameterAndDatePropertySpec = @"{
  ""openapi"": ""3.0.0"",
  ""info"": { ""title"": ""Dates"", ""version"": ""1"" },
  ""paths"": {
    ""/stats"": {
      ""get"": {
        ""operationId"": ""Stats_Get"",
        ""parameters"": [
          { ""name"": ""startDate"", ""in"": ""query"", ""schema"": { ""type"": ""string"", ""format"": ""date"", ""nullable"": true } }
        ],
        ""responses"": {
          ""200"": { ""description"": ""ok"", ""content"": { ""application/json"": { ""schema"": { ""$ref"": ""#/components/schemas/Report"" } } } }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""Report"": {
        ""type"": ""object"",
        ""properties"": { ""day"": { ""type"": ""string"", ""format"": ""date"" } }
      }
    }
  }
}";

        private const string DateFormParametersSpec = @"{
  ""swagger"": ""2.0"",
  ""info"": { ""title"": ""Dates"", ""version"": ""1"" },
  ""paths"": {
    ""/urlencoded"": {
      ""post"": {
        ""operationId"": ""Form_UrlEncoded"",
        ""consumes"": [ ""application/x-www-form-urlencoded"" ],
        ""parameters"": [
          { ""name"": ""day"", ""in"": ""formData"", ""type"": ""string"", ""format"": ""date"" },
          { ""name"": ""at"", ""in"": ""formData"", ""type"": ""string"", ""format"": ""date-time"" }
        ],
        ""responses"": { ""200"": { ""description"": ""ok"" } }
      }
    },
    ""/multipart"": {
      ""post"": {
        ""operationId"": ""Form_Multipart"",
        ""consumes"": [ ""multipart/form-data"" ],
        ""parameters"": [
          { ""name"": ""day"", ""in"": ""formData"", ""type"": ""string"", ""format"": ""date"" },
          { ""name"": ""at"", ""in"": ""formData"", ""type"": ""string"", ""format"": ""date-time"" }
        ],
        ""responses"": { ""200"": { ""description"": ""ok"" } }
      }
    }
  }
}";

        [Fact]
        public async Task When_parameter_is_date_and_date_type_is_Date_then_it_is_serialized_as_a_date_only_string()
        {
            // Arrange
            var document = await OpenApiDocument.FromJsonAsync(DateParametersSpec);
            var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings = { DateTimeType = TypeScriptDateTimeType.Date }
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("url_ = url_.replace(\"{day}\", encodeURIComponent(day ? formatDate(day) : \"null\"));", code);
            Assert.Contains("url_ += \"startDate=\" + encodeURIComponent(startDate ? formatDate(startDate) : \"\") + \"&\";", code);
            Assert.Contains("days.forEach(item_ => { url_ += \"days=\" + encodeURIComponent(item_ ? formatDate(item_) : \"null\") + \"&\"; });", code);

            // date-time parameters are still sent as instants
            Assert.Contains("url_ += \"since=\" + encodeURIComponent(since ? \"\" + since.toISOString() : \"\") + \"&\";", code);
            Assert.DoesNotContain("startDate.toISOString()", code);

            // the function is emitted although no DTO property needs it
            Assert.Equal(1, CountOccurrences(code, "function formatDate("));
            TypeScriptCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_parameter_is_date_and_a_dto_property_is_date_then_formatDate_is_emitted_once()
        {
            // Arrange
            var document = await OpenApiDocument.FromJsonAsync(DateParameterAndDatePropertySpec);
            var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings = { DateTimeType = TypeScriptDateTimeType.Date }
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("encodeURIComponent(startDate ? formatDate(startDate) : \"\")", code);
            Assert.Contains("formatDate(this.day)", code);
            Assert.Equal(1, CountOccurrences(code, "function formatDate("));
            TypeScriptCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_parameter_is_date_and_date_type_is_DayJS_then_it_is_formatted_as_a_date_only_string()
        {
            // Arrange
            var document = await OpenApiDocument.FromJsonAsync(DateParametersSpec);
            var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings = { DateTimeType = TypeScriptDateTimeType.DayJS }
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("encodeURIComponent(day ? day.format('YYYY-MM-DD') : \"null\")", code);
            Assert.Contains("encodeURIComponent(startDate ? startDate.format('YYYY-MM-DD') : \"\")", code);
            Assert.Contains("encodeURIComponent(item_ ? item_.format('YYYY-MM-DD') : \"null\")", code);
            Assert.Contains("encodeURIComponent(since ? \"\" + since.toISOString() : \"\")", code);
            Assert.DoesNotContain("formatDate(", code);
        }

        [Fact]
        public async Task When_form_parameter_is_date_and_date_type_is_Date_then_it_is_serialized_as_a_date_only_string()
        {
            // Arrange
            var document = await OpenApiDocument.FromJsonAsync(DateFormParametersSpec);
            var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings = { DateTimeType = TypeScriptDateTimeType.Date }
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("content_ += encodeURIComponent(\"day\") + \"=\" + encodeURIComponent(day ? formatDate(day) : \"\") + \"&\";", code);
            Assert.Contains("content_ += encodeURIComponent(\"at\") + \"=\" + encodeURIComponent(at ? \"\" + at.toJSON() : \"\") + \"&\";", code);
            Assert.Contains("content_.append(\"day\", formatDate(day));", code);
            Assert.Contains("content_.append(\"at\", at.toJSON());", code);
            Assert.Equal(1, CountOccurrences(code, "function formatDate("));
            TypeScriptCompiler.AssertCompile(code);
        }

        private static int CountOccurrences(string text, string value)
        {
            var count = 0;
            for (var index = text.IndexOf(value, StringComparison.Ordinal); index >= 0; index = text.IndexOf(value, index + value.Length, StringComparison.Ordinal))
            {
                count++;
            }

            return count;
        }
    }
}
