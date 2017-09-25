//-----------------------------------------------------------------------
// <copyright file="IAssemblySettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.SwaggerGeneration
{
    /// <summary>An interface describing a settings class with assembly loader settings.</summary>
    public interface IAssemblySettings
    {
        /// <summary>Gets or sets the Web API assembly paths.</summary>
        AssemblySettings AssemblySettings { get; }
    }
}