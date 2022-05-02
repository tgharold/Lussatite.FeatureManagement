using Microsoft.Extensions.Configuration;

namespace Lussatite.FeatureManagement.AspNetCore;

public class ConfigurationFeatureValueProvider : IReadOnlyFeatureValueProvider
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
}
