using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using System;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement.SessionManagers
{
    /// <summary>A feature value provider for .NET Core 3.1 / .NET 5+ which uses the
    /// <see cref="IConfiguration"/> system to obtain feature flag values.
    /// This session manager is a read-only session manager.
    /// </summary>
    public class ConfigurationValueSessionManager : ISessionManager
    {
        private readonly IConfiguration _configuration;
        private readonly ConfigurationValueSessionManagerSettings _sessionManagerSettings;

        public ConfigurationValueSessionManager(
            IConfiguration configuration,
            ConfigurationValueSessionManagerSettings sessionManagerSettings = null
            )
        {
            _configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
            _sessionManagerSettings = sessionManagerSettings
                ?? new ConfigurationValueSessionManagerSettings();
        }

        public async Task<bool?> GetAsync(string featureName)
        {
            if (string.IsNullOrWhiteSpace(featureName))
                return await Task.FromResult((bool?)null).ConfigureAwait(false);

            var key = string.IsNullOrWhiteSpace(_sessionManagerSettings?.SectionName)
                ? featureName
                : $"{_sessionManagerSettings.SectionName}:{featureName}";
            var value = _configuration[key];
            if (string.IsNullOrEmpty(value)) return null;
            return bool.TryParse(value, out var result) ? result : (bool?)null;
        }

        /// <summary>This session manager does not write values back. It is a read-only provider.</summary>
        public Task SetAsync(string featureName, bool enabled)
        {
            return Task.CompletedTask;
        }
    }
}
