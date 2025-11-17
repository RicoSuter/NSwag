using Microsoft.AspNetCore.Mvc;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Responses
{
    [ApiController]
    [Route("api/wrappedresponse")]
    public class WrappedResponseController : Controller
    {
        [HttpGet("task")]
        public Task Task()
        {
            throw new NotImplementedException();
        }

        [HttpGet("int")]
        public int Int()
        {
            throw new NotImplementedException();
        }

        [HttpGet("taskofint")]
        public Task<int> TaskOfInt()
        {
            throw new NotImplementedException();
        }

        [HttpGet("valuetaskofint")]
        public ValueTask<int> ValueTaskOfInt()
        {
            throw new NotImplementedException();
        }

        [HttpGet("actionresultofint")]
        public ActionResult<int> ActionResultOfInt()
        {
            throw new NotImplementedException();
        }

        [HttpGet("taskofactionresultofint")]
        public Task<ActionResult<int>> TaskOfActionResultOfInt()
        {
            throw new NotImplementedException();
        }

        [HttpGet("valuetaskofactionresultofint")]
        public ValueTask<ActionResult<int>> ValueTaskOfActionResultOfInt()
        {
            throw new NotImplementedException();
        }
    }
}