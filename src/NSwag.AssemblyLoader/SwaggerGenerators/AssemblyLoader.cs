//-----------------------------------------------------------------------
// <copyright file="AssemblyLoader.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NSwag.CodeGeneration.SwaggerGenerators
{
    internal class AssemblyLoader : MarshalByRefObject
    {
        protected void RegisterReferencePaths(IEnumerable<string> referencePaths)
        {
            var domain = AppDomain.CurrentDomain;

            var allReferencePaths = new List<string>(GetAllDirectories(domain.SetupInformation.ApplicationBase));
            foreach (var path in referencePaths.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                allReferencePaths.Add(path);
                allReferencePaths.AddRange(GetAllDirectories(path));
            }

            // Add path to nswag directory
            allReferencePaths.Add(
                Path.GetDirectoryName(typeof(AssemblyLoader).Assembly.CodeBase.Replace("file:///", string.Empty)));
            allReferencePaths = allReferencePaths.Distinct().ToList();

            domain.AssemblyResolve += (sender, args) =>
            {
                var separatorIndex = args.Name.IndexOf(",", StringComparison.InvariantCulture);
                var assemblyName = separatorIndex > 0 ? args.Name.Substring(0, separatorIndex) : args.Name;

                var existingAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
                if (existingAssembly != null)
                    return existingAssembly;

                foreach (var path in allReferencePaths)
                {
                    var files = Directory.GetFiles(path, assemblyName + ".dll", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        try
                        {
                            return Assembly.LoadFrom(file);
                        }
                        catch
                        {
                        }
                    }
                }

                return null;
            };
        }

        private string[] GetAllDirectories(string rootDirectory)
        {
            return Directory.GetDirectories(rootDirectory, "*", SearchOption.AllDirectories);
        }
    }
}
