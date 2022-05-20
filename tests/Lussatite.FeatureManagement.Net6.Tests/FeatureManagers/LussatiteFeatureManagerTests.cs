using System.Collections.Generic;
using System.Threading.Tasks;
using Lussatite.FeatureManagement.SessionManagers;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Lussatite.FeatureManagement.Net6.Tests.FeatureManagers
{
    public class LussatiteFeatureManagerTests
    {
        private readonly IConfiguration _configuration;

        public LussatiteFeatureManagerTests()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }

        [Fact]
        public void Constructor_can_accept_null_featureNames_collection()
        {
            var provider = new ConfigurationValueSessionManager(_configuration);
            var sut = new LussatiteFeatureManager(
                sessionManagers: new[] { provider },
                featureNames: null
                );
            Assert.NotNull(sut);
        }

        [Fact]
        public void Constructor_can_accept_empty_featureNames_list()
        {
            var provider = new ConfigurationValueSessionManager(_configuration);
            var sut = new LussatiteFeatureManager(
                sessionManagers: new[] { provider },
                featureNames: new List<string>()
            );
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData(false, TestFeatures.NotInAppConfig)]
        [InlineData(false, TestFeatures.NullInAppConfig)]
        [InlineData(true, TestFeatures.TrueInAppConfig)]
        [InlineData(true, TestFeatures.StringTrueInAppConfig)]
        [InlineData(false, TestFeatures.FalseInAppConfig)]
        [InlineData(false, TestFeatures.StringFalseInAppConfig)]
        [InlineData(false, TestFeatures.GarbageValueInAppConfig)]
        [InlineData(false, NotRegisteredTestFeatures.NotRegisteredButInAppConfig)]
        [InlineData(false, NotRegisteredTestFeatures.NotRegisteredAndNotInAppConfig)]
        [InlineData(false, NotRegisteredTestFeatures.NotRegisteredButGarbageValueInAppConfig)]
        public async Task IsEnabledAsync_returns_expected_for_feature(
            bool expected,
            string featureName
            )
        {
            var provider = new ConfigurationValueSessionManager(_configuration);
            var sut = new LussatiteFeatureManager(
                sessionManagers: new[] { provider },
                featureNames: TestFeatures.All.Value
                );
            var result = await sut.IsEnabledAsync(featureName);
            Assert.Equal(expected, result);
        }
    }
}
