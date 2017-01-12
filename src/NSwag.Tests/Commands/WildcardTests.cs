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

        [TestMethod]
        public void When_selector_does_not_contain_wildcard_then_item_is_matched()
        {
            //// Arrange
            var items = new string[] { "abc/def", "ghi/jkl" };

            //// Act
            var matches = PathUtilities.FindWildcardMatches("abc/def", items);

            //// Assert
            Assert.AreEqual(1, matches.Count());
            Assert.AreEqual("abc/def", matches.First());
        }

        [TestMethod]
        public void When_selector_contains_wildcard_then_item_is_matched()
        {
            //// Arrange
            var items = new string[] { "abc/def/ghi", "abc/def/jkl" };

            //// Act
            var matches = PathUtilities.FindWildcardMatches("abc/*/ghi", items);

            //// Assert
            Assert.AreEqual(1, matches.Count());
            Assert.AreEqual("abc/def/ghi", matches.First());
        }
    }
}
