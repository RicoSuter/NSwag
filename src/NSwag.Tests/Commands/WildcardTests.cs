using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.SwaggerGeneration.Utilities;

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
        public void NoWildcard()
        {
            //// Arrange
            var items = new string[] { "abc/def", "ghi/jkl" };

            //// Act
            var matches = PathUtilities.FindWildcardMatches("abc/def", items, '/');

            //// Assert
            Assert.AreEqual(1, matches.Count());
            Assert.AreEqual("abc/def", matches.First());
        }

        [TestMethod]
        public void SingleWildcardInTheMiddle()
        {
            //// Arrange
            var items = new string[] {"abc/def/ghi", "abc/def/jkl", "abc/a/b/ghi"};

            //// Act
            var matches = PathUtilities.FindWildcardMatches("abc/*/ghi", items, '/');

            //// Assert
            Assert.AreEqual(1, matches.Count());
            Assert.AreEqual("abc/def/ghi", matches.First());
        }

        [TestMethod]
        public void DoubleWildcardInTheMiddle()
        {
            //// Arrange
            var items = new string[] { "a/b/c", "a/b/d", "a/b/b/c" };

            //// Act
            var matches = PathUtilities.FindWildcardMatches("a/**/c", items, '/');

            //// Assert
            Assert.AreEqual(2, matches.Count());
            Assert.AreEqual("a/b/c", matches.First());
            Assert.AreEqual("a/b/b/c", matches.Last());
        }
        
        [TestMethod]
        public void DoubleWildcardAtTheEnd()
        {
            //// Arrange
            var items = new string[] { "abc/a", "abc/b", "abc/c/d" };

            //// Act
            var matches = PathUtilities.FindWildcardMatches("abc/**", items, '/');

            //// Assert
            Assert.AreEqual(3, matches.Count());
        }

        [TestMethod]
        public void SingleWildcardAtTheEnd()
        {
            //// Arrange
            var items = new string[] { "abc/a", "abc/b", "abc/c/d" };

            //// Act
            var matches = PathUtilities.FindWildcardMatches("abc/*", items, '/');

            //// Assert
            Assert.AreEqual(2, matches.Count());
        }

        [TestMethod]
        public void DoubleWildcardAtTheBeginning()
        {
            //// Arrange
            var items = new string[] { "a/b/c", "a/b/d", "a/c" };

            //// Act
            var matches = PathUtilities.FindWildcardMatches("**/c", items, '/');

            //// Assert
            Assert.AreEqual(2, matches.Count());
            Assert.AreEqual("a/b/c", matches.First());
            Assert.AreEqual("a/c", matches.Last());
        }

        [TestMethod]
        public void SingleWildcardAtTheBeginning()
        {
            //// Arrange
            var items = new string[] { "a/b/c", "x/y/c", "x/b/c" };

            //// Act
            var matches = PathUtilities.FindWildcardMatches("*/b/c", items, '/');

            //// Assert
            Assert.AreEqual(2, matches.Count());
            Assert.AreEqual("a/b/c", matches.First());
            Assert.AreEqual("x/b/c", matches.Last());
        }
    }
}
