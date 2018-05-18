//-----------------------------------------------------------------------
// <copyright file="TypeScriptFrameworkRxJsModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.TypeScript.Models
{
    /// <summary>RxJs specific information.</summary>
    public class TypeScriptFrameworkRxJsModel
    {
        private readonly TypeScriptFrameworkModel _model;

        internal TypeScriptFrameworkRxJsModel(TypeScriptFrameworkModel model)
        {
            _model = model;
        }

        /// <summary>Gets the RxJs observable mergeMap method name.</summary>
        public string ObservableMergeMapMethod => _model.UseRxJs5 ? "flatMap" : "observableMergeMap";

        /// <summary>Gets the RxJs observable catch method name.</summary>
        public string ObservableCatchMethod => _model.UseRxJs5 ? "catch" : "observableCatch";

        /// <summary>Gets the RxJs observable of method name.</summary>
        public string ObservableOfMethod => _model.UseRxJs5 ? "Observable.of" : "observableOf";

        /// <summary>Gets the RxJs observable from method name.</summary>
        public string ObservableFromMethod => _model.UseRxJs5 ? "Observable.fromPromise" : "observableFrom";

        /// <summary>Gets the RxJs observable throw method name.</summary>
        public string ObservableThrowMethod => _model.UseRxJs5 ? "Observable.throw" : "observableThrow";
    }
}