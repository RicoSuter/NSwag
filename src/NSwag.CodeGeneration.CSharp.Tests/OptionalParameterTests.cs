using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;
using NSwag.Generation.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class OptionalParameterTests
    {
        public class TestController : Controller
        {
            [Route("Test")]
            public void Test(string a, string b, string c = null)
            {
            }

            [Route("TestWithClass")]
            public void TestWithClass([FromUri] MyClass objet)
            {
            }

            [Route("TestWithEnum")]
            public void TestWithEnum([FromUri] MyEnum? myEnum = null)
            {
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
        public class FromUriAttribute : Attribute;

#pragma warning disable CA1711
        public enum MyEnum
#pragma warning restore CA1711
        {
            One,
            Two,
            Three,
            Four
        }

        public class MyClass
        {
#pragma warning disable IDE0051
            private string MyString { get; set; }
#pragma warning restore IDE0051
            public MyEnum? MyEnum { get; set; }
            public int MyInt { get; set; }
        }

        [Fact]
        public async Task When_setting_is_enabled_with_enum_fromuri_should_make_enum_nullable()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await generator.GenerateForControllerAsync<TestController>();

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_setting_is_enabled_with_class_fromuri_should_make_enum_nullable()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await generator.GenerateForControllerAsync<TestController>();

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_setting_is_enabled_then_optional_parameters_have_null_optional_value()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await generator.GenerateForControllerAsync<TestController>();

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_setting_is_enabled_then_parameters_are_reordered()
        {
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<TestController>();

            // Act
            var operation = document.Operations.First().Operation;
            var lastParameter = operation.Parameters.Last();
            operation.Parameters.Remove(lastParameter);
            operation.Parameters.Insert(0, lastParameter);
            var json = document.ToJson();
            Assert.NotNull(json);

            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_optional_parameter_comes_before_required()
        {
            // Arrange
            const string specification = """
                                         {
                                           "openapi": "3.0.0",
                                           "paths": {
                                             "/": {
                                               "get": {
                                                 "operationId": "Get",
                                                 "parameters": [
                                                   {
                                                     "name": "firstname",
                                                     "in": "query",
                                                     "schema": {
                                                       "type": "string",
                                                       "nullable": true
                                                     },
                                                     "x-position": 1
                                                   },
                                                   {
                                                     "name": "lastname",
                                                     "in": "query",
                                                     "required": true,
                                                     "schema": {
                                                       "type": "string"
                                                     },
                                                     "x-position": 2
                                                   }
                                                 ],
                                                 "responses": {
                                                   "200": {
                                                     "description": "",
                                                     "content": {
                                                       "application/json": {
                                                         "schema": {
                                                           "type": "string"
                                                         }
                                                       }
                                                     }
                                                   }
                                                 }
                                               }
                                             }
                                           }
                                         }
                                         """;

            // Act
            var document = await OpenApiDocument.FromJsonAsync(specification, "", SchemaType.OpenApi3);
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }
    }
}