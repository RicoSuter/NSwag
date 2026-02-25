using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using NJsonSchema;
using NSwag.Commands;

namespace NSwagStudio.Views.CodeGenerators;

public abstract class CodeGeneratorViewBase : UserControl
{
    public abstract string Title { get; }

    public string PropertyName => ConversionUtilities.ConvertToLowerCamelCase(GetType().Name, false);

    public abstract void UpdateOutput(OpenApiDocumentExecutionResult result);

    public abstract bool IsSelected { get; set; }

    public virtual bool IsPersistent => false;

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
