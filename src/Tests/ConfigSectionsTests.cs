using NUnit.Framework;
using TypedConfigProvider;

namespace Tests
{
    public class ConfigSectionsTests
    {
        [Test]
        [TestCase("target", "target")]
        [TestCase("TARGET", "target")]
        [TestCase("TArgET", "target")]
        public void Test_SectionConfigTarget_IsAlwaysLowerCase(string target, string expected)
        {
            var configSection = new ConfigSections {Target = target};

            Assert.AreEqual(expected, configSection.Target);
        }
    }
}