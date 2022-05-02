using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using System;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement.AspNetCore
{
    /// <summary>A feature value provider for .NET Core 3.1 / .NET 5+ which uses the
    /// <see cref="IConfiguration"/> system to obtain feature flag values.
    /// </summary>
    public class ConfigurationFeatureValueProvider : ISessionManager
    {
        private readonly IConfiguration _configuration;
        private readonly ConfigurationFeatureValueProviderSettings _providerSettings;

        public ConfigurationFeatureValueProvider(
            IConfiguration configuration,
            ConfigurationFeatureValueProviderSettings providerSettings = null
            )
        {
            _configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
            _providerSettings = providerSettings
                ?? new ConfigurationFeatureValueProviderSettings();
        }

        public async Task<bool?> GetAsync(string featureName)
        {
            if (string.IsNullOrWhiteSpace(featureName))
                return await Task.FromResult((bool?)null).ConfigureAwait(false);

            var key = string.IsNullOrWhiteSpace(_providerSettings?.SectionName)
                ? featureName
                : $"{_providerSettings.SectionName}:{featureName}";
            var value = _configuration[key];
            return bool.TryParse(value, out var result) && result;
        }

        [Obsolete("Not implemented. This is a read-only session provider.")]
        public Task SetAsync(string featureName, bool enabled)
        {
            throw new NotImplementedException();
        }
    }
}
