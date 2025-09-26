using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters;

namespace NSwag.Generation.AspNetCore.Tests.Parameters
{
    public class FormDataTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task WhenOperationHasFormDataFile_ThenItIsInRequestBody()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(FileUploadController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "FileUpload_UploadFile").Operation;
            var schema = operation.RequestBody.Content["multipart/form-data"].Schema;

            Assert.Equal("binary", schema.Properties["file"].Format);
            Assert.Equal(JsonObjectType.String, schema.Properties["file"].Type);
            Assert.Equal(JsonObjectType.String, schema.Properties["test"].Type);

            await VerifyHelper.Verify(json);
        }

        [Fact]
        public async Task WhenOperationHasFormDataComplex_ThenItIsInRequestBody()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(FileUploadController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "FileUpload_UploadAttachment").Operation;

            Assert.NotNull(operation);
            await VerifyHelper.Verify(json);
        }
    }
}