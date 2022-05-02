using Microsoft.FeatureManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement.LazyCache
{
    public class LussatiteLazyCacheFeatureManager : LussatiteFeatureManager, IFeatureManagerSnapshot
    {
        public LussatiteLazyCacheFeatureManager(
            IEnumerable<string> featureNames,
            IEnumerable<IReadOnlyFeatureValueProvider> readOnlyFeatureValueProviders
            ) : base(
                featureNames: featureNames,
                readOnlyFeatureValueProviders: readOnlyFeatureValueProviders
                )
        {
        }

        //TODO: Have Lazy<T> list of key-values, in a cache-aside approach with no expiration

        /// <inheritdoc cref="LussatiteFeatureManager.IsEnabledAsync"/>
        public override async Task<bool> IsEnabledAsync(string feature)
        {
            return await base.IsEnabledAsync(feature);

            //TODO: Read from a caching provider (Redis, LazyCache, etc.)            
            //return await base.IsEnabledAsync(feature);
        }

        /// <inheritdoc cref="LussatiteFeatureManager.IsEnabledAsync{TContext}"/>
        public override Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
