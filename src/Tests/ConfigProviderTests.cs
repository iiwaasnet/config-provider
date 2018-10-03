using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Tests.config;
using TypedConfigProvider;

namespace Tests
{
    public class ConfigProviderTests
    {
        [Test]
        public void Test_FlatConfig_ConvertedToClass()
        {
            var targetProvider = new Mock<IConfigTargetProvider>();
            var target = "dev";

            var stringProp = "str1";
            var booleanProp = true;
            var integerProp = 123;
            var dateTimeProp = new DateTime(2015, 3, 21, 9, 9, 11);
            var timeSpanProp = new TimeSpan(1, 2, 23, 1, 100);
            var decimalProp = 12.356m;

            targetProvider.Setup(m => m.GetTargetsSequence()).Returns(new[] {target});
            var configProvider = new ConfigProvider(targetProvider.Object);
            var config = configProvider.GetConfiguration<SimpleFlatConfiguration>();

            Assert.AreEqual(stringProp, config.StringProp);
            Assert.AreEqual(booleanProp, config.BooleanProp);
            Assert.AreEqual(integerProp, config.IntegerProp);
            Assert.AreEqual(dateTimeProp, config.DateTimeProp);
            Assert.AreEqual(timeSpanProp, config.TimeSpanProp);
            Assert.AreEqual(decimalProp, config.DecimalProp);
        }

        [Test]
        public void Test_NestedConfig_ConvertedToClass()
        {
            var targetProvider = new Mock<IConfigTargetProvider>();
            var target = "dev";

            var item1 = "item1";
            var item2 = "item2";

            targetProvider.Setup(m => m.GetTargetsSequence()).Returns(new[] {target});
            var configProvider = new ConfigProvider(targetProvider.Object);
            var config = configProvider.GetConfiguration<NestedObjectsConfiguration>();

            Assert.AreEqual(2, config.Entity.Items.Count());
            Assert.AreEqual(1, config.Entity.Items.Count(i => i.Name == item1));
            Assert.AreEqual(1, config.Entity.Items.Count(i => i.Name == item2));
        }

        [Test]
        public void Test_UseNullResetToRemoveAllArrayItems()
        {
            var targetProvider = new Mock<IConfigTargetProvider>();

            targetProvider.Setup(m => m.GetTargetsSequence()).Returns(new[] {"dev", "reset", "prod"});
            var configProvider = new ConfigProvider(targetProvider.Object);
            var config = configProvider.GetConfiguration<NestedObjectsConfiguration>();

            Assert.AreEqual(1, config.Entity.Items.Count());
            Assert.AreEqual("item3", config.Entity.Items.First().Name);
            Assert.AreEqual("valueDev", config.Prop);
            Assert.AreEqual("entityPropProd", config.Entity.Prop);
            Assert.AreEqual("valueDev", config.Prop);
        }

        [Test]
        public void Test_ConfigurationSectionsAreAppliedInOrderOfTargetsProvided()
        {
            var targetProvider = new Mock<IConfigTargetProvider>();

            var integerProp = 123;
            var dateTimeProp = new DateTime(2015, 3, 21, 9, 9, 11);
            var timeSpanProp = new TimeSpan(1, 2, 23, 1, 100);

            var devStringProp = "str1";
            var devBooleanProp = true;
            var devDecimalProp = 12.356m;

            targetProvider.Setup(m => m.GetTargetsSequence()).Returns(new[] {"prod", "dev"});
            var configProvider = new ConfigProvider(targetProvider.Object);
            var config = configProvider.GetConfiguration<SimpleFlatConfiguration>();

            Assert.AreEqual(devStringProp, config.StringProp);
            Assert.AreEqual(devBooleanProp, config.BooleanProp);
            Assert.AreEqual(integerProp, config.IntegerProp);
            Assert.AreEqual(dateTimeProp, config.DateTimeProp);
            Assert.AreEqual(timeSpanProp, config.TimeSpanProp);
            Assert.AreEqual(devDecimalProp, config.DecimalProp);

            targetProvider.Setup(m => m.GetTargetsSequence()).Returns(new[] {"dev", "prod"});
            configProvider = new ConfigProvider(targetProvider.Object);
            config = configProvider.GetConfiguration<SimpleFlatConfiguration>();

            var prodDecimalProp = 13.356m;
            var prodBooleanProp = false;
            var prodStringProp = "strPROD";

            Assert.AreEqual(prodStringProp, config.StringProp);
            Assert.AreEqual(prodBooleanProp, config.BooleanProp);
            Assert.AreEqual(integerProp, config.IntegerProp);
            Assert.AreEqual(dateTimeProp, config.DateTimeProp);
            Assert.AreEqual(timeSpanProp, config.TimeSpanProp);
            Assert.AreEqual(prodDecimalProp, config.DecimalProp);
        }

        [Test]
        public void Test_IfRequestedTargetIsNotConfigured_GetConfigurationThrowsException()
        {
            var targetProvider = new Mock<IConfigTargetProvider>();
            var target = "missing";

            targetProvider.Setup(m => m.GetTargetsSequence()).Returns(new[] {target});
            var configProvider = new ConfigProvider(targetProvider.Object);
            Assert.Throws<Exception>(() => configProvider.GetConfiguration<NestedObjectsConfiguration>());
        }
    }
}