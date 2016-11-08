using System;
using System.Web.Http;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.CodeGenerators;
using NSwag.CodeGeneration.CodeGenerators.TypeScript;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.OperationNameGenerator
{
    [TestClass]
    public class MultipleClientsFromOperationIdOperationNameGeneratorTests
    {
        public class PointController
        {
            [HttpPost]
            public Point[] Get()
            {
                throw new NotImplementedException();
            }

            [HttpGet]
            public Point Get(int id)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void When_two_methods_have_same_name_then_generated_id_is_still_different()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = generator.GenerateForController<PointController>();
            var codeGenerator = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                OperationGenerationMode = OperationGenerationMode.MultipleClientsFromOperationId
            });

            //// Act
            var code = codeGenerator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains(" get("));
            Assert.IsTrue(code.Contains(" getAll("));
        }
    }
}
