//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.Commands
{
    /// <summary>The generator.</summary>
    /// <seealso cref="NSwag.Commands.WebApiToSwaggerCommandBase" />
    public class WebApiToSwaggerCommand : WebApiToSwaggerCommandBase
    {
        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerCommand"/> class.</summary>
        public WebApiToSwaggerCommand()
            : base(new WebApiAssemblyToSwaggerGeneratorSettings())
        {
        }

        /// <summary>Creates a new generator instance.</summary>
        /// <returns>The generator.</returns>
        protected override Task<WebApiAssemblyToSwaggerGeneratorBase> CreateGeneratorAsync()
        {
            return Task.FromResult<WebApiAssemblyToSwaggerGeneratorBase>(new WebApiAssemblyToSwaggerGenerator(Settings));
        }
    }
}
