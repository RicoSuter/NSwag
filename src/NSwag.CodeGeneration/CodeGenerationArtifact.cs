//-----------------------------------------------------------------------
// <copyright file="ClientGeneratorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration
{
	/// <summary>The generated file.</summary>
	public class CodeGenerationArtifact
	{
		/// <summary>Name of generated file.</summary>
		public string Name { get; }
		/// <summary>Type of generated file.</summary>
		public CodeArtifactType Type { get; }
		/// <summary>The generated code.</summary>
		public string Code { get; set; }
	}
}