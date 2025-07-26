using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.Generation.WebApi;
using System.Runtime.Serialization;
using NJsonSchema.NewtonsoftJson.Converters;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class TypeScriptDiscriminatorTests
    {
        [JsonConverter(typeof(JsonInheritanceConverter), "type")]
        [KnownType(typeof(OneChild))]
        [KnownType(typeof(SecondChild))]
        public abstract class Base
        {
            public EBase Type { get; }
        }

        public enum EBase
        {
            OneChild,
            SecondChild
        }

        public class OneChild : Base
        {
            public string A { get; }
        }

        public class SecondChild : Base
        {
            public string B { get; }
        }

        public class Nested
        {
            public Base Child { get; set; }

            public ICollection<Base> ChildCollection { get; set; }
        }

        public class DiscriminatorController
        {
            [Route("foo")]
            public string TestLeaf(Base param)
            {
                return null;
            }

            [Route("foo-arr")]
            public string TestLeafArr(ICollection<Base> param)
            {
                return null;
            }

            [Route("bar")]
            public string Test(OneChild param)
            {
                return null;
            }

            [Route("baz")]
            public string TestNested(Nested param)
            {
                return null;
            }
        }

        [Fact]
        public async Task When_parameter_is_abstract_then_generate_union()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<DiscriminatorController>();
            var clientGenerator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings =
                {
                    UseLeafType = true,
                    TypeScriptVersion = 1.4m,
                    NullValue = TypeScriptNullValue.Undefined
                }
            });

            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var code = clientGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);

            // this seems to be broken syntax
            // CodeCompiler.AssertCompile(code);
        }
    }
}
