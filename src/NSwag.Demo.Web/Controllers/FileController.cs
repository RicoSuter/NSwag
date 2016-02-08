using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NSwag.Demo.Web.Controllers
{
    public class FileController : ApiController
    {
        public HttpResponseMessage GetFile()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(new byte[] { });
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Sample.png";
            return response;
        }
    }
}