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

namespace NSwagStudio.ViewModels.ClientGenerators
{
    public class SwaggerGeneratorViewModel : ViewModelBase
    {
        private string _swaggerCode;

        public async Task GenerateClientAsync(string swaggerData)
        {
            SwaggerCode = await RunTaskAsync(() => SwaggerService.FromJson(swaggerData).ToJson());
        }

        public override void HandleException(Exception exception)
        {
            MessageBox.Show(exception.Message);
        }

        /// <summary>Gets or sets the Swaggert code. </summary>
        public string SwaggerCode
        {
            get { return _swaggerCode; }
            set { Set(ref _swaggerCode, value); }
        }
    }
}
