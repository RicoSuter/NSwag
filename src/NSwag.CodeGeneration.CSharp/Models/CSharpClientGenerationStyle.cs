using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSwag.CodeGeneration.CSharp.Models
{
    /// <summary>The CSharp generation style enum.</summary>
    public enum CSharpClientGenerationStyle
    {
        /// <summary>Output a single file.</summary>
        SingleFile,

        /// <summary>Output one file per type.</summary>
        OneFilePerType
    }
}
