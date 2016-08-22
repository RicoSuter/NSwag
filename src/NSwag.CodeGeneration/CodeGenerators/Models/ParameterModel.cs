//-----------------------------------------------------------------------
// <copyright file="ParameterModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration.CodeGenerators.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ParameterModel
    {
        private readonly SwaggerOperation _operation;
        private readonly SwaggerParameter _parameter;
        private readonly CodeGeneratorSettingsBase _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterModel"/> class.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="settings">The settings.</param>
        public ParameterModel(string typeName, SwaggerOperation operation, SwaggerParameter parameter, CodeGeneratorSettingsBase settings)
        {
            Type = typeName;
            _operation = operation; 
            _parameter = parameter;
            _settings = settings;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name => _parameter.Name;

        /// <summary>
        /// Gets the variable name lower.
        /// </summary>
        /// <value>
        /// The variable name lower.
        /// </value>
        public string VariableNameLower => ConversionUtilities.ConvertToLowerCamelCase(_parameter.Name.Replace("-", "_").Replace(".", "_"), true);

        /// <summary>
        /// Gets the kind.
        /// </summary>
        /// <value>
        /// The kind.
        /// </value>
        public SwaggerParameterKind Kind => _parameter.Kind;

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description => ConversionUtilities.TrimWhiteSpaces(_parameter.Description);

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <value>
        /// The schema.
        /// </value>
        public JsonSchema4 Schema => _parameter.ActualSchema;

        /// <summary>
        /// Gets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public bool IsRequired => _parameter.IsRequired;

        /// <summary>
        /// Gets a value indicating whether this instance is nullable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is nullable; otherwise, <c>false</c>.
        /// </value>
        public bool IsNullable => _parameter.IsNullable(_settings.NullHandling);

        /// <summary>
        /// Gets a value indicating whether this instance is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is optional; otherwise, <c>false</c>.
        /// </value>
        public bool IsOptional => _parameter.IsRequired == false;

        /// <summary>
        /// Gets a value indicating whether this instance has description.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has description; otherwise, <c>false</c>.
        /// </value>
        public bool HasDescription => !string.IsNullOrEmpty(Description);

        /// <summary>
        /// Gets a value indicating whether this instance has description or is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has description or is optional; otherwise, <c>false</c>.
        /// </value>
        public bool HasDescriptionOrIsOptional => HasDescription || !IsRequired;

        /// <summary>
        /// Gets a value indicating whether this instance is last.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is last; otherwise, <c>false</c>.
        /// </value>
        public bool IsLast => _operation.ActualParameters.LastOrDefault() == _parameter;

        /// <summary>
        /// Gets a value indicating whether this instance is date.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is date; otherwise, <c>false</c>.
        /// </value>
        public bool IsDate => Schema.Type == JsonObjectType.String && Schema.Format == JsonFormatStrings.DateTime;

        /// <summary>
        /// Gets a value indicating whether this instance is array.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is array; otherwise, <c>false</c>.
        /// </value>
        public bool IsArray => Schema.Type.HasFlag(JsonObjectType.Array);

        /// <summary>
        /// Gets a value indicating whether this instance is file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is file; otherwise, <c>false</c>.
        /// </value>
        public bool IsFile => Schema.Type.HasFlag(JsonObjectType.File);

        /// <summary>
        /// Gets a value indicating whether this instance is dictionary.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dictionary; otherwise, <c>false</c>.
        /// </value>
        public bool IsDictionary => Schema.IsDictionary;

        /// <summary>
        /// Gets a value indicating whether this instance is date array.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is date array; otherwise, <c>false</c>.
        /// </value>
        public bool IsDateArray => IsArray && Schema.Item?.Format == JsonFormatStrings.DateTime;

        // TODO: Find way to remove TypeScript only properties

        /// <summary>
        /// Gets or sets a value indicating whether [use dto class].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use dto class]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDtoClass { get; set; } = false;
    }
}