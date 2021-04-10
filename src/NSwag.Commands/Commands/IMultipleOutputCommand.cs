//-----------------------------------------------------------------------
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
//-----------------------------------------------------------------------


using NConsole;
using NSwag.CodeGeneration;
using System.Collections.Generic;

namespace NSwag.Commands
{
    public interface IMultipleOutputCommand : IConsoleCommand
    {
        Dictionary<ClientGeneratorOutputType, string> OutputFilePaths { get; set; }
    }
}
