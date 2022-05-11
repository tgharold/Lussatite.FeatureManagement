using System.Threading.Tasks;
using Xunit;

namespace Lussatite.FeatureManagement.LazyCache.Tests
{
    public class LussatiteLazyCacheFeatureManagerTests
    {
        [Fact]
        public async Task Unknown_feature_name_returns_false()
        {
            const string featureName = "UnknownAlphaOne";
            var sessionManager = new FakeSessionManager();
            var sut = new LussatiteLazyCacheFeatureManager(
                new[] { featureName },
                new[] { sessionManager }
                );
            var result = await sut.IsEnabledAsync(featureName);
            Assert.False(result);
        }

        [Theory]
        [InlineData(false, "A1", null)]
        [InlineData(false, "B2", false)]
        [InlineData(true, "C3", true)]
        public async Task Feature_set_to_value_returns_expected(
            bool expected,
            string featureName,
            bool? value
            )
        {
            var sessionManager = new FakeSessionManager();
            sessionManager.SetValue(featureName, value);
            var sut = new LussatiteLazyCacheFeatureManager(
                new[] { featureName },
                new[] { sessionManager }
            );
            var result = await sut.IsEnabledAsync(featureName);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(false, "X-A", null, true)]
        [InlineData(false, "Z-B", false, true)]
        [InlineData(true, "Y-C", true, false)]
        public async Task Feature_set_to_value_after_first_read_returns_first_read_value(
            bool expected,
            string featureName,
            bool? value,
            bool? updatedValue
            )
        {
            Assert.NotEqual(value, updatedValue);
            var sessionManager = new FakeSessionManager();
            sessionManager.SetValue(featureName, value);
            var sut = new LussatiteLazyCacheFeatureManager(
                new[] { featureName },
                new[] { sessionManager }
            );
            var firstReadResult = await sut.IsEnabledAsync(featureName);
            Assert.Equal(expected, firstReadResult);
            sessionManager.SetValue(featureName, updatedValue);
            var result = await sut.IsEnabledAsync(featureName);
            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(false, "All-X-A", null, true)]
        [InlineData(false, "All-Z-B", false, true)]
        [InlineData(true, "All-Y-C", true, false)]
        public async Task Feature_set_to_value_after_cacheAll_returns_first_read_value(
            bool expected,
            string featureName,
            bool? value,
            bool? updatedValue
            )
        {
            Assert.NotEqual(value, updatedValue);
            var sessionManager = new FakeSessionManager();
            sessionManager.SetValue(featureName, value);
            var sut = new LussatiteLazyCacheFeatureManager(
                new[] { featureName },
                new[] { sessionManager }
            );
            await sut.CacheAllFeatureValuesAsync();
            sessionManager.SetValue(featureName, updatedValue);
            var result = await sut.IsEnabledAsync(featureName);
            Assert.Equal(expected, result);
        }
    }
}
