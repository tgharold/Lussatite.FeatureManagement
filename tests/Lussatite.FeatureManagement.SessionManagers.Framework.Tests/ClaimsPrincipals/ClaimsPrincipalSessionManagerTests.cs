using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Lussatite.FeatureManagement.SessionManagers.Framework.Tests.ClaimsPrincipals
{
    public class ClaimsPrincipalSessionManagerTests
    {
        private ClaimsPrincipal CreateClaimsPrincipal(IEnumerable<string> featureFlagValues)
        {
            var claims = featureFlagValues
                .Select(x => new Claim(ClaimsPrincipalSessionManager.FeatureFlagClaimType, x))
                .ToList();
            return CreateClaimsPrincipal(claims);
        }

        private ClaimsPrincipal CreateClaimsPrincipal(List<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims: claims);
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }

        [Fact]
        public async Task GetAsync_returns_true_for_featureName_with_no_prefix()
        {
            const string featureName = "abc456-true";
            var principal = CreateClaimsPrincipal(new[]
            {
                "someOtherFeatureTrue",
                featureName,
                "!someOtherFeatureFalse",
            });
            var sut = new ClaimsPrincipalSessionManager(principal);
            var result = await sut.GetAsync(featureName);
            Assert.True(result);
        }

        [Fact]
        public async Task GetAsync_returns_false_for_featureName_with_exclamation_prefix()
        {
            const string featureName = "Xyz123-false";
            var principal = CreateClaimsPrincipal(new[]
            {
                "ThisOtherUnrelatedFeature",
                $"!{featureName}",
            });
            var sut = new ClaimsPrincipalSessionManager(principal);
            var result = await sut.GetAsync(featureName);
            Assert.False(result);
        }

        [Fact]
        public async Task GetAsync_returns_null_for_featureName_not_set()
        {
            const string featureName = "QRS175-null";
            var principal = CreateClaimsPrincipal(new[]
            {
                $"FeatureNot{featureName}",
                "TotallyNotAFeature",
                $"{featureName}NotFeatureEither",
            });
            var sut = new ClaimsPrincipalSessionManager(principal);
            var result = await sut.GetAsync(featureName);
            Assert.Null(result);
        }
    }
}
