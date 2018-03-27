using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NSwag.Sample
{
    // This sample is similar to http://petstore.swagger.io/#/
    [Route("/pet")]
    public class PetController : ControllerBase
    {
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddPet([FromBody] Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await Task.Delay(0);
            return new EmptyResult();
        }

        [HttpPut]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> EditPet([FromBody] Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pet.Id == 0)
            {
                return NotFound();
            }

            await Task.Delay(0);
            return new EmptyResult();
        }

        // 'status' is intended to be an optional query string parameter
        [HttpGet("findByStatus")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Pet>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> FindByStatus([FromQuery] string[] status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await Task.Delay(0);
            return new ObjectResult(Array.Empty<Pet>());
        }

        // Included this extra action not present in http://petstore.swagger.io/#/ 
        // to represent an action with a required query parameter.
        [HttpGet("findByCategory")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Pet>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> FindByCategory([FromQuery] string category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await Task.Delay(0);
            return new ObjectResult(Array.Empty<Pet>());
        }


        [HttpGet("{petId}", Name = "FindPetById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Pet), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> FindById(int petId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await Task.Delay(0);
            if (petId == 0)
            {
                return NotFound();
            }

            return Ok(new Pet());
        }

        [HttpPost("{petId}")]
        [Consumes("application/www-form-url-encoded")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> EditPet(int petId, [FromForm] Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (petId == 0)
            {
                return NotFound();
            }

            await Task.Delay(0);
            return new EmptyResult();
        }

        [HttpDelete("{petId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeletePet(int petId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (petId == 0)
            {
                return NotFound();
            }

            await Task.Delay(0);
            return new EmptyResult();
        }


        [HttpPost("{petId}/uploadImage")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UploadImage(int petId, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (petId == 0)
            {
                return NotFound();
            }

            await Task.Delay(0);
            return Ok(new ApiResponse());
        }
    }
}