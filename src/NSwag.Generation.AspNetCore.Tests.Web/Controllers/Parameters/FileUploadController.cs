using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : Controller
    {
        [HttpPost("UploadFile")]
        public ActionResult UploadFile([NotNull] IFormFile file, [FromForm, NotNull]string test)
        {
            return Ok();
        }

        [HttpPost("UploadFiles")]
        public ActionResult UploadFiles([NotNull, FromForm] IFormFile[] files, [FromForm, NotNull]string test)
        {
            return Ok();
        }

        [HttpPost("UploadAttachment")]
        public Task<IActionResult> UploadAttachment(
            [FromRoute][Required] string caseId,
            [FromForm] CaseAttachmentModel model)
        {
            return Task.FromResult<IActionResult>(Ok());
        }

        public class CaseAttachmentModel
        {
            public string? Description { get; set; }

            public IFormFile? Contents { get; set; }
        }

        [HttpPost("UploadAttachment2")]
        public Task<IActionResult> UploadAttachment2(
            [FromForm][Required] CaseAttachmentModel2 model,
            [Required] IFormFile contents)
        {
            return Task.FromResult<IActionResult>(Ok());
        }

        public class CaseAttachmentModel2
        {
            [Required]
            public string Title { get; init; }

            public int? MessageId { get; set; }
        }
    }
}