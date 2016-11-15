//-----------------------------------------------------------------------
// <copyright file="NSwagDocument.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSwag.CodeGeneration.Commands;
using NSwag.CodeGeneration.Utilities;
using NSwag.Commands;

namespace NSwag.CodeGeneration
{
    /// <summary>The NSwagDocument implementation.</summary>
    /// <seealso cref="NSwag.Commands.NSwagDocumentBase" />
    public class NSwagDocument : NSwagDocumentBase
    {
        /// <summary>Initializes a new instance of the <see cref="NSwagDocument"/> class.</summary>
        public NSwagDocument()
        {
            SwaggerGenerators.WebApiToSwaggerCommand = new WebApiToSwaggerCommand();
            SwaggerGenerators.AssemblyTypeToSwaggerCommand = new AssemblyTypeToSwaggerCommand();
        }

        /// <summary>Creates a new NSwagDocument.</summary>
        /// <returns>The document.</returns>
        public static NSwagDocument Create()
        {
            return Create<NSwagDocument>();
        }

        /// <summary>Loads an existing NSwagDocument.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The document.</returns>
        public static Task<NSwagDocument> LoadAsync(string filePath)
        {
            return LoadAsync<NSwagDocument>(filePath, new Dictionary<Type, Type>
            {
                { typeof(WebApiToSwaggerCommandBase), typeof(WebApiToSwaggerCommand) },
                { typeof(AssemblyTypeToSwaggerCommandBase), typeof(AssemblyTypeToSwaggerCommand) }
            });
        }

        /// <summary>Converts to absolute path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns></returns>
        protected override string ConvertToAbsolutePath(string pathToConvert)
        {
            if (!string.IsNullOrEmpty(pathToConvert) && !System.IO.Path.IsPathRooted(pathToConvert))
                return PathUtilities.MakeAbsolutePath(pathToConvert, GetDocumentDirectory());
            return pathToConvert;
        }

        /// <summary>Converts a path to an relative path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns>The relative path.</returns>
        protected override string ConvertToRelativePath(string pathToConvert)
        {
            if (!string.IsNullOrEmpty(pathToConvert) && !pathToConvert.Contains("C:\\Program Files\\"))
                return PathUtilities.MakeRelativePath(pathToConvert, GetDocumentDirectory())?.Replace("\\", "/");
            return pathToConvert?.Replace("\\", "/");
        }

        private string GetDocumentDirectory()
        {
            var absoluteDocumentPath = PathUtilities.MakeAbsolutePath(Path, System.IO.Directory.GetCurrentDirectory());
            return System.IO.Path.GetDirectoryName(absoluteDocumentPath);
        }
    }
}