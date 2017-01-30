//-----------------------------------------------------------------------
// <copyright file="ParameterModelBase.cs" company="NSwag">
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
    /// <summary>The parameter template model.</summary>
    public abstract class ParameterModelBase
    {
        private readonly SwaggerOperation _operation;
        private readonly SwaggerParameter _parameter;
        private readonly CodeGeneratorSettingsBase _settings;
        private readonly IClientGenerator _generator;

        /// <summary>Initializes a new instance of the <see cref="ParameterModelBase" /> class.</summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator base.</param>
        protected ParameterModelBase(string typeName, SwaggerOperation operation, SwaggerParameter parameter,
            string parameterName, string variableName, CodeGeneratorSettingsBase settings, IClientGenerator generator)
        {
            Type = typeName;
            Name = parameterName;
            VariableName = variableName;

            _operation = operation;
            _parameter = parameter;
            _settings = settings;
            _generator = generator;
        }

        /// <summary>Gets the type of the parameter.</summary>
        public string Type { get; }

        /// <summary>Gets the name.</summary>
        public string Name { get; }
        /// <summary>Gets the variable name in (usually lowercase).</summary>
        public string VariableName { get; }

        /// <summary>Gets the parameter kind.</summary>
        public SwaggerParameterKind Kind => _parameter.Kind;

        /// <summary>Gets a value indicating whether the parameter has a description.</summary>
        public bool HasDescription => !string.IsNullOrEmpty(Description);

        /// <summary>Gets the parameter description.</summary>
        public string Description => ConversionUtilities.TrimWhiteSpaces(_parameter.Description);

        /// <summary>Gets the schema.</summary>
        public JsonSchema4 Schema => _parameter.ActualSchema;

        /// <summary>Gets a value indicating whether the parameter is required.</summary>
        public bool IsRequired => _parameter.IsRequired;

        /// <summary>Gets a value indicating whether the parameter is nullable.</summary>
        public bool IsNullable => _parameter.IsNullable(_settings.NullHandling);

        /// <summary>Gets a value indicating whether the parameter is optional (i.e. not required).</summary>
        public bool IsOptional => _parameter.IsRequired == false;

        /// <summary>Gets a value indicating whether the parameter has a description or is optional.</summary>
        public bool HasDescriptionOrIsOptional => HasDescription || !IsRequired;

        /// <summary>Gets a value indicating whether the parameter is the last parameter of the operation.</summary>
        public bool IsLast => _operation.ActualParameters.LastOrDefault() == _parameter;

        /// <summary>Gets a value indicating whether this is an XML body parameter.</summary>
        public bool IsXmlBodyParameter => _parameter.IsXmlBodyParameter;

        /// <summary>Gets a value indicating whether the parameter is of type date.</summary>
        public bool IsDate =>
            (Schema.Format == JsonFormatStrings.DateTime ||
            Schema.Format == JsonFormatStrings.Date) &&
            _generator.GetTypeName(Schema, IsNullable, "Response") != "string";

        /// <summary>Gets a value indicating whether the parameter is of type array.</summary>
        public bool IsArray => Schema.Type.HasFlag(JsonObjectType.Array) || _parameter.CollectionFormat == SwaggerParameterCollectionFormat.Multi;

        /// <summary>Gets a value indicating whether this is a file parameter.</summary>
        public bool IsFile => Schema.Type.HasFlag(JsonObjectType.File);

        /// <summary>Gets a value indicating whether the parameter is of type dictionary.</summary>
        public bool IsDictionary => Schema.IsDictionary;

        /// <summary>Gets a value indicating whether the parameter is of type date array.</summary>
        public bool IsDateArray =>
            IsArray &&
            (Schema.Item?.ActualSchema.Format == JsonFormatStrings.DateTime ||
            Schema.Item?.ActualSchema.Format == JsonFormatStrings.Date) &&
            _generator.GetTypeName(Schema.Item.ActualSchema, IsNullable, "Response") != "string";

        /// <summary>Gets a value indicating whether the parameter is of type object array.</summary>
        public bool IsObjectArray => IsArray && (Schema.Item?.Type == JsonObjectType.Object || Schema.Item?.IsAnyType == true);
    }
}
