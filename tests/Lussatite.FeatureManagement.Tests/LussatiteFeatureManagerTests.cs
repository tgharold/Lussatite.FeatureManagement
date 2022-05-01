using System.Collections.Generic;
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
}