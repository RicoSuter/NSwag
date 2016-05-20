using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using NSwag.Annotations;

namespace NSwag.Demo.Web.Controllers
{
    public interface IFormFile
    {
    }

    public interface IActionResult
    {
    }

    public interface IFormFileCollection
    {
    }

    public class FileController : ApiController
    {
        [HttpPost, Route("upload")]
        public IActionResult Upload(HttpPostedFileBase myFile, [Required] IFormFile myFile2, ICollection<IFormFile> files, ICollection<HttpPostedFileBase> files2, IFormFileCollection files3)
        {
            throw new NotImplementedException();
        }

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