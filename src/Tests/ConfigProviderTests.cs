using System;
using System.Linq;
using Moq;
using Tests.config;
using TypedConfigProvider;
using Xunit;

namespace Tests
{
    public class ConfigProviderTests
    {
        [Fact]
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
            var configProvider = new ConfigProvider(targetProvider.Object, "config");
            var config = configProvider.GetConfiguration<SimpleFlatConfiguration>();

            Assert.Equal(stringProp, config.StringProp);
            Assert.Equal(booleanProp, config.BooleanProp);
            Assert.Equal(integerProp, config.IntegerProp);
            Assert.Equal(dateTimeProp, config.DateTimeProp);
            Assert.Equal(timeSpanProp, config.TimeSpanProp);
            Assert.Equal(decimalProp, config.DecimalProp);
        }

        [Fact]
        public void Test_NestedConfig_ConvertedToClass()
        {
            var targetProvider = new Mock<IConfigTargetProvider>();
            var target = "dev";

            var item1 = "item1";
            var item2 = "item2";

            targetProvider.Setup(m => m.GetTargetsSequence()).Returns(new[] {target});
            var configProvider = new ConfigProvider(targetProvider.Object, "config");
            var config = configProvider.GetConfiguration<NestedObjectsConfiguration>();

            Assert.Equal(2, config.Entity.Items.Count());
            Assert.Equal(1, config.Entity.Items.Count(i => i.Name == item1));
            Assert.Equal(1, config.Entity.Items.Count(i => i.Name == item2));
        }

        [Fact]
        public void Test_UseNullResetToRemoveAllArrayItems()
        {
            var targetProvider = new Mock<IConfigTargetProvider>();

            targetProvider.Setup(m => m.GetTargetsSequence()).Returns(new[] {"dev", "reset", "prod"});
            var configProvider = new ConfigProvider(targetProvider.Object, "config");
            var config = configProvider.GetConfiguration<NestedObjectsConfiguration>();

            Assert.Equal(1, config.Entity.Items.Count());
            Assert.Equal("item3", config.Entity.Items.First().Name);
            Assert.Equal("valueDev", config.Prop);
            Assert.Equal("entityPropProd", config.Entity.Prop);
            Assert.Equal("valueDev", config.Prop);
        }

        [Fact]
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
            var configProvider = new ConfigProvider(targetProvider.Object, "config");
            var config = configProvider.GetConfiguration<SimpleFlatConfiguration>();

            Assert.Equal(devStringProp, config.StringProp);
            Assert.Equal(devBooleanProp, config.BooleanProp);
            Assert.Equal(integerProp, config.IntegerProp);
            Assert.Equal(dateTimeProp, config.DateTimeProp);
            Assert.Equal(timeSpanProp, config.TimeSpanProp);
            Assert.Equal(devDecimalProp, config.DecimalProp);

            targetProvider.Setup(m => m.GetTargetsSequence()).Returns(new[] {"dev", "prod"});
            configProvider = new ConfigProvider(targetProvider.Object, "config");
            config = configProvider.GetConfiguration<SimpleFlatConfiguration>();

            var prodDecimalProp = 13.356m;
            var prodBooleanProp = false;
            var prodStringProp = "strPROD";

            Assert.Equal(prodStringProp, config.StringProp);
            Assert.Equal(prodBooleanProp, config.BooleanProp);
            Assert.Equal(integerProp, config.IntegerProp);
            Assert.Equal(dateTimeProp, config.DateTimeProp);
            Assert.Equal(timeSpanProp, config.TimeSpanProp);
            Assert.Equal(prodDecimalProp, config.DecimalProp);
        }

        [Fact]
        public void Test_IfRequestedTargetIsNotConfigured_GetConfigurationThrowsException()
        {
            var targetProvider = new Mock<IConfigTargetProvider>();
            var target = "missing";

            targetProvider.Setup(m => m.GetTargetsSequence()).Returns(new[] {target});
            var configProvider = new ConfigProvider(targetProvider.Object, "config");
            Assert.Throws<Exception>(() => configProvider.GetConfiguration<NestedObjectsConfiguration>());
        }
    }
}