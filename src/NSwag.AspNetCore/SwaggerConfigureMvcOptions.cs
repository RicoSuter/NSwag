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
    internal class SwaggerConfigureMvcOptions : ConfigureOptions<MvcOptions>
    {
        public SwaggerConfigureMvcOptions()
            : base(options => options.Conventions.Add(new SwaggerMvcApplicationModelConvention()))
        {
        }
    }
}