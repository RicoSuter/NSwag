//-----------------------------------------------------------------------
// <copyright file="OperationNameGeneratorHelpers.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Text;

namespace NSwag.CodeGeneration.OperationNameGenerators
{
    static class OperationNameGeneratorHelpers
    {
        private static readonly UnicodeCategory[] s_validCategoriesFirstChar = [UnicodeCategory.UppercaseLetter, UnicodeCategory.LowercaseLetter, UnicodeCategory.TitlecaseLetter, UnicodeCategory.ModifierLetter, UnicodeCategory.OtherLetter, UnicodeCategory.LetterNumber];
        private static readonly UnicodeCategory[] s_additionalValidCategoriesOtherChars = [UnicodeCategory.DecimalDigitNumber, UnicodeCategory.ConnectorPunctuation, UnicodeCategory.NonSpacingMark, UnicodeCategory.SpacingCombiningMark];

        /// <summary>
        /// Modifies an identifier, so it will be both be a valid CSharp identifier as a valid TypeScript identifier
        /// </summary>
        /// <remarks>
        /// Because this function is used for both C# as TypeScript, some valid identifiers will be changed because they are invalid in the other. E.g. $test is valid TypeScript, but not C#.
        /// </remarks>
        /// <param name="identifier">The identifier.</param>
        /// <returns>The fixed identifier</returns>
        public static string FixIdentifier(string identifier)
        {
            if (identifier.Length > 0)
            {
                var newIdent = new StringBuilder();
                var firstChar = identifier[0];
                var firstCat = char.GetUnicodeCategory(firstChar);
                if (firstChar != '_' && Array.IndexOf(s_validCategoriesFirstChar, firstCat) == -1
                    && Array.IndexOf(s_additionalValidCategoriesOtherChars, firstCat) != -1)
                {
                    // Add underscore if first char is invalid as a first char, but valid as an additional char
                    newIdent.Append('_');
                }

                foreach (var character in identifier)
                {
                    // Replace all characters that are invalid with underscores
                    var cat = char.GetUnicodeCategory(character);
                    if (Array.IndexOf(s_validCategoriesFirstChar, cat) != -1 || Array.IndexOf(s_additionalValidCategoriesOtherChars, cat) != -1)
                    {
                        newIdent.Append(character);
                    }
                    else
                    {
                        newIdent.Append('_');
                    }
                }
                return newIdent.ToString();
            }
            else
            {
                return "_";
            }
        }
    }
}
