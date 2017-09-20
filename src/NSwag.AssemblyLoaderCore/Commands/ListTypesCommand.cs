//-----------------------------------------------------------------------
// <copyright file="ListTypesCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using NSwag.SwaggerGeneration;

namespace NSwag.Commands
{
    /// <summary>The generator.</summary>
    /// <seealso cref="NSwag.Commands.WebApiToSwaggerCommandBase" />
    public class ListTypesCommand : ListTypesCommandBase
    {
        /// <summary>Initializes a new instance of the <see cref="ListTypesCommand"/> class.</summary>
        public ListTypesCommand()
            : base(new AssemblyTypeToSwaggerGeneratorSettings())
        {
        }

        /// <summary>Creates a new generator instance.</summary>
        /// <returns>The generator.</returns>
        /// <summary>Creates a new generator instance.</summary>
        /// <returns>The generator.</returns>
        /// <exception cref="InvalidOperationException">Configuraiton file does not contain AssemblyTypeToSwagger settings.</exception>
        protected override async Task<AssemblyTypeToSwaggerGeneratorBase> CreateGeneratorAsync()
        {
            if (!string.IsNullOrEmpty(File))
            {
                var document = await NSwagDocument.LoadAsync(File);

                var settings = document.SwaggerGenerators?.AssemblyTypeToSwaggerCommand?.Settings;
                if (settings == null)
                    throw new InvalidOperationException("Configuraiton file does not contain AssemblyTypeToSwagger settings.");

                return new AssemblyTypeToSwaggerGenerator(settings);
            }
            else
                return new AssemblyTypeToSwaggerGenerator((AssemblyTypeToSwaggerGeneratorSettings)Settings);
        }
    }
}