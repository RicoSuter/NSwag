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

namespace NSwag.AssemblyLoader.SwaggerGenerators
{
    internal class AssemblyLoader : MarshalByRefObject
    {
        protected void RegisterReferencePaths(IEnumerable<string> referencePaths)
        {
            var domain = AppDomain.CurrentDomain;

            var allReferencePaths = new List<string>(GetAllDirectories(domain.SetupInformation.ApplicationBase));
            foreach (var path in referencePaths.Where(p => !string.IsNullOrWhiteSpace(p)))
                allReferencePaths.AddRange(GetAllDirectories(path));

            domain.AssemblyResolve += (sender, args) =>
            {
                foreach (var path in allReferencePaths)
                {
                    var assemblyName = args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.InvariantCulture)) + ".dll"; 
                    var files = Directory.GetFiles(path, assemblyName, SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        try
                        {
                            var assembly = Assembly.LoadFrom(file);
                            if (assembly.FullName == args.Name)
                                return assembly;
                        }
                        catch
                        {
                        }
                    }
                }
                return null;
            };
        }

        private static string[] GetAllDirectories(string rootDirectory)
        {
            return Directory.GetDirectories(rootDirectory, "*", SearchOption.AllDirectories);
        }
    }
}
