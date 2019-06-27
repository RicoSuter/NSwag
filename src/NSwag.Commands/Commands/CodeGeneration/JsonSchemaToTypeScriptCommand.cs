//-----------------------------------------------------------------------
// <copyright file="JsonSchemaToTypeScriptCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NConsole;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;

#pragma warning disable 1591

namespace NSwag.Commands.CodeGeneration
{
    [Command(Name = "jsonschema2tsclient", Description = "Generates TypeScript interfaces from a JSON Schema.")]
    public class JsonSchemaToTypeScriptCommand : InputOutputCommandBase
    {
        [Argument(Name = "Name", Description = "The type name of the root schema.")]
        public string Name { get; set; }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var schema = await GetJsonSchemaAsync().ConfigureAwait(false);
            var generator = new TypeScriptGenerator(schema);

            var code = generator.GenerateFile(Name);
            await TryWriteFileOutputAsync(host, () => code).ConfigureAwait(false);
            return Task.FromResult<object>(code);
        }
    }
}