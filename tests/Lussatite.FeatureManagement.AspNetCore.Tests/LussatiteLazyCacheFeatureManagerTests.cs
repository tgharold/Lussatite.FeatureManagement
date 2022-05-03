using System.Collections.Generic;
using System.Threading.Tasks;
using Lussatite.FeatureManagement.LazyCache;
using Lussatite.FeatureManagement.TestingCommon;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Lussatite.FeatureManagement.AspNetCore.Tests
{
    public class LussatiteLazyCacheFeatureManagerTests
    {
        private readonly IConfiguration _configuration;

        public LussatiteLazyCacheFeatureManagerTests()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }

        [Fact]
        public void Constructor_can_accept_null_featureNames_collection()
        {
            var provider = new ConfigurationFeatureValueProvider(_configuration);
            var sut = new LussatiteLazyCacheFeatureManager(
                sessionManagers: new[] { provider },
                featureNames: null
                );
            Assert.NotNull(sut);
        }

        [Fact]
        public void Constructor_can_accept_empty_featureNames_list()
        {
            var provider = new ConfigurationFeatureValueProvider(_configuration);
            var sut = new LussatiteLazyCacheFeatureManager(
                sessionManagers: new[] { provider },
                featureNames: new List<string>()
                );
            Assert.NotNull(sut);
        }


        [Theory]
        [InlineData(false, TestFeatures.RegisteredButNotInAppConfig)]
        [InlineData(false, TestFeatures.RegisteredAndNullInAppConfig)]
        [InlineData(true, TestFeatures.RegisteredAndTrueInAppConfig)]
        [InlineData(true, TestFeatures.RegisteredAndStringTrueInAppConfig)]
        [InlineData(false, TestFeatures.RegisteredAndFalseInAppConfig)]
        [InlineData(false, TestFeatures.RegisteredAndStringFalseInAppConfig)]
        [InlineData(false, TestFeatures.RegisteredAndGarbageValueInAppConfig)]
        [InlineData(false, NotRegisteredTestFeatures.NotRegisteredButInAppConfig)]
        [InlineData(false, NotRegisteredTestFeatures.NotRegisteredAndNotInAppConfig)]
        [InlineData(false, NotRegisteredTestFeatures.NotRegisteredButGarbageValueInAppConfig)]
        public async Task IsEnabledAsync_returns_expected_for_feature(
            bool expected,
            string featureName
            )
        {
            var provider = new ConfigurationFeatureValueProvider(_configuration);
            var sut = new LussatiteLazyCacheFeatureManager(
                sessionManagers: new[] { provider },
                featureNames: TestFeatures.All.Value
                );
            var result = await sut.IsEnabledAsync(featureName);
            Assert.Equal(expected, result);
        }
    }
}
