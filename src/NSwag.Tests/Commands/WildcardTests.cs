using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.Utilities;

namespace NSwag.Tests.Commands
{
    [TestClass]
    public class WildcardTests
    {
        [TestMethod]
        public void METHOD()
        {
            //// Arrange
            

            //// Act
            var x = Directory.GetCurrentDirectory();
            var files = PathUtilities.ExpandWildcards("../../**/*.dll");

            //// Assert

        }
    }
}
