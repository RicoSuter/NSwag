using System.Web.Http;

namespace NSwag.Demo.Web.Controllers
{
    [RoutePrefix("api/RoutePrefixWithPaths/{companyIdentifier:guid}")]
    public class RoutePrefixWithPathsController : ApiController
    {
        [HttpGet]
        [Route("documents")]
        //[CompanyFilterAuthorization(permission: ApplicationPermissions.Company.SDL.Read)]
        public IHttpActionResult GetDocuments(string query)
        {
            return Ok(1);
        }
    }
}