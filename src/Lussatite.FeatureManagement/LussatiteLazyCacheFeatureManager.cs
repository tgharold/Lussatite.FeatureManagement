using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyCache;
using LazyCache.Providers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.FeatureManagement;

namespace Lussatite.FeatureManagement
{
    /// <summary>An in-memory caching version of <see cref="IFeatureManagerSnapshot"/>.  Once
    /// a key has been read via <see cref="IsEnabledAsync(string)"/> it will never be fetched
    /// again from the underlying feature value provider.  This protects the application from
    /// values that change in the middle of a session or a request.  For use cases where you
    /// want a strong guarantee that all feature values were fetched at (roughly) the same
    /// time, you should call <see cref="CacheAllFeatureValuesAsync"/>.
    /// This should probably be registered with a scoped / per-request lifetime.
    /// </summary>
    public class LussatiteLazyCacheFeatureManager : LussatiteFeatureManager, IFeatureManagerSnapshot
    {
        readonly IAppCache _cache;
        private readonly Lazy<ICacheProvider> _cacheProvider = new Lazy<ICacheProvider>(() =>
            new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())));

        public LussatiteLazyCacheFeatureManager(
            IEnumerable<string> featureNames,
            IEnumerable<ISessionManager> sessionManagers
            ) : base(
                featureNames: featureNames,
                sessionManagers: sessionManagers
                )
        {
            // There's a subtle trap here.  Even though we new up a new caching service, because we
            // failed to pass in our own CacheProvider, we end up using the static:
            //      public static Lazy<ICacheProvider> DefaultCacheProvider
            _cache = new CachingService(_cacheProvider);
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
            throw new NotImplementedException();
        }

        /// <summary>Read all registered feature names into the local cache.</summary>
        public async Task CacheAllFeatureValuesAsync()
        {
            var featureNames = await GetFeatureNamesAsync().ToListAsync().ConfigureAwait(false);
            foreach (var featureName in featureNames)
            {
                _ = await IsEnabledAsync(featureName).ConfigureAwait(false);
            }
        }
    }
}
