using NJsonSchema;
using NSwag.CodeGeneration.OperationNameGenerators;

namespace NSwag.CodeGeneration.Tests
{
    public class CodeGenerationTests
    {
        [Fact]
        public async Task When_using_json_schema_with_references_in_service_then_references_are_correctly_resolved()
        {
            // Arrange
            var jsonSchema = @"{
  ""definitions"": {
    ""app"": {
      ""definitions"": {
        ""name"": {
          ""pattern"": ""^[a-z][a-z0-9-]{3,30}$"",
          ""type"": ""string""
        }
      },
      ""properties"": {
        ""name"": {
          ""$ref"": ""#/definitions/app/definitions/name""
        }
      },
      ""required"": [""name""],
      ""type"": ""object""
    }
  },
  ""properties"": {
    ""app"": {
      ""$ref"": ""#/definitions/app""
    },
  },
  ""type"": ""object""
}";

            // Act
            var schema = await JsonSchema.FromJsonAsync(jsonSchema);
            var document = new OpenApiDocument();
            document.Definitions["Foo"] = schema;

            // Assert
            var json = document.ToJson(); // no exception expected
            Assert.NotNull(json);
        }

        [Fact]
        public void No_Brackets_in_Operation_Name()
        {
            // Arrange
            var path = "/my/path/with/{parameter_with_underscore}/and/{another_parameter}";

            // Act
            var operationName = SingleClientFromPathSegmentsOperationNameGenerator.ConvertPathToName(path);

            // Assert
            Assert.DoesNotContain("{", operationName);
            Assert.DoesNotContain("}", operationName);
            Assert.False(string.IsNullOrWhiteSpace(operationName));
        }

        [Theory(DisplayName = "Ensure expected client name generation when using MultipleClientsFromFirstTagAndOperationName behavior")]
        [InlineData(new string[] { "firstTag", "secondTag" }, "FirstTag")]
        [InlineData(new string[] { "firsttag", "secondtag" }, "Firsttag")]
        public void When_using_MultipleClientsFromFirstTagAndOperationName_then_ensure_that_clientname_is_from_first_tag(string[] tags, string expectedClientName)
        {
            // Arrange
            var operation = new OpenApiOperation
            {
                Tags = [.. tags]
            };
            var generator = new MultipleClientsFromFirstTagAndOperationNameGenerator();

            var document = new OpenApiDocument();
            var path = string.Empty;
            var httpMethod = string.Empty;

            // Act
            string clientName = generator.GetClientName(document, path, httpMethod, operation);

            // Assert
            Assert.Equal(expectedClientName, clientName);
        }

        [Theory(DisplayName = "Ensure expected operation name generation when using MultipleClientsFromFirstTagAndOperationName behavior")]
        [InlineData("OperationId_SecondUnderscore_Test", "SecondUnderscore_Test")]
        [InlineData("OperationId_MultipleUnderscores_Client_Test", "MultipleUnderscores_Client_Test")]
        [InlineData("OperationId_Test", "Test")]
        [InlineData("UnderscoreLast_", "UnderscoreLast_")]
        [InlineData("_UnderscoreFirst", "UnderscoreFirst")]
        [InlineData("NoUnderscore", "NoUnderscore")]
        public void When_using_MultipleClientsFromFirstTagAndOperationName_then_ensure_that_operationname_is_last_part_of_operation_id(string operationId, string expectedOperationName)
        {
            // Arrange
            var operation = new OpenApiOperation
            {
                OperationId = operationId
            };
            var generator = new MultipleClientsFromFirstTagAndOperationNameGenerator();

            var document = new OpenApiDocument();
            var path = string.Empty;
            var httpMethod = string.Empty;

            // Act
            string operationName = generator.GetOperationName(document, path, httpMethod, operation);

            // Assert
            Assert.Equal(expectedOperationName, operationName);
        }

