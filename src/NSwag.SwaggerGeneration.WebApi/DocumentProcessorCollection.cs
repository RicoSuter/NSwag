//-----------------------------------------------------------------------
// <copyright file="DocumentProcessorCollection.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using NSwag.SwaggerGeneration.Processors;

namespace NSwag.SwaggerGeneration.WebApi
{
    /// <summary>A collection of docment processors.</summary>
    public class DocumentProcessorCollection : Collection<IDocumentProcessor>
    {

    }
}