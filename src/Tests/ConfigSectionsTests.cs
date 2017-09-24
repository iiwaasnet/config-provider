using TypedConfigProvider;
using Xunit;

namespace Tests
{
    public class ConfigSectionsTests
    {
        [Theory]
        [InlineData("target", "target")]
        [InlineData("TARGET", "target")]
        [InlineData("TArgET", "target")]
        public void Test_SectionConfigTarget_IsAlwaysLowerCase(string target, string expected)
        {
            var configSection = new ConfigSections {Target = target};

            Assert.Equal(expected, configSection.Target);
        }
    }
}