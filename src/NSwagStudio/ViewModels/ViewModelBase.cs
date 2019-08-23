//-----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows;
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
            ExceptionBox.Show("An error occured", exception, Application.Current.MainWindow);
        }

        protected string FromStringArray(string[] array)
        {
            return array != null ? string.Join(",", array) : "";
        }

        protected string[] ToStringArray(string value)
        {
            if (value != null)
                return value.Split(',').Select(n => n.Trim()).Where(n => !string.IsNullOrEmpty(n)).ToArray();
            else
                return new string[] { };
        }
    }
}