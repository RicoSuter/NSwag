using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

using CoreMvc = Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NJsonSchema;


namespace NSwag.Generation.WebApi.Tests
{
    [TestClass]
    public class WrappedResponseTests
    {
        public class WrappedResponseController : ApiController
        {
            [HttpGet, Route( "task" )]
            public Task Task()
            {
                throw new NotImplementedException();
            }

            [HttpGet, Route( "int" )]
            public int Int()
            {
                throw new NotImplementedException();
            }

            [HttpGet, Route( "taskofint" )]
            public Task<int> TaskOfInt()
            {
                throw new NotImplementedException();
            }

            [HttpGet, Route( "valuetaskofint" )]
            public ValueTask<int> ValueTaskOfInt()
            {
                throw new NotImplementedException();
            }

            [HttpGet, Route( "jsonresultofint" )]
            public JsonResult<int> JsonResultOfInt()
            {
                throw new NotImplementedException();
            }

            [HttpGet, Route( "actionresultofint" )]
            public CoreMvc.ActionResult<int> ActionResultOfInt()
            {
                throw new NotImplementedException();
            }
            
        }

        [TestMethod]
        public async Task When_response_is_wrapped_in_certain_generic_result_types_then_discard_the_wrapper_type()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator( new WebApiOpenApiDocumentGeneratorSettings() );

            // Act
            var document = await generator.GenerateForControllerAsync<WrappedResponseController>();

            // Assert
            OpenApiResponse GetOperationResponse( String ActionName )
                => document.Operations.Where( op => op.Operation.OperationId == $"{nameof(WrappedResponseController).Substring(0,nameof(WrappedResponseController).Length - "Controller".Length )}_{ActionName}" ).Single().Operation.ActualResponses.Single().Value;
            JsonObjectType GetOperationResponseSchemaType( String ActionName )
                => GetOperationResponse( ActionName ).Schema.Type;
            var IntType = JsonSchema.FromType<int>().Type;

            Assert.IsNull( GetOperationResponse( nameof( WrappedResponseController.Task ) ).Schema );
            Assert.AreEqual( IntType, GetOperationResponseSchemaType( nameof( WrappedResponseController.Int ) ) );
            Assert.AreEqual( IntType, GetOperationResponseSchemaType( nameof( WrappedResponseController.TaskOfInt ) ) );
            Assert.AreEqual( IntType, GetOperationResponseSchemaType( nameof( WrappedResponseController.ValueTaskOfInt ) ) );
            Assert.AreEqual( IntType, GetOperationResponseSchemaType( nameof( WrappedResponseController.JsonResultOfInt ) ) );
            Assert.AreEqual( IntType, GetOperationResponseSchemaType( nameof( WrappedResponseController.ActionResultOfInt ) ) );

        }
    }
}