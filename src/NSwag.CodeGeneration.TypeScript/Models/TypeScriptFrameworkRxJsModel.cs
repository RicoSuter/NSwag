//-----------------------------------------------------------------------
// <copyright file="TypeScriptFrameworkRxJsModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
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
        public string ObservableMergeMapMethod => _model.UseRxJs5 ? "flatMap" : "_observableMergeMap";

        /// <summary>Gets the RxJs observable catch method name.</summary>
        public string ObservableCatchMethod => _model.UseRxJs5 ? "catch" : "_observableCatch";

        /// <summary>Gets the RxJs observable of method name.</summary>
        public string ObservableOfMethod => _model.UseRxJs5 ? "Observable.of" : "_observableOf";

        /// <summary>Gets the RxJs observable from method name.</summary>
        public string ObservableFromMethod => _model.UseRxJs5 ? "Observable.fromPromise" : "_observableFrom";

        /// <summary>Gets the RxJs observable throw method name.</summary>
        public string ObservableThrowMethod => _model.UseRxJs5 ? "Observable.throw" : "_observableThrow";

        /// <summary>Gets the response text property in condition to rxjs version</summary>
        public string ResponseTextProperty => _model.UseRxJs7 ? "(_responseText: string)" : "_responseText";
    }
}