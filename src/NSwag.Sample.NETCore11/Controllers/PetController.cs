using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace NSwag.Sample.NETCore11.Controllers
{
    [Route("pet")]
    public class PetController : Controller
    {
        /// <summary>
        /// Creates a <see cref="Pet"/>.
        /// </summary>
        /// <param name="pet">The <see cref="Pet"/> to create.</param>
        /// <returns><see cref="HttpStatusCode.OK"/> if successful, <see cref="HttpStatusCode.BadRequest"/> when iput validation fails.</returns>
        [HttpPost]
        public ActionResult Post([FromBody] Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPut]
        public ActionResult Put([FromBody] Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pet.Id == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("findByStatus/{skip:int?}/{sortOrder?}")]
        public ActionResult FindByStatus([FromQuery] Status status, [FromRoute] int skip = 0, [FromRoute] string sortOrder = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpGet("findByTags")]
        [Produces("application/yaml", Type = typeof(IEnumerable<Pet>))]
        public ActionResult FindByTags([FromQuery] string[] tags)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id, [FromHeader(Name = "api_key")] string apiKey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("{id}")]
        [Produces(typeof(Pet))]
        [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult Get([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }

    /// <summary>
    /// A domestic or tamed animal such as an opposum.
    /// </summary>
    public class Pet
    {
        public long Id { get; set; }

        public Category Category { get; set; }

        public string Name { get; set; }

        public string[] PhotoUrls { get; set; }

        public Tag[] Tags { get; set; }

        public Status Status { get; set; }
    }

    public class Category
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public enum Status
    {
        Available,
        Pending,
        Sold,
    }

    public class Tag
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
