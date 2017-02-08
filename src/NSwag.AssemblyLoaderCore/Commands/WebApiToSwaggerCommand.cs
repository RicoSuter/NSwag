//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.Commands;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.Commands
{
    /// <summary></summary>
    /// <seealso cref="NSwag.Commands.WebApiToSwaggerCommandBase" />
    public class WebApiToSwaggerCommand : WebApiToSwaggerCommandBase
    {
        /// <summary>Creates a new generator instance.</summary>
        /// <returns>The generator.</returns>
        protected override WebApiAssemblyToSwaggerGeneratorBase CreateGenerator()
        {
            return new WebApiAssemblyToSwaggerGenerator(Settings);
        }
    }
}
