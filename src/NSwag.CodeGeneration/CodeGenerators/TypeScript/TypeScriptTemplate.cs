//-----------------------------------------------------------------------
// <copyright file="TypeScriptAsyncType.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript
{
    /// <summary>The TypeScript output templates.</summary>
    public enum TypeScriptTemplate
    {
        /// <summary>Uses JQuery with callbacks.</summary>
        JQueryCallbacks,

        /// <summary>Uses JQuery and Q promises.</summary>
        JQueryQPromises,

        /// <summary>Uses $http from AngularJS 1.x.</summary>
        AngularJS,
    }
}