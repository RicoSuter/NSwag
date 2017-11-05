//-----------------------------------------------------------------------
// <copyright file="ListTypesCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NConsole;
using NJsonSchema.Infrastructure;
using NSwag.AssemblyLoader.Utilities;

namespace NSwag.Commands.SwaggerGeneration
{
    [Command(Name = "list-types", Description = "List all types for the given assembly and settings.")]
    public class ListTypesCommand : IsolatedCommandBase<string[]>
    {
        [Argument(Name = "File", IsRequired = false, Description = "The nswag.json configuration file path.")]
        public string File { get; set; }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var classNames = await RunAsync();

            host.WriteMessage("\r\n");
            foreach (var className in classNames)
                host.WriteMessage(className + "\r\n");
            host.WriteMessage("\r\n");

            return classNames;
        }

        protected override async Task<string[]> RunIsolatedAsync(AssemblyLoader.AssemblyLoader assemblyLoader)
        {
            var assemblyPaths = AssemblyPaths;

            if (!string.IsNullOrEmpty(File))
            {
                var document = await NSwagDocument.LoadAsync(File);
                assemblyPaths = ((TypesToSwaggerCommand)document.SelectedSwaggerGenerator).AssemblyPaths;
            }

#if FullNet
            return PathUtilities.ExpandFileWildcards(assemblyPaths)
                .Select(Assembly.LoadFrom)
#else
            var currentDirectory = DynamicApis.DirectoryGetCurrentDirectoryAsync().GetAwaiter().GetResult();
            return PathUtilities.ExpandFileWildcards(assemblyPaths)
                .Select(p => assemblyLoader.Context.LoadFromAssemblyPath(PathUtilities.MakeAbsolutePath(p, currentDirectory)))
#endif
                .SelectMany(a => a.ExportedTypes)
                .Select(t => t.FullName)
                .OrderBy(c => c)
                .ToArray();
        }
    }
}