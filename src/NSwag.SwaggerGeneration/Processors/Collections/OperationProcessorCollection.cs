//-----------------------------------------------------------------------
// <copyright file="OperationProcessorCollection.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Linq;

namespace NSwag.SwaggerGeneration.Processors.Collections
{
    /// <summary>A collection of operation processors.</summary>
    public class OperationProcessorCollection : Collection<IOperationProcessor>
    {
        /// <summary>Gets an operation processor of the specified type.</summary>
        /// <typeparam name="T">The operation processor type.</typeparam>
        /// <returns>The operation processor.</returns>
        public T TryGet<T>()
        {
            return (T)this.FirstOrDefault(p => p is T);
        }

        /// <summary>Replaces the first element of type <typeparamref name="T"/>
        /// with <paramref name="newItem"/>.</summary>
        /// <typeparam name="T">The operation processor type to replace.</typeparam>
        /// <param name="newItem">The replacement item.</param>
        /// <returns>true, if an item was replaced; otherwise false.</returns>
        public bool Replace<T>(IOperationProcessor newItem) where T : IOperationProcessor
        {
            var item = this.OfType<T>().FirstOrDefault();

            if (item != null)
            {
                SetItem(IndexOf(item), newItem);
                return true;
            }

            return false;
        }
    }
}