using System;
using System.Windows;

namespace NSwagStudio.ViewModels
{
    public class ViewModelBase : MyToolkit.Mvvm.ViewModelBase
    {
        public override void HandleException(Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}