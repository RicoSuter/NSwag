using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi
{
    [TestClass]
    public class EnumListTests
    {
        public enum MetadataSchemaType
        {
            Foo,
            Bar
        }

        public class MetadataSchemaDetailViewItem
        {
            public string Id { get; set; }
            public List<MetadataSchemaType> Types { get; set; }
        }

        public class MetadataSchemaCreateRequest
        {
            public string Id { get; set; }
            public List<MetadataSchemaType> Types { get; set; }
        }

        public class MyController
        {
            public MetadataSchemaDetailViewItem GetItem(MetadataSchemaCreateRequest request)
            {
                return null;
            }
        }

        [TestMethod]
        public void When_enum_is_used_as_array_item_then_it_is_generated_only_once()
        {
            // Arrange
            var apiGenerator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var document = apiGenerator.GenerateForController<MyController>();
            var json = document.ToJson();

            // Assert
            Assert.IsTrue(json.Split(new[] { "x-enumNames" }, StringSplitOptions.None).Length == 2); // enum is defined only once
        }
    }
}