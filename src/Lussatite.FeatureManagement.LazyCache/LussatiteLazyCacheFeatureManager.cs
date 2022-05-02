using LazyCache;
using Microsoft.FeatureManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement.LazyCache
{
    public class LussatiteLazyCacheFeatureManager : LussatiteFeatureManager, IFeatureManagerSnapshot
    {
        IAppCache _cache;

        public LussatiteLazyCacheFeatureManager(
            IEnumerable<string> featureNames,
            IEnumerable<IReadOnlyFeatureValueProvider> readOnlyFeatureValueProviders,
            IAppCache cache = null
            ) : base(
                featureNames: featureNames,
                readOnlyFeatureValueProviders: readOnlyFeatureValueProviders
                )
        {
            _cache = cache ?? new CachingService();
        }

        /// <summary>Returns the feature's value which will remain the same for the
        /// rest of the lifespan of this object (in-memory cache).</summary>
        public override async Task<bool> IsEnabledAsync(string feature)
        {
            if (string.IsNullOrWhiteSpace(feature)) return false;
            var cacheKey = $"{nameof(LussatiteLazyCacheFeatureManager)}:{feature}";

            return await _cache.GetOrAddAsync(
                cacheKey,
                async () => await base.IsEnabledAsync(feature).ConfigureAwait(false)
                );
        }

        /// <inheritdoc cref="LussatiteFeatureManager.IsEnabledAsync{TContext}"/>
        public override Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
