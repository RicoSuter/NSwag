using NSwag.CodeGeneration.CSharp.Models;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class BinaryTests
    {
        [Fact]
        public async Task When_body_is_binary_then_stream_is_used_as_parameter_in_CSharp()
        {
            var yaml = @"openapi: 3.0.0
servers:
  - url: https://www.example.com/
info:
  version: '2.0.0'
  title: 'Test API'   
paths:
  /files:
    post:
      tags:
        - Files
      summary: 'Add File'
      operationId: addFile
      responses:
        '200':
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/FileToken'
      requestBody:
        content:
          image/png:
            schema:
              type: string
              format: binary
components:
  schemas:
    FileToken:
      type: object
      required:
        - fileId    
      properties:  
        fileId:
          type: string
          format: uuid";

            var document = await OpenApiYamlDocument.FromYamlAsync(yaml);

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_body_is_binary_then_IFormFile_is_used_as_parameter_in_CSharp_ASPNETCore()
        {
            var yaml = @"openapi: 3.0.0
servers:
  - url: https://www.example.com/
info:
  version: '2.0.0'
  title: 'Test API'   
paths:
  /files:
    post:
      tags:
        - Files
      summary: 'Add File'
      operationId: addFile
      responses:
        '200':
          description: 'something'
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/FileToken'
      requestBody:
       content:
         multipart/form-data:
           schema:
             type: string
             format: binary
components:
  schemas:
    FileToken:
      type: object
      required:
        - fileId    
      properties:  
        fileId:
          type: string
          format: uuid";

            var document = await OpenApiYamlDocument.FromYamlAsync(yaml);

            // Act
            CSharpControllerGeneratorSettings settings = new CSharpControllerGeneratorSettings();
            settings.ControllerTarget = CSharpControllerTarget.AspNetCore;
            var codeGenerator = new CSharpControllerGenerator(document, settings);
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_body_is_binary_array_then_IFormFile_Collection_is_used_as_parameter_in_CSharp_ASPNETCore()
        {
            var yaml = @"openapi: 3.0.0
servers:
  - url: https://www.example.com/
info:
  version: '2.0.0'
  title: 'Test API'   
paths:
  /files:
    post:
      tags:
        - Files
      summary: 'Add File'
      operationId: addFile
      responses:
        '200':
          description: 'something'
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/FileToken'
      requestBody:
       content:
         multipart/form-data:
           schema:
             type: array
             items:
               type: string
               format: binary
components:
  schemas:
    FileToken:
      type: object
      required:
        - fileId    
      properties:  
        fileId:
          type: string
          format: uuid";

            var document = await OpenApiYamlDocument.FromYamlAsync(yaml);

            // Act
            CSharpControllerGeneratorSettings settings = new CSharpControllerGeneratorSettings();
            settings.ControllerTarget = CSharpControllerTarget.AspNetCore;
            var codeGenerator = new CSharpControllerGenerator(document, settings);
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task WhenSpecContainsFormDataInSingleMultipartFile_ThenFormDataIsUsedInCSharp()
        {
            var json = @"{
  ""x-generator"": ""NSwag v13.7.0.0 (NJsonSchema v10.2.0.0 (Newtonsoft.Json v11.0.0.0))"",
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""title"": ""My Title"",
    ""version"": ""1.0.0""
  },
  ""paths"": {
    ""/api/FileUpload/UploadFile"": {
      ""post"": {
        ""tags"": [
          ""FileUpload""
        ],
        ""operationId"": ""FileUpload_UploadFile"",
        ""requestBody"": {
          ""content"": {
            ""multipart/form-data"": {
              ""schema"": {
                ""type"": ""object"",
                ""properties"": {
                  ""file"": {
                    ""type"": ""string"",
                    ""format"": ""binary""
                  },
                  ""test"": {
                    ""type"": ""string""
                  }
                }
              }
            }
          }
        },
        ""responses"": {
          ""200"": {
            ""description"": """",
            ""content"": {
              ""application/octet-stream"": {
                ""schema"": {
                  ""type"": ""string"",
                  ""format"": ""binary""
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {}
}";

            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task WhenSpecContainsFormDataInMultipartFileArray_ThenFormDataIsUsedInCSharp()
        {
            var json = @"{
  ""x-generator"": ""NSwag v13.7.0.0 (NJsonSchema v10.2.0.0 (Newtonsoft.Json v11.0.0.0))"",
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""title"": ""My Title"",
    ""version"": ""1.0.0""
  },
  ""paths"": {
    ""/api/FileUpload/UploadFiles"": {
      ""post"": {
        ""tags"": [
          ""FileUpload""
        ],
        ""operationId"": ""FileUpload_UploadFiles"",
        ""requestBody"": {
          ""content"": {
            ""multipart/form-data"": {
              ""schema"": {
                ""type"": ""object"",
                ""properties"": {
                  ""files"": {
                    ""type"": ""array"",
                    ""items"": {
                      ""type"": ""string"",
                      ""format"": ""binary""
                    }
                  },
                  ""test"": {
                    ""type"": ""string""
                  }
                }
              }
            }
          }
        },
        ""responses"": {
          ""200"": {
            ""description"": """",
            ""content"": {
              ""application/octet-stream"": {
                ""schema"": {
                  ""type"": ""string"",
                  ""format"": ""binary""
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {}
}";

            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task WhenSpecContainsFormDataInNestedMultipartForm_ThenFormDataIsUsedInCSharp()
        {
            var json = @"{
  ""x-generator"": ""NSwag v13.7.0.0 (NJsonSchema v10.2.0.0 (Newtonsoft.Json v11.0.0.0))"",
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""title"": ""My Title"",
    ""version"": ""1.0.0""
  },
  ""paths"": {
    ""/api/FileUpload/UploadAttachment"": {
      ""post"": {
        ""tags"": [
          ""FileUpload""
        ],
        ""operationId"": ""FileUpload_UploadAttachment"",
        ""parameters"": [
          {
            ""name"": ""caseId"",
            ""in"": ""path"",
            ""required"": true,
            ""schema"": {
              ""type"": ""string"",
              ""nullable"": true
            },
            ""x-position"": 1
          }
        ],
        ""requestBody"": {
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
        },
        ""responses"": {
          ""200"": {
            ""description"": """",
            ""content"": {
              ""application/octet-stream"": {
                ""schema"": {
                  ""type"": ""string"",
                  ""format"": ""binary""
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {}
}";

            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_multipart_with_ref_should_read_schema()
        {
            var yaml = @"openapi: 3.0.0
servers:
  - url: https://www.example.com/
info:
  version: '2.0.0'
  title: 'Test API'   
paths:
  /files:
    post:
      tags:
        - Files
      summary: 'Add File'
      operationId: addFile
      responses:
        '200':
          description: OK
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/CreateAddFileResponse'
      requestBody:
        required: true
        content:
          multipart/form-data:
            schema:
              $ref: '#/components/schemas/CreateAddFileRequest'
components:
  schemas:
    CreateAddFileResponse:
      type: object
      required:
        - fileId    
      properties:  
        fileId:
          type: string
          format: uuid
    CreateAddFileRequest:
      type: object
      additionalProperties: false
      properties:
        file:
         type: string     
         format: binary
        model:
         type: string
         enum: ['model-1']
      required:
      - file
      - model";

            var document = await OpenApiYamlDocument.FromYamlAsync(yaml);

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_multipart_inline_schema()
        {
            var yaml = @"openapi: 3.0.0
servers:
  - url: https://www.example.com/
info:
  version: '2.0.0'
  title: 'Test API'   
paths:
  /files:
    post:
      tags:
        - Files
      summary: 'Add File'
      operationId: addFile
      responses:
        '200':
          description: OK
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/CreateAddFileResponse'
      requestBody:
        required: true
        content:
          multipart/form-data:
            schema:
              type: object
              additionalProperties: false
              properties:
                file:
                 type: string     
                 format: binary
                model:
                 type: string
                 enum: ['model-1']
              required:
              - file
              - model
components:
  schemas:
    CreateAddFileResponse:
      type: object
      required:
        - fileId    
      properties:  
        fileId:
          type: string
          format: uuid";

            var document = await OpenApiYamlDocument.FromYamlAsync(yaml);

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }
    }
}
