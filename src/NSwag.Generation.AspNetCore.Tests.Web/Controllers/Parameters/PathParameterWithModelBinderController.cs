using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class PathParameterWithModelBinderController : Controller
    {
        [HttpGet("{stringModel}")]
        public ActionResult RequiredRouteParamWithModelBinder([ModelBinder(typeof(StringModelBinder))] string stringModel)
        {
            return Ok();
        }

        public class StringModelBinder : IModelBinder
        {
            public Task BindModelAsync(ModelBindingContext bindingContext)
            {
                bindingContext.Result =
                    ModelBindingResult.Success(
                        bindingContext.ValueProvider.GetValue(
                            bindingContext.ModelName));
                return Task.CompletedTask;
            }
        }
    }
}
