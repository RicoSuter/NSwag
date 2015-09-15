using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.Tests.Integration
{
    [TestClass]
    public class AssemblyTypeToSwaggerGeneratorTests
    {
        [TestMethod]
        public void When_loading_type_from_assembly_then_correct_count_of_properties_are_loaded()
        {
            //// Arrange
            var assemblyPath = "../../../NSwag.Demo.Web/bin/NSwag.Demo.Web.dll";
            var generator = new AssemblyToSwaggerGenerator(assemblyPath);

            //// Act
            var service = generator.FromAssemblyType("NSwag.Demo.Web.Models.Person");

            //// Assert
            Assert.AreEqual(3, service.Definitions["Person"].Properties.Count);
        }
    }
}