//-----------------------------------------------------------------------
// <copyright file="MainWindowModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Windows;
using MyToolkit.Mvvm;
using MyToolkit.Storage;
using NSwag;
using NSwag.CodeGeneration.ClientGenerators.CSharp;

namespace NSwagStudio.ViewModels.ClientGenerators
{
    public class CSharpClientGeneratorViewModel : ViewModelBase
    {
        private string _clientCode;

        private string _className;
        private string _namespace;

        public CSharpClientGeneratorViewModel()
        {
            ClassName = ApplicationSettings.GetSetting("CSharpClassName", "MyClass");
            Namespace = ApplicationSettings.GetSetting("CSharpNamespace", "MyNamespace");
        }

        /// <summary>Gets or sets the CSharp class name. </summary>
        public string ClassName
        {
            get { return _className; }
            set { Set(ref _className, value); }
        }

        /// <summary>Gets or sets the CSharp namespace. </summary>
        public string Namespace
        {
            get { return _namespace; }
            set { Set(ref _namespace, value); }
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

                    var codeGenerator = new SwaggerToCSharpGenerator(service);
                    codeGenerator.Class = ClassName;
                    codeGenerator.Namespace = Namespace;

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
