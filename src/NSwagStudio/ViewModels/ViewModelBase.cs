//-----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using MyToolkit.Dialogs;

namespace NSwagStudio.ViewModels
{
    /// <summary>The base view model.</summary>
    public class ViewModelBase : MyToolkit.Mvvm.ViewModelBase
    {
        /// <summary>Handles the exception.</summary>
        /// <param name="exception">The exception.</param>
        public override void HandleException(Exception exception)
        {
            ExceptionBox.Show("An error occured", exception);
        }
    }
}