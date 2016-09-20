using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using NJsonSchema.Converters;

namespace NSwag.Integration.WebAPI
{
    public class JsonExceptionFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext.Exception != null)
            {
                var json = JsonConvert.SerializeObject(actionExecutedContext.Exception, Formatting.None, new JsonExceptionConverter());
                actionExecutedContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(json, Encoding.UTF8)
                };
            }
            else
                await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}