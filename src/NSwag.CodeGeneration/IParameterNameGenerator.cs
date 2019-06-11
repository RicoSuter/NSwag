//-----------------------------------------------------------------------
// <copyright file="IParameterNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace NSwag.CodeGeneration
{
    /// <summary>The parameter name generator interface.</summary>
    public interface IParameterNameGenerator
    {
        /// <summary>Generates the parameter name for the given parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <returns>The parameter name.</returns>
        string Generate(OpenApiParameter parameter, IEnumerable<OpenApiParameter> allParameters);
    }
}