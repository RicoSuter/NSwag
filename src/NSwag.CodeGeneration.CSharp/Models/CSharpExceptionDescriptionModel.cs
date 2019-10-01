//-----------------------------------------------------------------------
// <copyright file="CSharpOperationModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp exception description model.</summary>
    public class CSharpExceptionDescriptionModel
    {
        private readonly string _type;
        private readonly CSharpClientGeneratorSettings _settings;
        private readonly string _controllerName;

        /// <summary>Initializes a new instance of the <see cref="CSharpExceptionDescriptionModel" /> class.</summary>
        /// <param name="type">The type.</param>
        /// <param name="description">The description.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="settings">The settings.</param>
        public CSharpExceptionDescriptionModel(string type, string description, string controllerName, CSharpClientGeneratorSettings settings)
        {
            _type = type;
            _settings = settings;
            _controllerName = controllerName;

            Description = !string.IsNullOrEmpty(description) ? description : "A server side error occurred.";
        }

        /// <summary>Gets or sets the name of the type.</summary>
        public string Type
        {
            get
            {
                if (_settings.WrapDtoExceptions)
                {
                    var exceptionClass = _settings.ExceptionClass.Replace("{controller}", _controllerName);
                    return _type == "void" ? exceptionClass : exceptionClass + "{" + _type + "}";
                }

                return _type;
            }
        }

        /// <summary>Gets or sets the description.</summary>
        public string Description { get; }
    }
}