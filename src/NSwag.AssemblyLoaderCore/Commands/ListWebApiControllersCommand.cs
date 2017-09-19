//-----------------------------------------------------------------------
// <copyright file="ListWebApiControllersCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.Commands
{
    /// <summary>The generator.</summary>
    /// <seealso cref="NSwag.Commands.WebApiToSwaggerCommandBase" />
    public class ListWebApiControllersCommand : ListWebApiControllersCommandBase
    {
        /// <summary>Creates a new generator instance.</summary>
        /// <returns>The generator.</returns>
        /// <summary>Creates a new generator instance.</summary>
        /// <returns>The generator.</returns>
        /// <exception cref="InvalidOperationException">Configuraiton file does not contain WebApiToSwagger settings.</exception>
        protected override async Task<WebApiAssemblyToSwaggerGeneratorBase> CreateGeneratorAsync()
        {
            if (!string.IsNullOrEmpty(File))
            {
                var document = await NSwagDocument.LoadAsync(File);

                var settings = document.SwaggerGenerators?.WebApiToSwaggerCommand?.Settings;
                if (settings == null)
                    throw new InvalidOperationException("Configuraiton file does not contain WebApiToSwagger settings.");

                return new WebApiAssemblyToSwaggerGenerator(settings);
            }
            else
                return new WebApiAssemblyToSwaggerGenerator(Settings);
        }
    }
}