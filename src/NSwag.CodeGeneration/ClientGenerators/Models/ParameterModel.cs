//-----------------------------------------------------------------------
// <copyright file="ParameterModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.ClientGenerators.Models
{
    internal class ParameterModel
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public bool IsLast { get; set; }

        public bool IsDate { get; set; }
    }
}