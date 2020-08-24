//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>Settings for the <see cref="CSharpGeneratorBase"/>.</summary>
    public abstract class CSharpGeneratorBaseSettings : ClientGeneratorBaseSettings
    {
        private bool _cSharpNamingConvention;

        /// <summary>Initializes a new instance of the <see cref="CSharpClientGeneratorSettings"/> class.</summary>
        protected CSharpGeneratorBaseSettings()
        {
            CSharpGeneratorSettings = new CSharpGeneratorSettings
            {
                Namespace = "MyNamespace",
                SchemaType = SchemaType.Swagger2
            };

            CSharpGeneratorSettings.TemplateFactory = new DefaultTemplateFactory(CSharpGeneratorSettings, new[]
            {
                typeof(CSharpGeneratorSettings).GetTypeInfo().Assembly,
                typeof(CSharpGeneratorBaseSettings).GetTypeInfo().Assembly,
            });

            ResponseArrayType = "System.Collections.Generic.ICollection";
            ResponseDictionaryType = "System.Collections.Generic.IDictionary";

            ParameterArrayType = "System.Collections.Generic.IEnumerable";
            ParameterDictionaryType = "System.Collections.Generic.IDictionary";

            AdditionalNamespaceUsages = new string[0];
            AdditionalContractNamespaceUsages = new string[0];
        }

        /// <summary>Gets the CSharp generator settings.</summary>
        public CSharpGeneratorSettings CSharpGeneratorSettings { get; }

        /// <summary>Gets the code generator settings.</summary>
        [JsonIgnore]
        public override CodeGeneratorSettingsBase CodeGeneratorSettings => CSharpGeneratorSettings;

        /// <summary>Gets or sets a value indicating whether to generate C# style naming (default: false).</summary>
        public bool CSharpNamingConvention
        {
            get => _cSharpNamingConvention;
            set
            {
                _cSharpNamingConvention = value;

                // Install the C# style name generators if desired
                if (!_cSharpNamingConvention) return;
                CSharpGeneratorSettings.PropertyNameGenerator = new CSharpPropertyNameGenerator();
                CSharpGeneratorSettings.TypeNameGenerator = new CSharpTypeNameGenerator();
                CSharpGeneratorSettings.EnumNameGenerator = new CSharpEnumNameGenerator();
            }
        }

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