using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lussatite.FeatureManagement.SessionManagers;
using Xunit;

namespace Lussatite.FeatureManagement.NetCore31.Tests.SessionManagers.Static
{
    public class StaticAnswerSessionManagerTests
    {
        private const string Pfx = TestConstants.Prefix;
        private const string TrueFeature = Pfx + nameof(TrueFeature);
        private const string FalseFeature = Pfx + nameof(FalseFeature);
        private const string NullFeature = Pfx + nameof(NullFeature);

        private readonly StaticAnswerSessionManager _sut = new StaticAnswerSessionManager(
            new Dictionary<string, bool?>(StringComparer.OrdinalIgnoreCase)
            {
                [TrueFeature] = true,
                [FalseFeature] = false,
            });

        [Theory]
        [InlineData(null, NullFeature)]
        [InlineData(true, TrueFeature)]
        [InlineData(false, FalseFeature)]
        public async Task GetAsync_returns_expected(bool? expected, string featureName)
        {
            var result = await _sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }
    }
}

