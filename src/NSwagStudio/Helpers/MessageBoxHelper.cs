using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace NSwagStudio.Helpers;

public static class MessageBoxHelper
{
    public static async Task ShowInfo(string message, string title = "Information")
    {
        var box = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok, Icon.Info);
        if (DialogService.MainWindow != null)
            await box.ShowWindowDialogAsync(DialogService.MainWindow);
        else
            await box.ShowAsync();
    }

    public static async Task ShowError(string message, string title = "Error", Exception? exception = null)
    {
        var fullMessage = exception != null ? $"{message}\n\n{exception.Message}\n\n{exception.StackTrace}" : message;
        var box = MessageBoxManager.GetMessageBoxStandard(title, fullMessage, ButtonEnum.Ok, Icon.Error);
        if (DialogService.MainWindow != null)
            await box.ShowWindowDialogAsync(DialogService.MainWindow);
        else
            await box.ShowAsync();
    }

    public static async Task<ButtonResult> ShowYesNoCancel(string message, string title = "Question")
    {
        var box = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.YesNoCancel, Icon.Question);
        if (DialogService.MainWindow != null)
            return await box.ShowWindowDialogAsync(DialogService.MainWindow);
        else
            return await box.ShowAsync();
    }
}
