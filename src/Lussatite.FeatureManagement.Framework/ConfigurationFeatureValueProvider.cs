using System.Configuration;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement.Framework
{
    /// <summary>
    /// <para>NOTE: This is designed for .NET Framework and the use of <see cref="ConfigurationManager"/>.</para>
    /// </summary>
    public class ConfigurationFeatureValueProvider : IReadOnlyFeatureValueProvider
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
            return bool.TryParse(value, out var result) && result;
        }
    }
}
