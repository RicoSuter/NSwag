using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSwag.Utilities
{
    public static class PathUtilities
    {
        // TODO: Move to MyToolkit

        public static IEnumerable<string> ExpandWildcards(string path)
        {
            return ExpandWildcards(new[] { path });
        }

        public static IEnumerable<string> ExpandWildcards(IEnumerable<string> paths)
        {
            var allFiles = new List<string>();
            foreach (var path in paths)
            {
                if (path.Contains("*"))
                {
                    var starIndex = path.IndexOf("*", StringComparison.InvariantCulture);

                    var rootIndex = path.IndexOf("\\", 0, starIndex, StringComparison.InvariantCulture);
                    if (rootIndex == -1)
                        rootIndex = path.IndexOf("/", 0, starIndex, StringComparison.InvariantCulture);

                    var rootPath = rootIndex >= 0 ? path.Substring(0, rootIndex + 1) : Directory.GetCurrentDirectory();
                    var files = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);

                    var regex = new Regex(
                        Regex.Escape(path.Substring(rootIndex + 1)
                        .Replace("/", "__del__")
                        .Replace("\\", "__del__")
                        .Replace("**", "__starstar__")
                        .Replace("*", "__star__"))
                        .Replace("__del__", "[\\\\/]")
                        .Replace("__starstar__", "(.*?)")
                        .Replace("__star__", "([^\\/]*?)"));

                    allFiles.AddRange(files
                        .Where(f => regex.Match(f).Success)
                        .Select(Path.GetFullPath));
                }
                else
                    allFiles.Add(path);
            }

            return allFiles.Distinct();
        }

        public static string MakeAbsolutePath(string relativePath, string relTo)
        {
            var absolutePath = System.IO.Path.Combine(relativePath, relTo);
            return System.IO.Path.GetFullPath(new Uri(absolutePath).LocalPath);
        }

        /// <exception cref="ArgumentException">The path of the two files doesn't have any common base.</exception>
        public static string MakeRelativePath(string absPath, string relTo)
        {
            string[] absParts = absPath.Split(System.IO.Path.DirectorySeparatorChar);
            string[] relParts = relTo.Split(System.IO.Path.DirectorySeparatorChar);

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
                return absPath;

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
