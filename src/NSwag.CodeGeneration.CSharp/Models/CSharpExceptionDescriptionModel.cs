//-----------------------------------------------------------------------
// <copyright file="CSharpOperationModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp exception description model.</summary>
    public class CSharpExceptionDescriptionModel
    {
        private readonly string _type;
        private readonly SwaggerToCSharpClientGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="CSharpExceptionDescriptionModel"/> class.</summary>
        /// <param name="type">The type.</param>
        /// <param name="description">The description.</param>
        /// <param name="settings">The settings.</param>
        public CSharpExceptionDescriptionModel(string type, string description, SwaggerToCSharpClientGeneratorSettings settings)
        {
            _type = type;
            _settings = settings;

            Description = !string.IsNullOrEmpty(description) ? description : "A server side error occurred."; 
        }

        /// <summary>Gets or sets the name of the type.</summary>
        public string Type => _settings.WrapDtoExceptions ? _settings.ExceptionClass + "{" + _type + "}" : _type;

        /// <summary>Gets or sets the description.</summary>
        public string Description { get; }
    }
}