//-----------------------------------------------------------------------
// <copyright file="TypeScriptCodeGeneratorViewModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MyToolkit.Mvvm;
using MyToolkit.Storage;
using NSwag;
using NSwag.CodeGeneration.ClientGenerators.TypeScript;

namespace NSwagStudio.ViewModels.ClientGenerators
{
    public class TypeScriptCodeGeneratorViewModel : ViewModelBase
    {
        private string _className;
        private string _clientCode;
        private TypeScriptAsyncType _asyncType;

        public TypeScriptCodeGeneratorViewModel()
        {
            ClassName = ApplicationSettings.GetSetting("ClassName", "Client");
        }

        /// <summary>Gets or sets the TypeScript class name. </summary>
        public string ClassName
        {
            get { return _className; }
            set { Set(ref _className, value); }
        }

        /// <summary>Gets or sets the async type. </summary>
        public TypeScriptAsyncType AsyncType
        {
            get { return _asyncType; }
            set { Set(ref _asyncType, value); }
        }

        /// <summary>Gets the async types. </summary>
        public TypeScriptAsyncType[] AsyncTypes
        {
            get { return Enum.GetNames(typeof(TypeScriptAsyncType)).Select(t => (TypeScriptAsyncType)Enum.Parse(typeof(TypeScriptAsyncType), t)).ToArray(); }
        }

        /// <summary>Gets or sets the client code. </summary>
        public string ClientCode
        {
            get { return _clientCode; }
            set { Set(ref _clientCode, value); }
        }

        public Task GenerateClientAsync(string swaggerData)
        {
            return RunTaskAsync(async () =>
            {
                var code = string.Empty;
                await Task.Run(() =>
                {
                    var service = SwaggerService.FromJson(swaggerData);

                    var codeGenerator = new SwaggerToTypeScriptGenerator(service);
                    codeGenerator.Class = ClassName;
                    codeGenerator.AsyncType = AsyncType;

                    code = codeGenerator.GenerateFile();
                });

                ClientCode = code;
            });
        }

        public override void HandleException(Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}
