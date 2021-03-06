using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement
{
    public class LussatiteFeatureManager : IFeatureManager
    {
        private readonly List<ISessionManager> _sessionManagers;
        private readonly List<string> _featureNames;

        public LussatiteFeatureManager(
            IEnumerable<string> featureNames,
            IEnumerable<ISessionManager> sessionManagers
            )
        {
            _featureNames = featureNames?.ToList() ?? new List<string>();
            _sessionManagers = sessionManagers?.ToList()
                ?? throw new ArgumentNullException(nameof(sessionManagers));
        }

        /// <inheritdoc cref="IFeatureManager.GetFeatureNamesAsync"/>
        public IAsyncEnumerable<string> GetFeatureNamesAsync()
        {
            return (_featureNames?.ToList() ?? new List<string>()).ToAsyncEnumerable();
        }

        /// <summary>Checks whether a given feature is enabled.</summary>
        /// <param name="feature">The name of the feature to check.  If the name was not
        /// registered in the constructor, it will always return false.</param>
        /// <returns>True if the feature is enabled, otherwise false.</returns>
        public virtual async Task<bool> IsEnabledAsync(string feature)
        {
            return await GetFeatureValueFromProviders(feature).ConfigureAwait(false);
        }

        /// <summary>Checks whether a given feature is enabled and returns information about the
        /// values returned by the registered session managers.  The session manager that first
        /// gave a definitive answer will be recorded at the top of the response object.</summary>
        /// <param name="feature">The name of the feature to check.  If the name was not
        /// registered in the constructor, it will always return false.</param>
        /// <returns><see cref="WhyEnabledResponse"/></returns>
        public virtual async Task<WhyEnabledResponse> WhyIsEnabledAsync(string feature)
        {
            var response = new WhyEnabledResponse
            {
                FeatureName = feature,
                Enabled = false,
                SessionManagerName = "n/a",
            };

            if (!FeatureIsRegistered(feature)) return response;

            bool? enabled = null;
            foreach (var sessionManager in _sessionManagers)
            {
                var result = await sessionManager.GetAsync(feature).ConfigureAwait(false);

                var sessionManagerNameObject = sessionManager as IHasNameProperty;
                var name = sessionManagerNameObject?.Name ?? sessionManager.GetType().Name;
                response.SessionManagers.Add(new WhyEnabledSessionManagerResponse
                {
                    Name = name,
                    Enabled = result,
                });

                if (enabled is null && result.HasValue)
                {
                    enabled = result.Value;
                    response.Enabled = result.Value;
                    response.SessionManagerName = name;
                }
            }

            response.Enabled = enabled == true;
            return response;
        }

        /// <summary>WARNING: This is not yet implemented (out of scope for current needs).
        /// Checks whether a given feature is enabled within the TContext.</summary>
        /// <param name="feature">The name of the feature to check.  If the name was not
        /// registered in the constructor, it will always return false.</param>
        /// <param name="context">A context providing information that can be used to evaluate whether a feature should be on or off.</param>
        /// <returns>True if the feature is enabled, otherwise false.</returns>
        public virtual Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)
        {
            throw new NotImplementedException();
        }

        private bool FeatureIsRegistered(string feature)
        {
            return !string.IsNullOrWhiteSpace(feature)
                && _featureNames?.Any(x => x == feature) == true;
        }

        private async Task<bool> GetFeatureValueFromProviders(string feature)
        {
            if (!FeatureIsRegistered(feature)) return false;

            foreach (var sessionManager in _sessionManagers)
            {
                var result = await sessionManager.GetAsync(feature).ConfigureAwait(false);
                if (result.HasValue) return result.Value;
            }

            return false;
        }
    }
}
