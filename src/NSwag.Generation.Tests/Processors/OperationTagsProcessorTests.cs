using System;
using System.Reflection;
using NSwag.Annotations;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Xunit;

namespace NSwag.Generation.Tests.Processors
{
    public class OperationTagsProcessorTests
    {
        [OpenApiTag("Controller tag 1")]
        [OpenApiTags("Controller tag 2", "Controller tag 3")]
        public class TaggedController
        {
            public void UntaggedMethod()
            {
            }

            [OpenApiTag("Method tag 1")]
            [OpenApiTags("Method tag 2", "Method tag 3")]
            public void TaggedMethod()
            {
            }
        }

        public class UntaggedController
        {
            public void UntaggedMethod()
            {
            }
        }

        [Fact]
        public void Process_AddsTagsFromTaggedControllerForUntaggedMethod()
        {
            //// Arrange
            var controllerType = typeof(TaggedController);
            var methodInfo = controllerType.GetMethod("UntaggedMethod");

            var context = GetContext(controllerType, methodInfo);
            var processor = new OperationTagsProcessor();

            //// Act
            processor.Process(context);

            //// Assert
            var tags = context.OperationDescription.Operation.Tags;

            Assert.Collection(
                tags,
                tag =>
                {
                    Assert.Contains("Controller tag 1", tags);
                },
                tag =>
                {
                    Assert.Contains("Controller tag 2", tags);
                },
                tag =>
                {
                    Assert.Contains("Controller tag 3", tags);
                });
        }

        [Fact]
        public void Process_AddsTagsFromTaggedMethod()
        {
            //// Arrange
            var controllerType = typeof(TaggedController);
            var methodInfo = controllerType.GetMethod("TaggedMethod");

            var context = GetContext(controllerType, methodInfo);
            var processor = new OperationTagsProcessor();

            //// Act
            processor.Process(context);

            //// Assert
            var tags = context.OperationDescription.Operation.Tags;

            Assert.Collection(
                tags,
                tag =>
                {
                    Assert.Contains("Method tag 1", tags);
                },
                tag =>
                {
                    Assert.Contains("Method tag 2", tags);
                },
                tag =>
                {
                    Assert.Contains("Method tag 3", tags);
                });
        }

        [Fact]
        public void Process_AddsControllerNameWhenNoTagsArePresent()
        {
            //// Arrange
            var controllerType = typeof(UntaggedController);
            var methodInfo = controllerType.GetMethod("UntaggedMethod");

            var context = GetContext(controllerType, methodInfo);
            var processor = new OperationTagsProcessor();

            //// Act
            processor.Process(context);

            //// Assert
            var tags = context.OperationDescription.Operation.Tags;

            Assert.Collection(
                tags,
                tag =>
                {
                    Assert.Equal("Untagged", tag);
                });
        }

        private OperationProcessorContext GetContext(Type controllerType, MethodInfo methodInfo)
        {
            var document = new OpenApiDocument();
            var operationDescription = new OpenApiOperationDescription { Operation = new OpenApiOperation() };
            return new OperationProcessorContext(document, operationDescription, controllerType, methodInfo, null, null, null, null, null);
        }
    }
}
