using System;
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

        /// <summary>Construct the <see cref="CachedSqlSessionManager"/> instance.</summary>
        /// <param name="settings"><see cref="CachedSqlSessionManagerSettings"/></param>
        /// <param name="cache">Optional application-wide <see cref="IAppCache"/> instance.</param>
        public CachedSqlSessionManager(
            CachedSqlSessionManagerSettings settings = null,
            IAppCache cache = null
            ) : base(
            settings: settings
            )
        {
            _cachedSettings = settings ?? new CachedSqlSessionManagerSettings();
            if (_cachedSettings.CacheTime.TotalSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(_cachedSettings.CacheTime));
            _cache = cache ?? new CachingService();
        }

        /// <inheritdoc cref="SqlSessionManager.GetAsync"/>
        public override async Task<bool?> GetAsync(string featureName)
        {
            if (string.IsNullOrWhiteSpace(featureName)) return false;
            var cacheKey = CalculateCacheKey(featureName);
            var absoluteExpiration = CalculateAbsoluteExpiration();

            var cacheValue = await _cache.GetOrAddAsync(
                expires: absoluteExpiration,
                key: cacheKey,
                addItemFactory: async () =>
                {
                    var value = await base.GetAsync(featureName).ConfigureAwait(false);
                    return ToCacheValue(value);
                });
            return ToNullableBoolean(cacheValue);
        }

        /// <inheritdoc cref="SqlSessionManager.SetAsync"/>
        public override async Task SetAsync(string featureName, bool enabled)
        {
            await base.SetAsync(featureName, enabled).ConfigureAwait(false);

            var cacheKey = CalculateCacheKey(featureName);
            var absoluteExpiration = CalculateAbsoluteExpiration();
            _cache.Add(cacheKey, ToCacheValue(enabled), absoluteExpiration);
        }

        /// <inheritdoc cref="SqlSessionManager.SetNullableAsync"/>
        public override async Task SetNullableAsync(string featureName, bool? enabled)
        {
            await base.SetNullableAsync(featureName, enabled).ConfigureAwait(false);

            var cacheKey = CalculateCacheKey(featureName);
            var absoluteExpiration = CalculateAbsoluteExpiration();
            // _cache.Add() does not currently support null values
            // https://github.com/alastairtree/LazyCache/issues/155
            _cache.Add(cacheKey, ToCacheValue(enabled), absoluteExpiration);
        }

        private static string CalculateCacheKey(string featureName)
        {
            return $"Lussatite.FeatureManagement:{nameof(CachedSqlSessionManager)}:{featureName}";
        }

        private DateTimeOffset CalculateAbsoluteExpiration()
        {
            return DateTimeOffset.UtcNow.Add(_cachedSettings.CacheTime);
        }

        private CacheValue ToCacheValue(bool? value)
        {
            return value.HasValue
                ? (value.Value ? CacheValue.True : CacheValue.False)
                : CacheValue.Null;
        }

        private bool? ToNullableBoolean(CacheValue cacheValue)
        {
            switch (cacheValue)
            {
                case CacheValue.Null: return null;
                case CacheValue.False: return false;
                case CacheValue.True: return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cacheValue), cacheValue, null);
            }
        }

        /// <summary>This is required because AppCacheExtensions.Add() in LazyCache throws
        /// an exception for null value. See https://github.com/alastairtree/LazyCache/issues/155
        /// </summary>
        private enum CacheValue
        {
            Null = -1,
            False = 0,
            True = 1
        }
    }
}
