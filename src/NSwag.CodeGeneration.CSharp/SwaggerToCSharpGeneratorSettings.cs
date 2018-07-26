//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using System.Reflection;

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>Settings for the <see cref="SwaggerToCSharpGeneratorBase"/>.</summary>
    public abstract class SwaggerToCSharpGeneratorSettings : ClientGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpClientGeneratorSettings"/> class.</summary>
        protected SwaggerToCSharpGeneratorSettings()
        {
            CSharpGeneratorSettings = new CSharpGeneratorSettings
            {
                Namespace = "MyNamespace",
                SchemaType = SchemaType.Swagger2,
            };

            CSharpGeneratorSettings.TemplateFactory = new DefaultTemplateFactory(CSharpGeneratorSettings, new[]
            {
                typeof(CSharpGeneratorSettings).GetTypeInfo().Assembly,
                typeof(SwaggerToCSharpGeneratorSettings).GetTypeInfo().Assembly,
            });

            ResponseArrayType = "System.Collections.ObjectModel.ObservableCollection";
            ResponseDictionaryType = "System.Collections.Generic.Dictionary";

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