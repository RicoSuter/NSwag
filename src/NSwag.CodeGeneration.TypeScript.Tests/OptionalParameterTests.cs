using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    [TestClass]
    public class OptionalParameterTests
    {
        public class TestController : ApiController
        {
            [Route("Test")]
            public void Test(string a, string b, string c = null)
            {
            }

            [Route("TestWithDefaultStringValue")]
            public void TestWithDefaultStringValue(string a, string b, string c = "aaa")
            {
            }

            [Route("TestWithDefaultNumericValue")]
            public void TestWithDefaultNumericValue(string a, string b, decimal c = 3.14M)
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

        public enum MyEnum
        {
            One,
            Two,
            Three,
            Four

        }
        public class MyClass
        {
            private string MyString { get; set; }
            public MyEnum? MyEnum { get; set; }
            public int MyInt { get; set; }
        }

        [TestMethod]
        public async Task When_setting_is_enabled_with_enum_fromuri_should_make_enum_nullable()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGenerator = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = codeGenerator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("testWithEnum(myEnum?: MyEnum): Promise<void>"));
        }

        [TestMethod]
        public async Task When_setting_is_enabled_with_class_fromuri_should_make_enum_nullable()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGenerator = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = codeGenerator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("testWithClass(myString?: string, myEnum?: MyEnum, myInt?: number): Promise<void>"));
        }


        [TestMethod]
        public async Task When_setting_is_enabled_then_optional_parameters_have_null_optional_value()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGenerator = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = codeGenerator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("test(a: string, b: string, c?: string): Promise<void>"));
        }

        [TestMethod]
        public async Task When_setting_is_enabled_then_parameters_are_reordered()
        {
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<TestController>();

            //// Act
            var operation = document.Operations.First().Operation;
            var lastParameter = operation.Parameters.Last();
            operation.Parameters.Remove(lastParameter);
            operation.Parameters.Insert(0, lastParameter);
            var json = document.ToJson();

            var codeGenerator = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = codeGenerator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("test(a: string, b: string, c?: string): Promise<void>"));
        }


        [TestMethod]
        public async Task When_setting_is_enabled_then_optional_parameters_have_default_value()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGenerator = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                GenerateOptionalParameters = true,
                GenerateOptionalParameterDefaultValues = true
            });
            var code = codeGenerator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("testWithDefaultStringValue(a: string, b: string, c?: string = \"aaa\"): Promise<void>"));
            Assert.IsTrue(code.Contains("testWithDefaultNumericValue(a: string, b: string, c?: number = 3.14): Promise<void>"));
        }
    }
}