using Microsoft.FeatureManagement;
using System.Configuration;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers.Framework
{
    /// <summary>
    /// <para>A feature value provider for .NET Framework which uses the
    /// <see cref="ConfigurationManager"/> static class to obtain feature flag
    /// values.</para>
    /// <para>The assumption is that feature flag values are stored in the "appsettings"
    /// section of the app.config / web.config files and prefixed with a string
    /// constant like "FeatureManagement:".  This assumption can be changed through
    /// <see cref="ConfigurationValueSessionManagerSettings"/>.</para>
    /// </summary>
    public class ConfigurationValueSessionManager : ISessionManager
    {
        private readonly ConfigurationValueSessionManagerSettings _sessionManagerSettings;

        public ConfigurationValueSessionManager(
            ConfigurationValueSessionManagerSettings sessionManagerSettings = null
            )
        {
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
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value)) return null;
            var boolResult = bool.TryParse(value, out var result) ? result : (bool?)null;
            return await Task.FromResult(boolResult).ConfigureAwait(false);
        }

        /// <summary>This session manager does not write values back. It is a read-only provider.</summary>
        public Task SetAsync(string featureName, bool enabled)
        {
            return Task.CompletedTask;
        }
    }
}
