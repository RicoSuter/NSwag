using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.OperationNameGenerators;

namespace NSwag.SwaggerGeneration.WebApi.Tests.OperationNameGenerator
{
    [TestClass]
    public class MultipleClientsFromPathSegmentsOperationNameGeneratorTests
    {
        [TestMethod]
        public void When_path_is_slash_then_operation_name_is_index()
        {
            //// Arrange
            var generator = new MultipleClientsFromPathSegmentsOperationNameGenerator();

            //// Act
            var operationName = generator.GetOperationName(new SwaggerDocument(), "/", OpenApiOperationMethod.Get, new OpenApiOperation());

            //// Assert
            Assert.AreEqual("Index", operationName);
        }

        [TestMethod]
        public void When_path_with_path_parameter_is_slash_then_operation_name_is_index()
        {
            //// Arrange
            var generator = new MultipleClientsFromPathSegmentsOperationNameGenerator();

            //// Act
            var operationName = generator.GetOperationName(new SwaggerDocument(), "/{id}", OpenApiOperationMethod.Get, new OpenApiOperation());

            //// Assert
            Assert.AreEqual("Index", operationName);
        }
    }
}
