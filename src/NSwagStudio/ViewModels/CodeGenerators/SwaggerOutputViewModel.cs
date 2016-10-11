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
using NSwag;

namespace NSwagStudio.ViewModels.CodeGenerators
{
    public class SwaggerOutputViewModel : ViewModelBase
    {
        private string _swaggerCode;

        public async Task GenerateClientAsync(string swaggerData, string documentPath)
        {
            var code = !string.IsNullOrEmpty(swaggerData) ? await RunTaskAsync(() => SwaggerService.FromJson(swaggerData, documentPath)?.ToJson()) : string.Empty;
            SwaggerCode = code ?? string.Empty;
        }

        public override void HandleException(Exception exception)
        {
            MessageBox.Show(exception.Message);
        }

        /// <summary>Gets or sets the Swagger code. </summary>
        public string SwaggerCode
        {
            get { return _swaggerCode; }
            set { Set(ref _swaggerCode, value); }
        }
    }
}
