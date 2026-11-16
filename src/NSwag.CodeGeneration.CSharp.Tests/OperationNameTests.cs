using NSwag.CodeGeneration.OperationNameGenerators;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class OperationNameTests
    {
        private const string Specification = @"{
  ""openapi"": ""3.0.0"",
  ""info"": { ""title"": ""Test"", ""version"": ""1.0"" },
  ""paths"": {
    ""/rest/vcenter/vcha/cluster/mode?vmw-task=true"": {
      ""put"": {
        ""operationId"": ""set$task"",
        ""responses"": { ""200"": { ""description"": ""OK"" } }
      }
    },
    ""/TicketApproval/{id}&{seq}"": {
      ""delete"": {
        ""operationId"": ""ticket&approval"",
        ""parameters"": [
          { ""name"": ""id"", ""in"": ""path"", ""required"": true, ""schema"": { ""type"": ""integer"" } },
          { ""name"": ""seq"", ""in"": ""path"", ""required"": true, ""schema"": { ""type"": ""integer"" } }
        ],
        ""responses"": { ""200"": { ""description"": ""OK"" } }
      }
    }
  }
}";

        [Fact]
        public async Task When_operation_id_contains_invalid_characters_then_method_name_is_valid()
        {
            var document = await OpenApiDocument.FromJsonAsync(Specification);

            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator()
            });
            var code = generator.GenerateFile();

            Assert.Contains("Task SetTaskAsync(", code);
            Assert.Contains("Task TicketApprovalAsync(", code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_path_contains_invalid_characters_then_method_name_is_valid()
        {
            var document = await OpenApiDocument.FromJsonAsync(Specification);

            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                OperationNameGenerator = new SingleClientFromPathSegmentsOperationNameGenerator()
            });
            var code = generator.GenerateFile();

            Assert.Contains("Task RestVcenterVchaClusterModeVmwTaskTrueAsync(", code);
            Assert.Contains("Task TicketApprovalAsync(", code);
            CSharpCompiler.AssertCompile(code);
        }
    }
}
