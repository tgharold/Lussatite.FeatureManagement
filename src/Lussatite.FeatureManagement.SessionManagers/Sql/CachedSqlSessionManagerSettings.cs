using System;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers
{
    public class CachedSqlSessionManagerSettings : SqlSessionManagerSettings
    {
        /// <summary>How long a cache entry will be valid until it is forced to
        /// refresh from the database.  Defaults to 30 seconds.</summary>
        public TimeSpan CacheTime { get; set; } = TimeSpan.FromSeconds(30);
    }
}
