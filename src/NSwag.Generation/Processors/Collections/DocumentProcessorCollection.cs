//-----------------------------------------------------------------------
// <copyright file="DocumentProcessorCollection.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace NSwag.Generation.Processors.Collections
{
    /// <summary>A collection of docment processors.</summary>
    public class DocumentProcessorCollection : Collection<IDocumentProcessor>
    {
        /// <summary>Gets an operation processor of the specified type.</summary>
        /// <typeparam name="T">The operation processor type.</typeparam>
        /// <returns>The operation processor.</returns>
        public T TryGet<T>()
        {
            return (T)this.FirstOrDefault(p => p is T);
        }
    }
}