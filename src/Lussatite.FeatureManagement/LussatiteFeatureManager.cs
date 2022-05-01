using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;

namespace Lussatite.FeatureManagement
{
    public class LussatiteFeatureManager : IFeatureManager
    {
        private readonly List<string> _featureNames;
        private readonly string _sectionName = "FeatureManagement";

        public LussatiteFeatureManager(
            IEnumerable<string> featureNames
            )
        {
            _featureNames = featureNames?.ToList() ?? new List<string>();
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
        public virtual Task<bool> IsEnabledAsync(string feature)
        {
            return Task.FromResult(GetFeatureValueFromConfigurationManager(feature));
        }

        /// <summary>Checks whether a given feature is enabled.</summary>
        /// <param name="feature">The name of the feature to check.  If the name was not
        /// registered in the constructor, it will always return false.</param>
        /// <param name="context">A context providing information that can be used to evaluate whether a feature should be on or off.</param>
        /// <returns>True if the feature is enabled, otherwise false.</returns>
        public virtual Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)
        {
            throw new System.NotImplementedException();
        }

        private bool FeatureIsRegistered(string feature)
        {
            return !string.IsNullOrWhiteSpace(feature)
                && _featureNames?.Any(x => x == feature) == true;
        }

        protected bool GetFeatureValueFromConfigurationManager(string feature)
        {
            if (!FeatureIsRegistered(feature)) return false;
            var stringValue = ConfigurationManager.AppSettings[FeaturePath(feature)];
            
            //TODO: Need to write the code to handle both web.config XML style "sectionName:featureName" and app.config / appsettings.config JSON files
            
            return bool.TryParse(stringValue, out var boolResult) && boolResult;
        }

        private string FeaturePath(string feature)
        {
            return $"{_sectionName}.{feature}";
        }
    }
}