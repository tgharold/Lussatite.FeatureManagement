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

        /// <summary>Construct the <see cref="CachedSqlSessionManager"/> instance.</summary>
        /// <param name="getValueCommandFactory">A <see cref="DbCommand"/> query which must
        /// filter down to the single row matching the feature name string.</param>
        /// <param name="setValueCommandFactory">An optional <see cref="DbCommand"/> query which
        /// must support being an INSERT/UPDATE (UPSERT) of the bool value into the database table.
        /// Note that you rarely want to use this in practice, unless your SQL table is already
        /// per-user or per-session.</param>
        /// <param name="setNullableValueCommandFactory">An optional <see cref="DbCommand"/> query which
        /// must support being an INSERT/UPDATE (UPSERT) of the nullable bool value into the database table.
        /// </param>
        /// <param name="settings"><see cref="CachedSqlSessionManagerSettings"/></param>
        /// <param name="cache">Optional application-wide <see cref="IAppCache"/> instance.</param>
        public CachedSqlSessionManager(
            Func<string, DbCommand> getValueCommandFactory,
            Func<string, bool, DbCommand> setValueCommandFactory = null,
            Func<string, bool?, DbCommand> setNullableValueCommandFactory = null,
            CachedSqlSessionManagerSettings settings = null,
            IAppCache cache = null
            ) : base(
            settings: settings,
            getValueCommandFactory: getValueCommandFactory,
            setValueCommandFactory: setValueCommandFactory,
            setNullableValueCommandFactory: setNullableValueCommandFactory
            )
        {
            _cachedSettings = settings ?? new CachedSqlSessionManagerSettings();
            if (_cachedSettings.CacheTime.Seconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(_cachedSettings.CacheTime));
            _cache = cache ?? new CachingService();
        }

        /// <inheritdoc cref="SqlSessionManager.GetAsync"/>
        public override async Task<bool?> GetAsync(string featureName)
        {
            if (string.IsNullOrWhiteSpace(featureName)) return false;
            var cacheKey = CalculateCacheKey(featureName);
            var absoluteExpiration = CalculateAbsoluteExpiration();

            return await _cache.GetOrAddAsync(
                expires: absoluteExpiration,
                key: cacheKey,
                addItemFactory: async () => await base.GetAsync(featureName).ConfigureAwait(false)
                );
        }

        /// <inheritdoc cref="SqlSessionManager.SetAsync"/>
        public override async Task SetAsync(string featureName, bool enabled)
        {
            await base.SetAsync(featureName, enabled).ConfigureAwait(false);

            var cacheKey = CalculateCacheKey(featureName);
            var absoluteExpiration = CalculateAbsoluteExpiration();
            _cache.Add(cacheKey, enabled, absoluteExpiration);
        }

        /// <inheritdoc cref="SqlSessionManager.SetNullableAsync"/>
        public override async Task SetNullableAsync(string featureName, bool? enabled)
        {
            await base.SetNullableAsync(featureName, enabled).ConfigureAwait(false);

            var cacheKey = CalculateCacheKey(featureName);
            var absoluteExpiration = CalculateAbsoluteExpiration();
            _cache.GetOrAdd(
                expires: absoluteExpiration,
                key: cacheKey,
                addItemFactory: () => enabled
                );
            // _cache.Add() does not currently support null values
            // https://github.com/alastairtree/LazyCache/issues/155
            // _cache.Add(cacheKey, enabled, absoluteExpiration);
        }

        private static string CalculateCacheKey(string featureName)
        {
            return $"Lussatite.FeatureManagement:{nameof(CachedSqlSessionManager)}:{featureName}";
        }

        private DateTimeOffset CalculateAbsoluteExpiration()
        {
            return DateTimeOffset.UtcNow.Add(_cachedSettings.CacheTime);
        }
    }
}
