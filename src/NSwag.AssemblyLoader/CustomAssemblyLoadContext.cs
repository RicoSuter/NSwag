﻿//-----------------------------------------------------------------------
// <copyright file="CustomAssemblyLoadContext.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using NJsonSchema.Infrastructure;
using NSwag.AssemblyLoader.Utilities;

namespace NSwag.AssemblyLoader
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly HashSet<string> _assembliesLoadedByAssemblyName = new HashSet<string>();
        private bool _isResolving = false;

        internal Dictionary<string, Assembly> Assemblies { get; } = new Dictionary<string, Assembly>();

        internal List<string> AllReferencePaths { get; set; } = new List<string>();

        public Assembly Resolve(AssemblyName args)
        {
            if (!_isResolving)
            {
                return Load(args);
            }
            else
            {
                return null;
            }
        }

        protected override Assembly Load(AssemblyName args)
        {
            if (Assemblies.ContainsKey(args.Name))
            {
                return Assemblies[args.Name];
            }

            var separatorIndex = args.Name.IndexOf(",", StringComparison.Ordinal);
            var assemblyName = separatorIndex > 0 ? args.Name.Substring(0, separatorIndex) : args.Name;

            var assembly = TryLoadExistingAssemblyName(args.FullName);
            if (assembly != null)
            {
                Assemblies[args.Name] = assembly;
                return assembly;
            }

            var version = args.Version;
            if (version != null)
            {
                // var assemblyByVersion = TryLoadByVersion(AllReferencePaths, assemblyName, version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision);
                // if (assemblyByVersion != null)
                // {
                //     Assemblies[args.Name] = assemblyByVersion;
                //     return assemblyByVersion;
                // }

                var assemblyByVersion = TryLoadByVersion(AllReferencePaths, assemblyName, version.Major + "." + version.Minor + "." + version.Build + ".");
                if (assemblyByVersion != null)
                {
                    Assemblies[args.Name] = assemblyByVersion;
                    return assemblyByVersion;
                }

                assemblyByVersion = TryLoadByVersion(AllReferencePaths, assemblyName, version.Major + "." + version.Minor + ".");
                if (assemblyByVersion != null)
                {
                    Assemblies[args.Name] = assemblyByVersion;
                    return assemblyByVersion;
                }

                assemblyByVersion = TryLoadByVersion(AllReferencePaths, assemblyName, version.Major + ".");
                if (assemblyByVersion != null)
                {
                    Assemblies[args.Name] = assemblyByVersion;
                    return assemblyByVersion;
                }
            }

            assembly = TryLoadByAssemblyName(args.FullName);
            if (assembly != null)
            {
                Assemblies[args.Name] = assembly;
                return assembly;
            }

            assembly = TryLoadByName(AllReferencePaths, assemblyName);
            if (assembly != null)
            {
                Assemblies[args.Name] = assembly;
                return assembly;
            }

            Assemblies[args.Name] = TryLoadByAssemblyName(assemblyName);
            return Assemblies[args.Name];
        }

        private Assembly TryLoadExistingAssemblyName(string assemblyName)
        {
            try
            {
                _isResolving = true;
                return Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
            }
            catch (Exception exception)
            {
                Debug.WriteLine("NSwag: AssemblyLoader exception when loading assembly by name in Default context '" +
                                assemblyName + "': \n" + exception);
            }
            finally
            {
                _isResolving = false;
            }

            return null;
        }

        private Assembly TryLoadByVersion(List<string> allReferencePaths, string assemblyName, string assemblyVersion)
        {
            foreach (var path in allReferencePaths)
            {
                var files = Directory.GetFiles(path, assemblyName + ".dll", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    try
                    {
                        var info = FileVersionInfo.GetVersionInfo(file);
                        if (info.FileVersion.StartsWith(assemblyVersion))
                        {
                            var assembly = TryLoadByPath(assemblyName, file);
                            if (assembly != null)
                            {
                                return assembly;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine("NSwag: AssemblyLoader exception when loading assembly by file '" + file + "': \n" + exception);
                    }
                }
            }

            return null;
        }

        private Assembly TryLoadByName(List<string> allReferencePaths, string assemblyName)
        {
            foreach (var path in allReferencePaths)
            {
                var files = Directory.GetFiles(path, assemblyName + ".dll", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    var assembly = TryLoadByPath(assemblyName, file);
                    if (assembly != null)
                    {
                        return assembly;
                    }
                }
            }

            return null;
        }

        private Assembly TryLoadByPath(string assemblyName, string file)
        {
            try
            {
                if (!file.EndsWith("/refs/" + assemblyName + ".dll") &&
                    !file.EndsWith("\\refs\\" + assemblyName + ".dll"))
                {
                    var currentDirectory = DynamicApis.DirectoryGetCurrentDirectory();
                    return LoadFromAssemblyPath(PathUtilities.MakeAbsolutePath(file, currentDirectory));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("NSwag: AssemblyLoader exception when loading assembly by file '" + file + "': \n" + exception);
            }

            return null;
        }

        private Assembly TryLoadByAssemblyName(string assemblyName)
        {
            if (!_assembliesLoadedByAssemblyName.Contains(assemblyName))
            {
                try
                {
                    _assembliesLoadedByAssemblyName.Add(assemblyName);
                    return LoadFromAssemblyName(new AssemblyName(assemblyName));
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("NSwag: AssemblyLoader exception when loading assembly by name '" + assemblyName + "': \n" + exception);
                }
            }

            return null;
        }
    }
}

#endif