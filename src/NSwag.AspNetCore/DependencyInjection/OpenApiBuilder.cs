//-----------------------------------------------------------------------
// <copyright file="OpenApiBuilder.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace NSwag.AspNetCore.DependencyInjection
{
    internal class OpenApiBuilder : IOpenApiBuilder
    {
        public OpenApiBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
