//-----------------------------------------------------------------------
// <copyright file="ParameterModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;

namespace NSwag.CodeGeneration.CodeGenerators.Models
{
    internal class ParameterModel
    {
        public string Name { get; set; }

        public string VariableNameLower { get; set; }

        public SwaggerParameterKind Kind { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public JsonSchema4 Schema { get; set; }

        public bool IsRequired { get; set; }

        public bool HasDescription => !string.IsNullOrEmpty(Description);

        public bool HasDescriptionOrIsOptional => HasDescription || IsOptional;

        public bool IsLast { get; set; }

        public bool IsDate => Schema.Type == JsonObjectType.String && Schema.Format == JsonFormatStrings.DateTime;

        public bool IsArray => Schema.Type == JsonObjectType.Array;

        public bool IsDateArray => IsArray && Schema.Item?.Format == JsonFormatStrings.DateTime;

        public bool IsOptional => !IsRequired;
    }
}