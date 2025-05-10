//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using System.Reflection;

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>Settings for the <see cref="CSharpGeneratorBase"/>.</summary>
    public abstract class CSharpGeneratorBaseSettings : ClientGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="CSharpClientGeneratorSettings"/> class.</summary>
        protected CSharpGeneratorBaseSettings()
        {
            CSharpGeneratorSettings = new CSharpGeneratorSettings
            {
                Namespace = "MyNamespace",
                SchemaType = SchemaType.Swagger2
            };

            CSharpGeneratorSettings.TemplateFactory = new DefaultTemplateFactory(CSharpGeneratorSettings, [
                typeof(CSharpGeneratorSettings).GetTypeInfo().Assembly,
                typeof(CSharpGeneratorBaseSettings).GetTypeInfo().Assembly
            ]);

            ResponseArrayType = "System.Collections.Generic.ICollection";
            ResponseDictionaryType = "System.Collections.Generic.IDictionary";

            ParameterArrayType = "System.Collections.Generic.IEnumerable";
            ParameterDictionaryType = "System.Collections.Generic.IDictionary";

            AdditionalNamespaceUsages = [];
            AdditionalContractNamespaceUsages = [];
        }

        /// <summary>Gets the CSharp generator settings.</summary>
        public CSharpGeneratorSettings CSharpGeneratorSettings { get; }

        /// <summary>Gets the code generator settings.</summary>
        [JsonIgnore]
        public override CodeGeneratorSettingsBase CodeGeneratorSettings => CSharpGeneratorSettings;

        /// <summary>Gets or sets the additional namespace usages.</summary>
        public string[] AdditionalNamespaceUsages { get; set; }

        /// <summary>Gets or sets the additional contract namespace usages.</summary>
        public string[] AdditionalContractNamespaceUsages { get; set; }

        /// <summary>Gets or sets the array type of operation responses (i.e. the method return type).</summary>
        public string ResponseArrayType { get; set; }

        /// <summary>Gets or sets the dictionary type of operation responses (i.e. the method return type).</summary>
        public string ResponseDictionaryType { get; set; }

        /// <summary>Gets or sets the array type of operation parameters.</summary>
        public string ParameterArrayType { get; set; }

        /// <summary>Gets or sets the dictionary type of operation parameters.</summary>
        public string ParameterDictionaryType { get; set; }
    }
}