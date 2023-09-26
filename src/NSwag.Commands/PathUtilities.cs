//-----------------------------------------------------------------------
// <copyright file="NSwagDocument.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSwag.Commands
{
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
                    {
                        rootIndex = path.Substring(0, starIndex).LastIndexOf("/", StringComparison.Ordinal);
                    }

                    var rootPath = rootIndex >= 0 ? path.Substring(0, rootIndex + 1) : Directory.GetCurrentDirectory();
                    var files = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);

                    allFiles.AddRange(FindWildcardMatches(path, files.Select(f => f.Replace("\\", "/")), '/').Select(Path.GetFullPath));
                }
                else
                {
                    allFiles.Add(path);
                }
            }

            return allFiles.Distinct();
        }

        /// <summary>Finds the wildcard matches.</summary>
        /// <param name="selector">The selector.</param>
        /// <param name="items">The items.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>The matches.</returns>
        public static IEnumerable<string> FindWildcardMatches(string selector, IEnumerable<string> items, char delimiter)
        {
            var escapedDelimiter = Regex.Escape(delimiter.ToString());

            var regex = new Regex(
                "^" + Regex.Escape(selector
                    .Replace("\\", "/")
                    .Replace(delimiter.ToString(), "__del__")
                    .Replace("**", "__starstar__")
                    .Replace("*", "__star__"))
                .Replace("__del__", "(" + escapedDelimiter + ")")
                .Replace("__starstar__", "(.*?)")
                .Replace("__star__", "([^" + escapedDelimiter + "]*?)") + "$");

            return items.Where(i => regex.Match(i.Replace("\\", "/")).Success);
        }

        /// <summary>Converts a relative path to an absolute path.</summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="relativeTo">The current directory.</param>
        /// <returns>The absolute path.</returns>
        public static string MakeAbsolutePath(string relativePath, string relativeTo)
        {
            // TODO: Rename to ToAbsolutePath, switch parameters
            if (Path.IsPathRooted(relativePath))
            {
                return relativePath;
            }

            var absolutePath = Path.Combine(relativeTo, relativePath);
            return Path.GetFullPath(absolutePath);
        }

        /// <summary>Converts an absolute path to a relative path if possible.</summary>
        /// <param name="absolutePath">The absolute path.</param>
        /// <param name="relativeTo">The current directory.</param>
        /// <returns>The relative path.</returns>
        /// <exception cref="ArgumentException">The path of the two files doesn't have any common base.</exception>
        public static string MakeRelativePath(string absolutePath, string relativeTo)
        {
            string[] absParts = absolutePath.Split(Path.DirectorySeparatorChar, '/');
            string[] relParts = relativeTo.Split(Path.DirectorySeparatorChar, '/');

            if (absParts.SequenceEqual(relParts))
            {
                return ".";
            }

            // Get the shortest of the two paths
            int len = absParts.Length < relParts.Length ? absParts.Length : relParts.Length;

            // Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            // Find common root
            for (index = 0; index < len; index++)
            {
                if (absParts[index].Equals(relParts[index], StringComparison.OrdinalIgnoreCase))
                {
                    lastCommonRoot = index;
                }
                else
                {
                    break;
                }
            }

            // If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
            {
                return absolutePath;
            }

            // Build up the relative path
            var relativePath = new StringBuilder();

            // Add on the ..
            for (index = lastCommonRoot + 1; index < relParts.Length; index++)
            {
                relativePath.Append("..");
                relativePath.Append(Path.DirectorySeparatorChar);
            }

            // Add on the folders
            for (index = lastCommonRoot + 1; index < absParts.Length - 1; index++)
            {
                relativePath.Append(absParts[index]);
                relativePath.Append(Path.DirectorySeparatorChar);
            }
            relativePath.Append(absParts[absParts.Length - 1]);

            return relativePath.ToString();
        }
    }
}
