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
	public class CodeGenerationArtifact
	{
		public string Name { get; }
		public CodeArtifactType Type { get; }
		public string Code { get; set; }
	}
}