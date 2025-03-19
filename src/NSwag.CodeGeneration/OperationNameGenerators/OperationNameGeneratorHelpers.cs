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
        /// <summary>
        /// Modifies an identifier, so it will be both be a valid CSharp identfier as a valid TypeScript identifier
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
                var validCategoriesFirstChar = new[] { UnicodeCategory.UppercaseLetter, UnicodeCategory.LowercaseLetter, UnicodeCategory.TitlecaseLetter, UnicodeCategory.ModifierLetter, UnicodeCategory.OtherLetter, UnicodeCategory.LetterNumber };
                var additionalValidCategoriesOtherChars = new[] { UnicodeCategory.DecimalDigitNumber, UnicodeCategory.ConnectorPunctuation, UnicodeCategory.NonSpacingMark, UnicodeCategory.SpacingCombiningMark };

                var newIdent = new StringBuilder();
                var firstChar = identifier[0];
                var firstCat = char.GetUnicodeCategory(firstChar);
                if (firstChar != '_' && !validCategoriesFirstChar.Contains(firstCat)
                    && additionalValidCategoriesOtherChars.Contains(firstCat))
                {
                    // Add underscore if first char is invalid as a first char, but valid as an additional char
                    newIdent.Append('_');
                }

                foreach (var character in identifier)
                {
                    // Replace all characters that are invalid with underscores
                    var cat = char.GetUnicodeCategory(character);
                    if (validCategoriesFirstChar.Contains(cat) || additionalValidCategoriesOtherChars.Contains(cat))
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
