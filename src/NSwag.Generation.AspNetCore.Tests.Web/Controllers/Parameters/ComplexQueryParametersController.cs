﻿using Microsoft.AspNetCore.Mvc;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplexQueryParametersController : Controller
    {
        /// <param name="getListCommand">Foo.</param>
        [HttpGet]
        public ActionResult GetList([FromQuery]GetListCommand getListCommand)
        {
            return Ok();
        }

        public class GetListCommand
        {
            /// <example>42</example>
            /// <summary>Bar.</summary>
            public int? Required01 { get; set; }

            /// <summary>Baz.</summary>
            public int Required02 { get; set; } = 10; // TODO: Should be optional
        }
    }
}