using NSwag.Commands;

namespace NSwag.ConsoleCore.Tests
{
    public class RuntimeUtilitiesTests
    {
        [Theory]
        [InlineData(Runtime.Net80, "Net80")]
        [InlineData(Runtime.Net90, "Net90")]
        [InlineData(Runtime.Net100, "Net100")]
        [InlineData(Runtime.Net110, "Net110")]
        public void GetDirectoryName_returns_folder_for_core_runtimes(Runtime runtime, string expected)
        {
            Assert.Equal(expected, RuntimeUtilities.GetDirectoryName(runtime));
        }

        [Theory]
        [InlineData(Runtime.Default)]
        [InlineData(Runtime.WinX64)]
        [InlineData(Runtime.WinX86)]
        [InlineData(Runtime.Debug)]
        public void GetDirectoryName_returns_null_for_non_core_runtimes(Runtime runtime)
        {
            Assert.Null(RuntimeUtilities.GetDirectoryName(runtime));
        }
    }
}