        [Theory(DisplayName = "Ensure expected client name generation with different operationIds when using the MultipleClientsFromOperationId behavior")]
        [InlineData("OperationId_SecondUnderscore_Test", "OperationId")]
        [InlineData("OperationId_MultipleUnderscores_Client_Test", "OperationId")]
        [InlineData("OperationId_Test", "OperationId")]
        [InlineData("UnderscoreLast_", "UnderscoreLast")]
        [InlineData("_UnderscoreFirst", "")]
        [InlineData("NoUnderscore", "")]
        public void When_using_MultipleClientsFromOperationId_then_ensure_that_underscores_are_handled_as_expected(string operationId, string expectedClientName)
        {
            // Arrange
            var operation = new OpenApiOperation
            {
                OperationId = operationId
            };
            var generator = new MultipleClientsFromOperationIdOperationNameGenerator();

            // Arrange - "unused"
            // We don't need these values, because internally GetClientName only uses the operation
            // Use default values to prevent future exceptions when e.g. any null validation would be added
            var document = new OpenApiDocument();
            var path = string.Empty;
            var httpMethod = string.Empty;

            // Act
            string clientName = generator.GetClientName(document, path, httpMethod, operation);

            // Assert
            Assert.Equal(expectedClientName, clientName);
        }

        [Theory(DisplayName = "Ensure expected operation name generation with different operationIds when using the MultipleClientsFromOperationId behavior")]
        [InlineData("OperationId_SecondUnderscore_Test", "SecondUnderscore_Test")]
        [InlineData("OperationId_MultipleUnderscores_Client_Test", "MultipleUnderscores_Client_Test")]
        [InlineData("OperationId_Test", "Test")]
        [InlineData("UnderscoreLast_", "UnderscoreLast_")]
        [InlineData("_UnderscoreFirst", "UnderscoreFirst")]
        [InlineData("NoUnderscore", "NoUnderscore")]
        public void When_using_MultipleClientsFromOperationId_then_ensure_that_operationname_is_correct(string operationId, string expectedOperationName)
        {
            // Arrange
            var operation = new OpenApiOperation
            {
                OperationId = operationId
            };
            var generator = new MultipleClientsFromOperationIdOperationNameGenerator();

            var document = new OpenApiDocument();
            var path = string.Empty;
            var httpMethod = string.Empty;

            // Act
            string operationName = generator.GetOperationName(document, path, httpMethod, operation);

            // Assert
            Assert.Equal(expectedOperationName, operationName);
        }

        [Theory(DisplayName = "Ensure unique (client, operation) pairs when using MultipleClientsFromOperationId to avoid duplicate method names")]
        [InlineData("Orders_items_get", "Products_items_get")]
        [InlineData("Resource1_getSomething", "Resource2_getSomething")]
        [InlineData("A_B_C", "D_B_C")]
        public void When_using_MultipleClientsFromOperationId_then_unique_operationIds_produce_unique_client_operation_pairs(string operationId1, string operationId2)
        {
            // Arrange
            var operation1 = new OpenApiOperation { OperationId = operationId1 };
            var operation2 = new OpenApiOperation { OperationId = operationId2 };
            var generator = new MultipleClientsFromOperationIdOperationNameGenerator();

            var document = new OpenApiDocument();
            var path = string.Empty;
            var httpMethod = string.Empty;

            // Act
            string clientName1 = generator.GetClientName(document, path, httpMethod, operation1);
            string operationName1 = generator.GetOperationName(document, path, httpMethod, operation1);
            string clientName2 = generator.GetClientName(document, path, httpMethod, operation2);
            string operationName2 = generator.GetOperationName(document, path, httpMethod, operation2);

            // Assert: unique operation IDs must not produce the same (client, operation) pair
            Assert.False(
                clientName1 == clientName2 && operationName1 == operationName2,
                $"OperationIds '{operationId1}' and '{operationId2}' produced duplicate (client='{clientName1}', operation='{operationName1}') pair");
        }
    }
}
