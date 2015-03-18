using NUnit.Framework;
using TypedConfigProvider;

namespace Tests
{
    [TestFixture]
    public class ConfigSectionsTests
    {
        [TestCase("target", "target")]
        [TestCase("TARGET", "target")]
        [TestCase("TArgET", "target")]
        [Test]
        public void Test_SectionConfigTarget_IsAlwaysLowerCase(string target, string expected)
        {
            var configSection = new ConfigSections {Target = target};

            Assert.AreEqual(expected, configSection.Target);
        }
    }
}