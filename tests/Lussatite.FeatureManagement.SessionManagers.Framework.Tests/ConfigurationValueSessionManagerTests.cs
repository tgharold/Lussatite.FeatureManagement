using System.Threading.Tasks;
using Xunit;

namespace Lussatite.FeatureManagement.SessionManagers.Framework.Tests
{
    public class ConfigurationValueSessionManagerTests
    {
        private ConfigurationValueSessionManager CreateSut(
            ConfigurationValueSessionManagerSettings settings = null
            )
        {
            return new ConfigurationValueSessionManager(
                sessionManagerSettings: settings
                );
        }

        [Theory]
        [InlineData(TestFeatures.NotInAppConfig)]
        [InlineData(TestFeatures.NullInAppConfig)]
        [InlineData(TestFeatures.TrueInAppConfig)]
        [InlineData(TestFeatures.StringTrueInAppConfig)]
        [InlineData(TestFeatures.FalseInAppConfig)]
        [InlineData(TestFeatures.StringFalseInAppConfig)]
        [InlineData(TestFeatures.GarbageValueInAppConfig)]
        public async Task SetAsync_can_be_called_without_changing_values(string featureName)
        {
            var sut = CreateSut();
            var originalValue = await sut.GetAsync(featureName);
            var updatedValue = !(originalValue ?? false);

            await sut.SetAsync(featureName, updatedValue);

            var latestValue = await sut.GetAsync(featureName);
            Assert.Equal(originalValue, latestValue);
        }

        [Theory]
        [InlineData(null, TestFeatures.NotInAppConfig)]
        [InlineData(null, TestFeatures.NullInAppConfig)]
        [InlineData(true, TestFeatures.TrueInAppConfig)]
        [InlineData(true, TestFeatures.StringTrueInAppConfig)]
        [InlineData(false, TestFeatures.FalseInAppConfig)]
        [InlineData(false, TestFeatures.StringFalseInAppConfig)]
        [InlineData(null, TestFeatures.GarbageValueInAppConfig)]
        public async Task GetAsync_returns_expected_for_null_settings(
            bool? expected,
            string featureName
            )
        {
            var sut = CreateSut(settings: null);
            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, TestFeatures.NotInAppConfig)]
        [InlineData(null, TestFeatures.NullInAppConfig)]
        [InlineData(true, TestFeatures.TrueInAppConfig)]
        [InlineData(true, TestFeatures.StringTrueInAppConfig)]
        [InlineData(false, TestFeatures.FalseInAppConfig)]
        [InlineData(false, TestFeatures.StringFalseInAppConfig)]
        [InlineData(null, TestFeatures.GarbageValueInAppConfig)]
        public async Task GetAsync_returns_expected_for_default_settings(
            bool? expected,
            string featureName
            )
        {
            var sut = CreateSut(settings: new ConfigurationValueSessionManagerSettings());
            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, TestFeatures.NotInAppConfig)]
        [InlineData(null, TestFeatures.NullInAppConfig)]
        [InlineData(false, TestFeatures.TrueInAppConfig)]
        [InlineData(false, TestFeatures.StringTrueInAppConfig)]
        [InlineData(true, TestFeatures.FalseInAppConfig)]
        [InlineData(true, TestFeatures.StringFalseInAppConfig)]
        [InlineData(null, TestFeatures.GarbageValueInAppConfig)]
        public async Task GetAsync_returns_expected_for_mirror_universe(
            bool? expected,
            string featureName
            )
        {
            var sut = CreateSut(
                settings: new ConfigurationValueSessionManagerSettings { SectionName = "MirrorUniverse" }
                );
            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }
    }
}
