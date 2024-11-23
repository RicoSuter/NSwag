﻿//-----------------------------------------------------------------------
// <copyright file="MainWindowModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag;

namespace NSwagStudio.ViewModels.CodeGenerators
{
    public class SwaggerOutputViewModel : ViewModelBase
    {
        private string _swaggerCode;

        public async Task GenerateClientAsync(OpenApiDocument document, string documentPath)
        {
            if (document != null)
                SwaggerCode = await RunTaskAsync(Task.Run(() => document.ToJson()));
            else
                SwaggerCode = string.Empty;
        }

        /// <summary>Gets or sets the Swagger code. </summary>
        public string SwaggerCode
        {
            get => _swaggerCode;
            set => Set(ref _swaggerCode, value);
        }
    }
}
