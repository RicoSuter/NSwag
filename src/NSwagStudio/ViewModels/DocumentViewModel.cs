using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.Input;
using NSwag.Commands;
using NSwagStudio.Helpers;

namespace NSwagStudio.ViewModels;

public class DocumentViewModel : ViewModelBase
{
    private DocumentModel? _document;

    /// <summary>Initializes a new instance of the <see cref="DocumentViewModel"/> class.</summary>
    public DocumentViewModel()
    {
        GenerateCommand = new AsyncRelayCommand<string?>(GenerateAsync);
    }

    /// <summary>Gets or sets the command to generate code from the selected Swagger generator.</summary>
    public AsyncRelayCommand<string?> GenerateCommand { get; set; }

    public string? SwaggerGenerator { get; set; }

    /// <summary>Gets or sets the settings. </summary>
    public DocumentModel? Document
    {
        get => _document;
        set => Set(ref _document, value);
    }

    /// <summary>Gets the available runtimes (only shows runtimes whose tools are present on disk).</summary>
    public Runtime[] Runtimes
    {
        get
        {
            var root = NSwagDocument.RootBinaryDirectory;
            var runtimes = new List<Runtime> { Runtime.Default, Runtime.Debug };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (File.Exists(Path.Combine(root, "Win", "nswag.exe")))
                    runtimes.Add(Runtime.WinX64);
                if (File.Exists(Path.Combine(root, "Win", "nswag.x86.exe")))
                    runtimes.Add(Runtime.WinX86);
            }

            if (File.Exists(Path.Combine(root, "Net80", "dotnet-nswag.dll")))
                runtimes.Add(Runtime.Net80);
            if (File.Exists(Path.Combine(root, "Net90", "dotnet-nswag.dll")))
                runtimes.Add(Runtime.Net90);
            if (File.Exists(Path.Combine(root, "Net100", "dotnet-nswag.dll")))
                runtimes.Add(Runtime.Net100);

            return runtimes.ToArray();
        }
    }

    private async Task GenerateAsync(string? type)
    {
        if (Document == null)
            return;

        IsLoading = true;
        await RunTaskAsync(async () =>
        {
            var redirectOutput = type != "files";

            var start = Stopwatch.GetTimestamp();
            var result = await Document.Document.ExecuteCommandLineAsync(redirectOutput);
            var duration = TimeSpan.FromSeconds((Stopwatch.GetTimestamp() - start) / (double)Stopwatch.Frequency);

            if (redirectOutput)
            {
                foreach (var codeGenerator in Document.CodeGenerators)
                    codeGenerator.View.UpdateOutput(result);
            }
            else
            {
                foreach (var codeGenerator in Document.CodeGenerators)
                    codeGenerator.View.UpdateOutput(result);

                await MessageBoxHelper.ShowInfo(
                    "File: " + Document.Document.Path + "\nDuration: " + duration,
                    "Generation complete!");
            }
        });
        IsLoading = false;
    }
}
