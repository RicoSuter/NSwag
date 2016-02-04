using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApiToSwaggerGenerator
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        [ExpectedException(typeof(TypeLoadException))]
        public void When_controller_type_is_not_found_then_type_load_exception_is_thrown()
        {
            //// Arrange
            var settings = new WebApiAssemblyToSwaggerGeneratorSettings
            {
                AssemblyPath = @"./NSwag.CodeGeneration.Tests.dll",
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            };

            var generator = new WebApiAssemblyToSwaggerGenerator(settings);

            //// Act
            var swaggerService = generator.GenerateForController("NonExistingClass"); // Should throw exception

            //// Assert
        }
    }
}
