using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;

namespace Lussatite.FeatureManagement
{
    public class LussatiteFeatureManager : IFeatureManager
    {
        private readonly List<string> _featureNames;

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
        /// <param name="feature">The name of the feature to check.</param>
        /// <returns>True if the feature is enabled, otherwise false.</returns>
        public virtual Task<bool> IsEnabledAsync(string feature)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>Checks whether a given feature is enabled.</summary>
        /// <param name="feature">The name of the feature to check.</param>
        /// <param name="context">A context providing information that can be used to evaluate whether a feature should be on or off.</param>
        /// <returns>True if the feature is enabled, otherwise false.</returns>
        public virtual Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}