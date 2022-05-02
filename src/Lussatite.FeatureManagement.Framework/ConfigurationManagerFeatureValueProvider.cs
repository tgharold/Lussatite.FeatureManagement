using System.Threading.Tasks;

namespace Lussatite.FeatureManagement.Framework
{
    public class ConfigurationManagerFeatureValueProvider : IReadOnlyFeatureValueProvider
    {
        private readonly ConfigurationManagerFeatureValueProviderOptions _providerOptions;

        public ConfigurationManagerFeatureValueProvider(
            ConfigurationManagerFeatureValueProviderOptions providerOptions = null
            )
        {
            _providerOptions = providerOptions 
                ?? new ConfigurationManagerFeatureValueProviderOptions();
        }
        
        public Task<bool?> GetAsync(string featureName)
        {
            throw new System.NotImplementedException();
        }
    }
}