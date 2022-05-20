using System.Threading.Tasks;
using Xunit;

namespace Lussatite.FeatureManagement.Tests
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

        /// <summary>The first session manager which returns a definitive value (true/false)
        /// instead of returning a null value will be what IsEnabledAsync() looks at.
        /// </summary>
        [Theory]
        [InlineData(false, "LMT1_null_null_null", null, null, null)]
        [InlineData(true, "LMT1_null_null_true", null, null, true)]
        [InlineData(true, "LMT1_null_true_null", null, true, null)]
        [InlineData(true, "LMT1_true_null_null", true, null, null)]
        [InlineData(true, "LMT1_true_null_true", true, null, true)]
        [InlineData(false, "LMT1_null_false_true", null, false, true)]
        public async void Multiple_sessionManagers_returns_expected(
            bool expected,
            string featureName,
            bool? s1value,
            bool? s2value,
            bool? s3value
            )
        {
            var s1 = new FakeSessionManager();
            s1.SetValue(featureName, s1value);
            var s2 = new FakeSessionManager();
            s2.SetValue(featureName, s2value);
            var s3 = new FakeSessionManager();
            s3.SetValue(featureName, s3value);
            var sut = new LussatiteLazyCacheFeatureManager(
                new[] { featureName },
                new[] { s1, s2, s3 }
            );
            var result = await sut.IsEnabledAsync(featureName);
            Assert.Equal(expected, result);
        }
    }
}
