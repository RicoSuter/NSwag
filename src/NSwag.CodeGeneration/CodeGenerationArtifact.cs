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
	/// <summary>An output from the multi-file generation process.</summary>
	public class CodeGenerationArtifact
	{
		private readonly CodeArtifact _sourceArtifact;

		/// <summary>
		/// Main constructor for the type
		/// </summary>
		/// <param name="sourceArtifact">The source artifact from which this generated result was created</param>
		public CodeGenerationArtifact(CodeArtifact sourceArtifact)
		{
			_sourceArtifact = sourceArtifact;
		}

		/// <summary>
		/// The name of the type that was generated
		/// </summary>
		public string TypeName => _sourceArtifact.TypeName;

		/// <summary>
		/// The name of the file the code will be placed in
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// The type of artifact that was generated
		/// </summary>
		public CodeArtifactType Type => _sourceArtifact.Type;

		/// <summary>
		/// The language of the generated artifact
		/// </summary>
		public CodeArtifactLanguage Language => _sourceArtifact.Language;

		/// <summary>
		/// The category of the generated artifact
		/// </summary>
		public CodeArtifactCategory Category => _sourceArtifact.Category;

		/// <summary>
		/// The code that was generated for this artifact
		/// </summary>
		public string Code { get; set; }
	}
}