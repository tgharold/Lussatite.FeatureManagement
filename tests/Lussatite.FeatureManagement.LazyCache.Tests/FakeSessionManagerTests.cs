using System.Threading.Tasks;
using Xunit;

namespace Lussatite.FeatureManagement.LazyCache.Tests
{
    public class FakeSessionManagerTests
    {
        [Theory]
        [InlineData(null, "A")]
        [InlineData(false, "C")]
        [InlineData(true, "A")]
        public async Task FakeSessionManager_returns_expected(bool? expected, string featureName)
        {
            var sut = new FakeSessionManager();
            sut.SetValue(featureName, expected);
            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, "F")]
        [InlineData(false, "A")]
        [InlineData(true, "E")]
        public async Task FakeSessionManager_returns_expected_after_updates(bool? expected, string featureName)
        {
            var sut = new FakeSessionManager();
            sut.SetValue(featureName, false);
            sut.SetValue(featureName, null);
            sut.SetValue(featureName, expected);
            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task FakeSessionManger_returns_null_for_unknown_key()
        {
            const string featureName = "NotInList";
            var sut = new FakeSessionManager();
            var result = await sut.GetAsync(featureName);
            Assert.Null(result);
        }
    }
}
