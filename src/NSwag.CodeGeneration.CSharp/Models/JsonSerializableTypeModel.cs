//-----------------------------------------------------------------------
// <copyright file="JsonSerializableTypeModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>Describes a single C# type that needs a <c>[JsonSerializable]</c> entry on the generated
    /// <c>JsonSerializerContext</c> partial class.</summary>
    public sealed class JsonSerializableTypeModel
    {
        /// <summary>Initializes a new instance of the <see cref="JsonSerializableTypeModel"/> class.</summary>
        /// <param name="typeReference">The full C# type reference (e.g. <c>Foo</c>, <c>System.Collections.Generic.List&lt;Foo&gt;</c>).</param>
        /// <param name="typeInfoPropertyName">The stable property name exposed by the source-generated context (e.g. <c>ListOfFoo</c>).</param>
        public JsonSerializableTypeModel(string typeReference, string typeInfoPropertyName)
        {
            TypeReference = typeReference;
            TypeInfoPropertyName = typeInfoPropertyName;
        }

        /// <summary>Gets the C# type reference written inside <c>typeof(...)</c> in the emitted <c>[JsonSerializable]</c> attribute.</summary>
        public string TypeReference { get; }

        /// <summary>Gets the stable property name exposed on the source-generated context (used at call sites as
        /// <c>ContextClassName.Default.&lt;TypeInfoPropertyName&gt;</c>).</summary>
        public string TypeInfoPropertyName { get; }
    }
}
