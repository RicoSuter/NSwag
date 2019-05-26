//-----------------------------------------------------------------------
// <copyright file="SwaggerExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace NSwag.AspNetCore
{
    internal class OpenApiConfigureMvcOptions : ConfigureOptions<MvcOptions>
    {
        public OpenApiConfigureMvcOptions()
            : base(options => options.Conventions.Add(new OpenApiMvcApplicationModelConvention()))
        {
        }
    }
}