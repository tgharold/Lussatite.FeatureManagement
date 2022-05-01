using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Lussatite.FeatureManagement.Tests;

public class LussatiteFeatureManagerTests
{
    [Fact]
    public void Constructor_can_accept_null_featureNames_collection()
    {
        var sut = new LussatiteFeatureManager(featureNames: null);
        Assert.NotNull(sut);
    }
    
    [Fact]
    public void Constructor_can_accept_empty_featureNames_list()
    {
        var sut = new LussatiteFeatureManager(featureNames: new List<string>());
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
        var sut = new LussatiteFeatureManager(featureNames: TestFeatures.All.Value);
        var result = await sut.IsEnabledAsync(featureName);
        Assert.Equal(expected, result);
    }
}