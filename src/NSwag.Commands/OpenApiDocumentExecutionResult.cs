//-----------------------------------------------------------------------
// <copyright file="SwaggerDocumentExecutionResult.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace NSwag.Commands
{
    /// <summary>Stores the result of a <see cref="OpenApiDocument"/> execution.</summary>
    public class OpenApiDocumentExecutionResult
    {
        private readonly IDictionary<Type, string> _generatorOutputs = new Dictionary<Type, string>();

        /// <summary>Initializes a new instance of the <see cref="NSwagDocumentBase"/> class.</summary>
        /// <param name="output">The command line output.</param>
        /// <param name="swaggerOutput">The Swagger JSON output.</param>
        /// <param name="isRedirectedOutput">Indicates whether the output is redirect.</param>
        public OpenApiDocumentExecutionResult(string output, string swaggerOutput, bool isRedirectedOutput)
        {
            Output = output;
            SwaggerOutput = swaggerOutput;
            IsRedirectedOutput = isRedirectedOutput;
        }

        /// <summary>Gets the command line output.</summary>
        public string Output { get; }

        /// <summary>Gets the Swagger JSON output.</summary>
        public string SwaggerOutput { get; }

        /// <summary>Gets a value indicating whether the output is redirect.</summary>
        public bool IsRedirectedOutput { get; }

        /// <summary>Adds a generator output (e.g. code) to the result</summary>
        /// <param name="key">The type of the generator command.</param>
        /// <param name="output">The output string.</param>
        public void AddGeneratorOutput(Type key, string output)
        {
            _generatorOutputs[key] = output;
        }

        /// <summary>Gets a genrator output with a generator command type key.</summary>
        /// <typeparam name="T">The generator command type.</typeparam>
        /// <returns>The output.</returns>
        public string GetGeneratorOutput<T>()
        {
            return _generatorOutputs.ContainsKey(typeof(T)) ? _generatorOutputs[typeof(T)] : null;
        }
    }
}