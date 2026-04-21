using NSwag.Commands;

namespace NSwagStudio;

/// <summary>The interface for a Swagger generator.</summary>
public interface ISwaggerGeneratorView
{
    /// <summary>Gets the title.</summary>
    string Title { get; }

    /// <summary>Gets the command.</summary>
    IOutputCommand Command { get; }

    /// <summary>Generates the Swagger specification.</summary>
    Task<string> GenerateSwaggerAsync();
}
