using Microsoft.FeatureManagement;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement.Framework
{
    /// <summary>
    /// <para>A feature value provider for .NET Framework which uses the
    /// <see cref="ConfigurationManager"/> static class to obtain feature flag
    /// values.</para>
    /// <para>The assumption is that feature flag values are stored in the "appsettings"
    /// section of the app.config / web.config files and prefixed with a string
    /// constant like "FeatureManagement:".  This assumption can be changed through
    /// <see cref="ConfigurationFeatureValueProviderSettings"/>.</para>
    /// </summary>
    public class ConfigurationFeatureValueProvider : ISessionManager
    {
        private readonly ConfigurationFeatureValueProviderSettings _providerSettings;

        public ConfigurationFeatureValueProvider(
            ConfigurationFeatureValueProviderSettings providerSettings = null
            )
        {
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
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value)) return null;
            return bool.TryParse(value, out var result) ? result : (bool?)null;
        }

        [Obsolete("Not implemented. This is a read-only session provider.")]
        public Task SetAsync(string featureName, bool enabled)
        {
            throw new NotImplementedException();
        }
    }
}
