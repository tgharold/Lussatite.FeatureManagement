using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement
{
    public class LussatiteFeatureManagerSnapshot : LussatiteFeatureManager
    {
        public LussatiteFeatureManagerSnapshot(
            IEnumerable<string> featureNames
            ) : base(
                featureNames: featureNames
                )
        {
        }
        
        //TODO: Have Lazy<T> list of key-values, in a cache-aside approach with no expiration
            
        /// <inheritdoc cref="LussatiteFeatureManager.IsEnabledAsync"/>
        public override Task<bool> IsEnabledAsync(string feature)
        {
            throw new System.NotImplementedException();
        }
        
        /// <inheritdoc cref="LussatiteFeatureManager.IsEnabledAsync{TContext}"/>
        public override Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}