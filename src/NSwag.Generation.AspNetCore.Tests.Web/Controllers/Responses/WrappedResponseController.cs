using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using NSwag.Annotations;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Responses
{
    [ApiController]
    [Route( "api/wrappedresponse" )]
    public class WrappedResponseController : Controller
    {

        [HttpGet( "task" )]
        public async Task Task()
        {
            throw new NotImplementedException();
        }

        [HttpGet( "int" )]
        public int Int()
        {
            throw new NotImplementedException();
        }

        [HttpGet( "taskofint" )]
        public async Task<int> TaskOfInt()
        {
            throw new NotImplementedException();
        }

        [HttpGet( "valuetaskofint" )]
        public async ValueTask<int> ValueTaskOfInt()
        {
            throw new NotImplementedException();
        }

        [HttpGet( "actionresultofint" )]
        public ActionResult<int> ActionResultOfInt()
        {
            throw new NotImplementedException();
        }

        [HttpGet( "taskofactionresultofint" )]
        public async Task<ActionResult<int>> TaskOfActionResultOfInt()
        {
            throw new NotImplementedException();
        }

        [HttpGet( "valuetaskofactionresultofint" )]
        public async ValueTask<ActionResult<int>> ValueTaskOfActionResultOfInt()
        {
            throw new NotImplementedException();
        }
    }
}
