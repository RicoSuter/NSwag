//-----------------------------------------------------------------------
// <copyright file="AssemblyTypeToSwaggerCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.CodeGeneration.SwaggerGenerators;
using NSwag.Commands;

namespace NSwag.CodeGeneration.Commands
{
    /// <summary></summary>
    /// <seealso cref="NSwag.Commands.AssemblyTypeToSwaggerCommandBase" />
    public class AssemblyTypeToSwaggerCommand : AssemblyTypeToSwaggerCommandBase
    {

        /// <summary>Creates a new generator instance.</summary>
        /// <returns>The generator.</returns>
        protected override AssemblyTypeToSwaggerGeneratorBase CreateGenerator()
        {
            return new AssemblyTypeToSwaggerGenerator(Settings);
        }
    }
}