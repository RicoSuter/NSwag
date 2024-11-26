﻿using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters;
using Xunit;

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

            Assert.Contains(@"    ""/api/FileUpload/UploadFiles"": {
      ""post"": {
        ""tags"": [
          ""FileUpload""
        ],
        ""operationId"": ""FileUpload_UploadFiles"",
        ""requestBody"": {
          ""content"": {
            ""multipart/form-data"": {
              ""schema"": {
                ""required"": [
                  ""files"",
                  ""test""
                ],
                ""properties"": {
                  ""files"": {
                    ""type"": ""array"",
                    ""nullable"": false,
                    ""items"": {
                      ""type"": ""string"",
                      ""format"": ""binary""
                    }
                  },
                  ""test"": {
                    ""type"": ""string"",
                    ""nullable"": false
                  }
                }
              }
            }
          }
        },".Replace("\r", ""), json.Replace("\r", ""));
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
            Assert.Contains(@"""requestBody"": {
          ""content"": {
            ""multipart/form-data"": {
              ""schema"": {
                ""type"": ""object"",
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
        },".Replace("\r", ""), json.Replace("\r", ""));
        }

        [Fact]
        public async Task WhenOperationHasFormDataComplexWithRequiredProperties_ThenItIsInRequestBody()
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
            var operation = document.Operations.First(o => o.Operation.OperationId == "FileUpload_UploadAttachment2").Operation;

            Assert.NotNull(operation);
            Assert.Contains(@"""requestBody"": {
          ""content"": {
            ""multipart/form-data"": {
              ""schema"": {
                ""type"": ""object"",
                ""required"": [
                  ""Title"",
                  ""contents""
                ],
                ""properties"": {
                  ""Title"": {
                    ""type"": ""string"",
                    ""nullable"": false
                  },
                  ""MessageId"": {
                    ""type"": ""integer"",
                    ""format"": ""int32"",
                    ""nullable"": true
                  },
                  ""contents"": {
                    ""type"": ""string"",
                    ""format"": ""binary"",
                    ""nullable"": false
                  }
                }
              }
            }
          }
        },".Replace("\r", ""), json.Replace("\r", ""));
        }
    }
}