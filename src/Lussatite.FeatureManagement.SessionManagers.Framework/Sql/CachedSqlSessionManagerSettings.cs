using System;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers.Framework
{
    public class CachedSqlSessionManagerSettings : SqlSessionManagerSettings
    {
        //NOTE: This is a copy of the one from Lussatite.FeatureManagement.SessionManagers.Core
        //Because DbCommand is not .NET Standard 2.0, we are splitting along Framework/Core lines.

        /// <summary>How long a cache entry will be valid until it is forced to
        /// refresh from the database.  Defaults to 30 seconds.</summary>
        public TimeSpan CacheTime { get; set; } = TimeSpan.FromSeconds(30);
    }
}
