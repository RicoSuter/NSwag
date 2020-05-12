using System.Linq;
using System.Threading.Tasks;
using NJsonSchema;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters;
using Xunit;

namespace NSwag.Generation.AspNetCore.Tests.Parameters
{
    public class FileUploadTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task WhenOperationHasFormDataFile_ThenItIsInRequestBody()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings { SchemaType = SchemaType.OpenApi3 };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(FileUploadController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "FileUpload_UploadFile").Operation;
            var schema = operation.RequestBody.Content["multipart/form-data"].Schema;

            Assert.Equal("binary", schema.Properties["file"].Format);
            Assert.Equal(JsonObjectType.String, schema.Properties["file"].Type);

            Assert.Equal(JsonObjectType.String, schema.Properties["test"].Type);
        }

        [Fact]
        public async Task WhenOperationHasFormDataComplex_ThenItIsInRequestBody()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings { SchemaType = SchemaType.OpenApi3 };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(FileUploadController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "FileUpload_UploadAttachment").Operation;

            Assert.NotNull(operation);
            Assert.Contains(@"""requestBody"": {
          ""content"": {
            ""multipart/form-data"": {
              ""schema"": {
                ""properties"": {
                  ""Description"": {
                    ""type"": ""string"",
                    ""nullable"": true
                  },
                  ""Contents"": {
                    ""type"": ""string"",
                    ""format"": ""binary"",
                    ""nullable"": true
                  }
                }
              }
            }
          }
        },", json);
        }
    }
}