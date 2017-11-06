//-----------------------------------------------------------------------
// <copyright file="OutputCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NConsole;

namespace NSwag.Commands
{
    public interface IOutputCommand : IConsoleCommand
    {
        string OutputFilePath { get; set; }
    }
}