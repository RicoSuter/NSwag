//-----------------------------------------------------------------------
// <copyright file="CustomAssemblyLoadContext.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Runtime.Loader;

namespace NSwag.CodeGeneration.Infrastructure
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null; // Use Resolving event in AssemblyLoader
        }
    }
}
