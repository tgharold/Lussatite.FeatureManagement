using Microsoft.Extensions.Configuration;

namespace Lussatite.FeatureManagement.AspNetCore;

public class ConfigurationFeatureValueProvider : IReadOnlyFeatureValueProvider
{
    private readonly IConfiguration _configuration;
    private readonly string _sectionName;

    public ConfigurationFeatureValueProvider(
        IConfiguration configuration,
        string sectionName = "FeatureManagement"
        )
    {
        if (string.IsNullOrWhiteSpace(sectionName))
            throw new ArgumentNullException(nameof(sectionName));
        _sectionName = sectionName;

        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));
        _configuration = configuration;
    }
    
    public async Task<bool?> GetAsync(string featureName)
    {
        if (string.IsNullOrWhiteSpace(featureName)) 
            return await Task.FromResult((bool?)null).ConfigureAwait(false);
        
        var value = _configuration[$"{_sectionName}:{featureName}"];
        return bool.TryParse(value, out var result) && result;
    }
}