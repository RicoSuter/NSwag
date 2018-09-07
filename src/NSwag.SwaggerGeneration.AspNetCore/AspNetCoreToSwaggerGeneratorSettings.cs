//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.SwaggerGeneration.AspNetCore.Processors;

namespace NSwag.SwaggerGeneration.AspNetCore
{
    /// <summary>Settings for the <see cref="AspNetCoreToSwaggerGeneratorSettings"/>.</summary>
    public class AspNetCoreToSwaggerGeneratorSettings : SwaggerGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="AspNetCoreToSwaggerGeneratorSettings"/> class.</summary>
        public AspNetCoreToSwaggerGeneratorSettings()
        {
            OperationProcessors.Insert(3, new OperationParameterProcessor(this));
            OperationProcessors.Insert(3, new OperationResponseProcessor(this));
        }
    }
}