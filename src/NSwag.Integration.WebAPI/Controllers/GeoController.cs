using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using NSwag.Annotations;
using NSwag.Integration.WebAPI.Models;

namespace NSwag.Integration.WebAPI.Controllers
{
    public class GeoController : ApiController
    {
        public void FromBodyTest([FromBody] GeoPoint location)
        {

        }

        public void FromUriTest([FromUri] GeoPoint location)
        {

        }

        [HttpPost]
        public void AddPolygon(GeoPoint[] points)
        {

        }

        [ResponseType("204", typeof(void))]
        public object Refresh()
        {
            throw new NotSupportedException();
        }

        public void UploadFile(HttpPostedFileBase file)
        {
            
        }

        public void UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {

        }

        [HttpPost]
        public void SaveItems(GenericRequest<Address, Person> request)
        {
            
        }
    }
}