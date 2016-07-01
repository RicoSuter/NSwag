using System;
using System.Text;

namespace NSwag.Utilities
{
    public static class PathUtilities
    {
        // TODO: Move to MyToolkit

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
