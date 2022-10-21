using System;
using System.ComponentModel;
using System.Reflection;
using NSwag.Annotations;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Xunit;

namespace NSwag.Generation.Tests.Processors
{
    public class OperationSummaryAndDescriptionProcessorTests
    {
        public class DocumentedController
        {
            [OpenApiOperation("\r\n\t This method has a summary. \r\n\t", "\r\n\t This method has a description. \r\n\t")]
            public void DocumentedMethodWithOpenApiOperationAttribute()
            {
            }
            
            [Description("\r\n\t This method has a description. \r\n\t")]
            public void DocumentedMethodWithDescriptionAttribute()
            {
            }

            /// <summary>
            ///     This method has a summary.
            /// </summary>
            /// <remarks>
            ///     This method has a description.
            /// </remarks>
            public void DocumentedMethodWithSummary()
            {
            }
        }
        
        [Fact]
        public void Process_TrimsWhitespaceFromOpenApiOperationSummary()
        {
            //// Arrange
            var controllerType = typeof(DocumentedController);
            var methodInfo = controllerType.GetMethod(nameof(DocumentedController.DocumentedMethodWithOpenApiOperationAttribute));

            var context = GetContext(controllerType, methodInfo);
            var processor = new OperationSummaryAndDescriptionProcessor();

            //// Act
            processor.Process(context);

            //// Assert
            var summary = context.OperationDescription.Operation.Summary;
            Assert.Equal("This method has a summary.", summary);

            var description = context.OperationDescription.Operation.Description;
            Assert.Equal("This method has a description.", description);
        }
        
        [Fact]
        public void Process_TrimsWhitespaceFromDescription()
        {
            //// Arrange
            var controllerType = typeof(DocumentedController);
            var methodInfo = controllerType.GetMethod(nameof(DocumentedController.DocumentedMethodWithDescriptionAttribute));

            var context = GetContext(controllerType, methodInfo);
            var processor = new OperationSummaryAndDescriptionProcessor();

            //// Act
            processor.Process(context);

            //// Assert
            var summary = context.OperationDescription.Operation.Summary;
            Assert.Equal("This method has a description.", summary);

            var description = context.OperationDescription.Operation.Description;
            Assert.Null(description);
        }
        
        [Fact]
        public void Process_TrimsWhitespaceFromSummary()
        {
            //// Arrange
            var controllerType = typeof(DocumentedController);
            var methodInfo = controllerType.GetMethod(nameof(DocumentedController.DocumentedMethodWithSummary));

            var context = GetContext(controllerType, methodInfo);
            var processor = new OperationSummaryAndDescriptionProcessor();

            //// Act
            processor.Process(context);

            //// Assert
            var summary = context.OperationDescription.Operation.Summary;
            Assert.Equal("This method has a summary.", summary);

            var description = context.OperationDescription.Operation.Description;
            Assert.Equal("This method has a description.", description);
        }
        
        private OperationProcessorContext GetContext(Type controllerType, MethodInfo methodInfo)
        {
            var document = new OpenApiDocument();
            var operationDescription = new OpenApiOperationDescription { Operation = new OpenApiOperation() };
            var settings = new OpenApiDocumentGeneratorSettings();
            return new OperationProcessorContext(document, operationDescription, controllerType, methodInfo, null, null, null, settings, null);
        }
    }
}
