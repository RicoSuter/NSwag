#nullable enable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema.Annotations;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Responses
{
    [ApiController]
    [Route("api/[controller]")]
    public class NullableResponseController : Controller
    {
        /// <summary>
        /// Gets an order.
        /// </summary>
        /// <response code="200" nullable="true">Order created.</response>
        /// <response code="404">Order not found.</response>
        [HttpPost("OperationWithNullableResponse")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        public ActionResult<string> OperationWithNullableResponse()
        {
            return Ok();
        }

        /// <summary>
        /// Gets an order.
        /// </summary>
        /// <response code="200" nullable="false">Order created.</response>
        /// <response code="404">Order not found.</response>
        [HttpPost("OperationWithNonNullableResponse")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        public ActionResult<string> OperationWithNonNullableResponse()
        {
            return Ok();
        }

        [HttpPost("OperationWithNoXmlDocs")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        public ActionResult<string> OperationWithNoXmlDocs()
        {
            return Ok();
        }


        [HttpPost(nameof(OperationWithNullString))]
        public Task<string?> OperationWithNullString()
        {
            return Task.FromResult((string?)null);
        }
        [HttpPost(nameof(OperationWithNullObj))]
        public Task<NullableResponseController?> OperationWithNullObj()
        {
            return Task.FromResult((NullableResponseController?)null);
        }

        [HttpPost(nameof(OperationWithNullObjAndCanBeNull))]
        [return:CanBeNull] public Task<NullableResponseController?> OperationWithNullObjAndCanBeNull()
        {
            return Task.FromResult((NullableResponseController?)null);
        }

        [HttpPost(nameof(OperationWithNullObjAndNotNull))]
        [return:NotNull] public Task<NullableResponseController?> OperationWithNullObjAndNotNull()
        {
            return Task.FromResult((NullableResponseController?)null);
        }

    }
}
