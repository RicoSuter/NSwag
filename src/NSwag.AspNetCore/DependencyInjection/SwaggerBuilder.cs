//-----------------------------------------------------------------------
// <copyright file="SwaggerBuilder.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace Microsoft.Extensions.DependencyInjection
{
    internal class SwaggerBuilder : ISwaggerBuilder
    {
        public SwaggerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
