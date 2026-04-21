using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using NSwagStudio.Helpers;

namespace NSwagStudio.ViewModels;

/// <summary>The base view model.</summary>
public class ViewModelBase : ObservableObject
{
    private bool _isLoading;

    /// <summary>Gets or sets whether the view model is loading.</summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>Sets a property value and raises PropertyChanged.</summary>
    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        return SetProperty(ref field, value, propertyName);
    }

    /// <summary>Raises PropertyChanged for all properties.</summary>
    public void RaiseAllPropertiesChanged()
    {
        OnPropertyChanged(string.Empty);
    }

    /// <summary>Handles exceptions by showing an error dialog.</summary>
    public virtual void HandleException(Exception exception)
    {
        _ = MessageBoxHelper.ShowError("An error occurred", "Error", exception);
    }

    /// <summary>Runs an async task with error handling.</summary>
    protected async Task RunTaskAsync(Func<Task> task)
    {
        try
        {
            await task();
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
    }

    /// <summary>Runs an async task with error handling and return value.</summary>
    protected async Task<T?> RunTaskAsync<T>(Func<Task<T>> task)
    {
        try
        {
            return await task();
        }
        catch (Exception exception)
        {
            HandleException(exception);
            return default;
        }
    }

    /// <summary>Runs an async task with error handling.</summary>
    protected async Task<T?> RunTaskAsync<T>(Task<T> task)
    {
        try
        {
            return await task;
        }
        catch (Exception exception)
        {
            HandleException(exception);
            return default;
        }
    }

    /// <summary>Called when the view is loaded.</summary>
    public virtual void OnLoaded() { }

    /// <summary>Called when the view is unloaded.</summary>
    public virtual void OnUnloaded() { }

    /// <summary>Triggers the OnUnloaded lifecycle method.</summary>
    public void CallOnUnloaded() => OnUnloaded();
}
