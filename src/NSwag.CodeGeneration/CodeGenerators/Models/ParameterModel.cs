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
    internal class ParameterModel
    {
        private readonly SwaggerOperation _operation;
        private readonly SwaggerParameter _parameter;

        public ParameterModel(string typeName, SwaggerOperation operation, SwaggerParameter parameter)
        {
            Type = typeName;
            _operation = operation; 
            _parameter = parameter; 
        }

        public string Type { get; set; }

        public string Name => _parameter.Name;

        public string VariableNameLower => ConversionUtilities.ConvertToLowerCamelCase(_parameter.Name.Replace("-", "_").Replace(".", "_"));

        public SwaggerParameterKind Kind => _parameter.Kind;

        public string Description => ConversionUtilities.TrimWhiteSpaces(_parameter.Description);

        public JsonSchema4 Schema => _parameter.ActualSchema;

        public bool IsRequired => _parameter.IsRequired;

        public bool IsNullable => _parameter.IsNullable(PropertyNullHandling.Required);

        public bool IsOptional => _parameter.IsRequired == false; 

        public bool HasDescription => !string.IsNullOrEmpty(Description);

        public bool HasDescriptionOrIsOptional => HasDescription || !IsRequired;

        public bool IsLast => _operation.Parameters.LastOrDefault() == _parameter;

        public bool IsDate => Schema.Type == JsonObjectType.String && Schema.Format == JsonFormatStrings.DateTime;

        public bool IsArray => Schema.Type.HasFlag(JsonObjectType.Array);

        public bool IsDictionary => Schema.IsDictionary;

        public bool IsDateArray => IsArray && Schema.Item?.Format == JsonFormatStrings.DateTime;

        // TODO: Find way to remove TypeScript only properties

        public bool UseDtoClass { get; set; } = false;
    }
}