//-----------------------------------------------------------------------
// <copyright file="ClientGeneratorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace NSwag.CodeGeneration
{
	/// <summary>The result of multi-file code generation.</summary>
	public class CodeGenerationResult
	{
		/// <summary>Indicator of whether file generation was successful.</summary>
		public Result result;

		/// <summary>List of generated files.</summary>
		public IEnumerable<CodeGenerationArtifact> artifacts;
	}
}