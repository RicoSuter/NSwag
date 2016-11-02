//-----------------------------------------------------------------------
// <copyright file="PathUtilities.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSwag.CodeGeneration.Utilities
{
    // TODO: Move to MyToolkit

    /// <summary>Provides file path utility methods.</summary>
    public static class PathUtilities
    {
        /// <summary>Expands the given wildcards (** or *) in the path.</summary>
        /// <param name="path">The file path with wildcards.</param>
        /// <returns>All expanded file paths.</returns>
        public static IEnumerable<string> ExpandFileWildcards(string path)
        {
            return ExpandFileWildcards(new[] { path });
        }

        /// <summary>Expands the given wildcards (** or *) in the paths.</summary>
        /// <param name="paths">The files path with wildcards.</param>
        /// <returns>All expanded file paths.</returns>
        public static IEnumerable<string> ExpandFileWildcards(IEnumerable<string> paths)
        {
            var allFiles = new List<string>();
            foreach (var path in paths)
            {
                if (path.Contains("*"))
                {
                    var starIndex = path.IndexOf("*", StringComparison.Ordinal);

                    var rootIndex = path.Substring(0, starIndex).LastIndexOf("\\", StringComparison.Ordinal);
                    if (rootIndex == -1)
                        rootIndex = path.Substring(0, starIndex).LastIndexOf("/", StringComparison.Ordinal);

                    var rootPath = rootIndex >= 0 ? path.Substring(0, rootIndex + 1) : Directory.GetCurrentDirectory();
                    var files = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);

                    var regex = new Regex(
                        "^" + 
                        Regex.Escape(path//.Substring(rootIndex + 1)
                        .Replace("**/", "__starstar__")
                        .Replace("**\\", "__starstar__")
                        .Replace("/", "__del__")
                        .Replace("\\", "__del__")
                        .Replace("*", "__star__"))
                        .Replace("__del__", "([\\\\/])")
                        .Replace("__starstar__", "((.*?)[/\\\\])")
                        .Replace("__star__", "([^\\/]*?)") + "$");

                    allFiles.AddRange(files
                        .Where(f => regex.Match(f).Success)
                        .Select(Path.GetFullPath));
                }
                else
                    allFiles.Add(path);
            }

            return allFiles.Distinct();
        }

        /// <summary>Converts a relative path to an absolute path.</summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="relativeTo">The current directory.</param>
        /// <returns>The absolute path.</returns>
        public static string MakeAbsolutePath(string relativePath, string relativeTo)
        {
            if (Path.IsPathRooted(relativePath))
                return relativePath; 

            var absolutePath = Path.Combine(relativeTo, relativePath);
            return Path.GetFullPath(new Uri(absolutePath).LocalPath);
        }

        /// <summary>Converts an absolute path to a relative path if possible.</summary>
        /// <param name="absolutePath">The absolute path.</param>
        /// <param name="relativeTo">The current directory.</param>
        /// <returns>The relative path.</returns>
        /// <exception cref="ArgumentException">The path of the two files doesn't have any common base.</exception>
        public static string MakeRelativePath(string absolutePath, string relativeTo)
        {
            string[] absParts = absolutePath.Split(System.IO.Path.DirectorySeparatorChar);
            string[] relParts = relativeTo.Split(System.IO.Path.DirectorySeparatorChar);

            // Get the shortest of the two paths
            int len = absParts.Length < relParts.Length ? absParts.Length : relParts.Length;

            // Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            // Find common root
            for (index = 0; index < len; index++)
            {
                if (absParts[index].Equals(relParts[index], StringComparison.OrdinalIgnoreCase))
                    lastCommonRoot = index;
                else
                    break;
            }

            // If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
                return absolutePath;

            // Build up the relative path
            var relativePath = new StringBuilder();

            // Add on the ..
            for (index = lastCommonRoot + 1; index < relParts.Length; index++)
            {
                relativePath.Append("..");
                relativePath.Append(System.IO.Path.DirectorySeparatorChar);
            }

            // Add on the folders
            for (index = lastCommonRoot + 1; index < absParts.Length - 1; index++)
            {
                relativePath.Append(absParts[index]);
                relativePath.Append(System.IO.Path.DirectorySeparatorChar);
            }
            relativePath.Append(absParts[absParts.Length - 1]);

            return relativePath.ToString();
        }
    }
}
