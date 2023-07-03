//-----------------------------------------------------------------------
// <copyright file="ClientGeneratorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration
{
	/// <summary>Indicates whether a code generation attempt has been successful.</summary>
	public enum Result
	{
		/// <summary>Code generation was a success.</summary>
		Success,

		/// <summary>Code generation was a failure.</summary>
		Failure
	}
}