//-----------------------------------------------------------------------
// <copyright file="AssemblyLoader.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using NSwag.AssemblyLoader.Utilities;

#if !FullNet
using NJsonSchema.Infrastructure;
using System.Runtime.Loader;
#endif

namespace NSwag.AssemblyLoader
{
#if FullNet
    public class AssemblyLoader : MarshalByRefObject
    {
#else
    public class AssemblyLoader
    {
        public AssemblyLoadContext Context { get; }

        public AssemblyLoader()
        {
            Context = AssemblyLoadContext.Default; // TODO: Switch back to new CustomAssemblyLoadContext(); ?
        }

#endif

        protected void RegisterReferencePaths(IEnumerable<string> referencePaths)
        {
#if FullNet
            var allReferencePaths = new List<string>(GetAllDirectories(AppDomain.CurrentDomain.SetupInformation.ApplicationBase));
#else
            var allReferencePaths = new List<string>();
#endif

            foreach (var path in referencePaths.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                allReferencePaths.Add(path);
                allReferencePaths.AddRange(GetAllDirectories(path));
            }

            // Add path to executable directory
            allReferencePaths.Add(Path.GetDirectoryName(typeof(AssemblyLoader).GetTypeInfo().Assembly.CodeBase.Replace("file:///", string.Empty)));
            allReferencePaths = allReferencePaths.Distinct().ToList();

#if FullNet
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
#else
            Context.Resolving += (context, args) =>
#endif
            {
                var separatorIndex = args.Name.IndexOf(",", StringComparison.Ordinal);
                var assemblyName = separatorIndex > 0 ? args.Name.Substring(0, separatorIndex) : args.Name;

#if FullNet
                var existingAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
                if (existingAssembly != null)
                    return existingAssembly;
#endif

                foreach (var path in allReferencePaths)
                {
                    var files = Directory.GetFiles(path, assemblyName + ".dll", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        try
                        {
#if FullNet
                            return Assembly.LoadFrom(file);
#else
                            var currentDirectory = DynamicApis.DirectoryGetCurrentDirectoryAsync().GetAwaiter().GetResult();
                            return Context.LoadFromAssemblyPath(PathUtilities.MakeAbsolutePath(file, currentDirectory));
#endif
                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine("AssemblyLoader.AssemblyResolve exception when loading DLL '" + file + "': \n" + exception.ToString());
                        }
                    }
                }

                try
                {
#if FullNet
                    return Assembly.Load(new AssemblyName(assemblyName));
#else
                    return Context.LoadFromAssemblyName(new AssemblyName(assemblyName));
#endif
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("AssemblyLoader.AssemblyResolve exception when loading assembly '" + assemblyName + "': \n" + exception.ToString());
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
