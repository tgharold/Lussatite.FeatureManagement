using System;
using System.Linq;
using Microsoft.FeatureManagement;
using System.Security.Claims;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers
{
    /// <summary>
    /// <para>A read-only <see cref="ISessionManager"/> implementation that examines claims
    /// of type <see cref="FeatureFlagClaimType"/> to determine the specified feature flag
    /// name's value.</para>
    /// <para>This type should be registered/constructed on a per-request basis
    /// since it takes dependency on the current user (<see cref="ClaimsPrincipal"/>).</para>
    /// </summary>
    public class ClaimsPrincipalSessionManager : ISessionManager, IHasNameProperty
    {
        private string _name;
        public string Name
        {
            get => string.IsNullOrEmpty(_name) ? GetType().Name : _name;
            set => _name = value;
        }

        /// <summary>The claim type that will be examined when determining whether the
        /// current <see cref="ClaimsPrincipal"/> has the claim for a particular
        /// feature name. The expectation is that the value of this claim will
        /// be something that can be parsed as the feature name.  Feature names
        /// that are prefixed with an exclamation point indicate "false".</summary>
        public const string FeatureFlagClaimType = "feature_flag";

        // https://docs.microsoft.com/en-us/dotnet/api/system.security.claims.claimtypes

        private readonly ClaimsPrincipal _claimsPrincipal;

        /// <summary>Construct the session manager associated with the specified
        /// <see cref="ClaimsPrincipal"/> object, such as the user.</summary>
        /// <param name="claimsPrincipal"><see cref="ClaimsPrincipal"/></param>
        public ClaimsPrincipalSessionManager(
            ClaimsPrincipal claimsPrincipal
            )
        {
            _claimsPrincipal = claimsPrincipal
                ?? throw new ArgumentNullException(nameof(claimsPrincipal));
        }

        /// <summary>This session manager does not write values back. It is a read-only provider.</summary>
        public Task SetAsync(string featureName, bool enabled)
        {
            return Task.CompletedTask;
        }

        public async Task<bool?> GetAsync(string featureName)
        {
            var featureClaim = await Task.FromResult(_claimsPrincipal.Claims
                .FirstOrDefault(x =>
                    x.Type == FeatureFlagClaimType
                    && (
                        x.Value.Equals(featureName, StringComparison.OrdinalIgnoreCase)
                        || x.Value.Equals($"!{featureName}", StringComparison.OrdinalIgnoreCase)
                        )
                    ));

            if (featureClaim is null) return null;
            return featureClaim.Value.Substring(0, 1) != "!";
        }
    }
}
