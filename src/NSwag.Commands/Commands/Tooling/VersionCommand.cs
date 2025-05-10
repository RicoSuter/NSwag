﻿//-----------------------------------------------------------------------
// <copyright file="VersionCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NConsole;
using NJsonSchema;

namespace NSwag.Commands.Tooling
{
    /// <summary>Prints the tool chain version.</summary>
    [Command(Name = "version", Description = "Prints the toolchain version.")]
    public class VersionCommand : IConsoleCommand
    {
        /// <summary>Runs the command.</summary>
        /// <param name="processor">The processor.</param>
        /// <param name="host">The host.</param>
        /// <returns>The output.</returns>
        public Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            host.WriteMessage("\nNSwag version: " + OpenApiDocument.ToolchainVersion + "\n");
            host.WriteMessage("NJsonSchema version: " + JsonSchema.ToolchainVersion + "\n");
            return Task.FromResult<object>(null);
        }
    }
}