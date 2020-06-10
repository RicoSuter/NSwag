//-----------------------------------------------------------------------
// <copyright file="TypeScriptFrameworkModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema.CodeGeneration.TypeScript;

namespace NSwag.CodeGeneration.TypeScript.Models
{
    /// <summary>Framework specific information.</summary>
    public class TypeScriptFrameworkModel
    {
        private readonly TypeScriptClientGeneratorSettings _settings;

        internal TypeScriptFrameworkModel(TypeScriptClientGeneratorSettings settings)
        {
            _settings = settings;

            RxJs = new TypeScriptFrameworkRxJsModel(this);
            Angular = new TypeScriptFrameworkAngularModel(settings);
        }

        /// <summary>Gets a value indicating whether the generated code is for Angular 2.</summary>
        public bool IsAngular => _settings.Template == TypeScriptTemplate.Angular;

        /// <summary>Gets a value indicating whether the generated code is for Aurelia.</summary>
        public bool IsAurelia => _settings.Template == TypeScriptTemplate.Aurelia;

        /// <summary>Gets a value indicating whether the generated code is for Angular.</summary>
        public bool IsAngularJS => _settings.Template == TypeScriptTemplate.AngularJS;

        /// <summary>Gets a value indicating whether the generated code is for Knockout.</summary>
        public bool IsKnockout => _settings.TypeScriptGeneratorSettings.TypeStyle == TypeScriptTypeStyle.KnockoutClass;

        /// <summary>Gets a value indicating whether to render for JQuery.</summary>
        public bool IsJQuery => _settings.Template == TypeScriptTemplate.JQueryCallbacks || _settings.Template == TypeScriptTemplate.JQueryPromises;

        /// <summary>Gets a value indicating whether to render for Fetch or Aurelia</summary>
        public bool IsFetchOrAurelia => _settings.Template == TypeScriptTemplate.Fetch ||
                                        _settings.Template == TypeScriptTemplate.Aurelia;

        /// <summary>Gets a value indicating whether to render for Axios.</summary>
        public bool IsAxios => _settings.Template == TypeScriptTemplate.Axios;

        /// <summary>Gets a value indicating whether MomentJS is required.</summary>
        public bool UseMomentJS => _settings.TypeScriptGeneratorSettings.DateTimeType == TypeScriptDateTimeType.MomentJS ||
                                   _settings.TypeScriptGeneratorSettings.DateTimeType == TypeScriptDateTimeType.OffsetMomentJS;

        /// <summary>Gets a value indicating whether to use RxJs 5.</summary>
        public bool UseRxJs5 => _settings.RxJsVersion < 6.0m;

        /// <summary>Gets a value indicating whether to use RxJs 6.</summary>
        public bool UseRxJs6 => _settings.RxJsVersion >= 6.0m;

        /// <summary>Gets Rxjs information.</summary>
        public TypeScriptFrameworkRxJsModel RxJs { get; }

        /// <summary>Gets Angular information.</summary>
        public TypeScriptFrameworkAngularModel Angular { get; set; }
    }
}
