using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.Utilities;

namespace NSwag.Tests.Commands
{
    [TestClass]
    public class WildcardTests
    {
        [TestMethod]
        public void When_path_has_wildcards_then_they_are_expanded_correctly()
        {
            //// Arrange
            

            //// Act
            var files = PathUtilities.ExpandFileWildcards("../../**/NSwag.*.dll").ToList();

            //// Assert
            Assert.IsTrue(files.Any(f => f.Contains("bin\\Debug")) || files.Any(f => f.Contains("bin\\Release")));
        }
    }
}
