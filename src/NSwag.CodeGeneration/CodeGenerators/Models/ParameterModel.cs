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
    /// <summary>This is model which contains information about single parameter. It will be passed into <see cref="ITemplate"/> to generate client.</summary>
    public class ParameterModel
    {
        private readonly SwaggerOperation _operation;
        private readonly SwaggerParameter _parameter;
        private readonly CodeGeneratorSettingsBase _settings;

        /// <summary>Initializes a new instance of the <see cref="ParameterModel"/> class.</summary>
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

        /// <summary>Gets the type.</summary>
        public string Type { get; }

        /// <summary>Gets the name.</summary>
        public string Name => _parameter.Name;

        /// <summary>Gets the variable name as lowercase.</summary>
        public string VariableNameLower => ConversionUtilities.ConvertToLowerCamelCase(_parameter.Name.Replace("-", "_").Replace(".", "_"), true);

        /// <summary>Gets the kind.</summary>
        public SwaggerParameterKind Kind => _parameter.Kind;

        /// <summary>Gets the description.</summary>
        public string Description => ConversionUtilities.TrimWhiteSpaces(_parameter.Description);

        /// <summary>Gets the schema.</summary>
        public JsonSchema4 Schema => _parameter.ActualSchema;

        /// <summary>Gets a value indicating whether this parameter is required.</summary>
        public bool IsRequired => _parameter.IsRequired;

        /// <summary>Gets a value indicating whether this parameter is nullable.</summary>
        public bool IsNullable => _parameter.IsNullable(_settings.NullHandling);

        /// <summary>Gets a value indicating whether this parameter is optional.</summary>
        public bool IsOptional => _parameter.IsRequired == false;

        /// <summary>Gets a value indicating whether this parameter has description.</summary>
        public bool HasDescription => !string.IsNullOrEmpty(Description);

        /// <summary>Gets a value indicating whether this parameter has description or is optional.</summary>
        public bool HasDescriptionOrIsOptional => HasDescription || !IsRequired;

        /// <summary>Gets a value indicating whether this parameter is last on argument list.</summary>
        public bool IsLast => _operation.ActualParameters.LastOrDefault() == _parameter;

        /// <summary>Gets a value indicating whether this parameter is date.</summary>
        public bool IsDate => Schema.Type == JsonObjectType.String && Schema.Format == JsonFormatStrings.DateTime;

        /// <summary>Gets a value indicating whether this parameter is array.</summary>
        public bool IsArray => Schema.Type.HasFlag(JsonObjectType.Array);

        /// <summary>Gets a value indicating whether this parameter is file.</summary>
        public bool IsFile => Schema.Type.HasFlag(JsonObjectType.File);

        /// <summary>Gets a value indicating whether this parameter is dictionary.</summary>
        public bool IsDictionary => Schema.IsDictionary;

        /// <summary>Gets a value indicating whether this parameter is date array.</summary>
        public bool IsDateArray => IsArray && Schema.Item?.Format == JsonFormatStrings.DateTime;

        // TODO: Find way to remove TypeScript only properties

        /// <summary>Gets or sets a value indicating whether DTO classes should be used.</summary>
        public bool UseDtoClass { get; set; } = false;
    }
}