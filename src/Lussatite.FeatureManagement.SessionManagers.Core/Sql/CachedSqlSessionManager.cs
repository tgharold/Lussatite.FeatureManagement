using System;
using System.Data.Common;
using System.Threading.Tasks;
using LazyCache;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers
{
    /// <summary>A caching version of <see cref="SqlSessionManager"/> which reduces the
    /// number of calls to the underlying database.</summary>
    public class CachedSqlSessionManager : SqlSessionManager
    {
        private readonly IAppCache _cache;
        private readonly CachedSqlSessionManagerSettings _cachedSettings;

        public CachedSqlSessionManager(
            Func<string, DbCommand> getValueCommandFactory,
            CachedSqlSessionManagerSettings settings = null,
            IAppCache cache = null
            ) : base(getValueCommandFactory, settings)
        {
            _cachedSettings = settings ?? new CachedSqlSessionManagerSettings();
            if (_cachedSettings.CacheTime.Seconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(_cachedSettings.CacheTime));
            _cache = cache ?? new CachingService();
        }

        public override async Task<bool?> GetAsync(string featureName)
        {
            if (string.IsNullOrWhiteSpace(featureName)) return false;
            var cacheKey = $"Lussatite.FeatureManagement:{nameof(CachedSqlSessionManager)}:{featureName}";
            var absoluteExpiration = DateTimeOffset.UtcNow.Add(_cachedSettings.CacheTime);

            return await _cache.GetOrAddAsync(
                expires: absoluteExpiration,
                key: cacheKey,
                addItemFactory: async () => await base.GetAsync(featureName).ConfigureAwait(false)
                );
        }
    }
}
