using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers.Framework
{
    /// <summary><para>WARN: This class is still under active development and will likely change
    /// names before the final release.</para>
    /// <para>A read-only implementation of <see cref="ISessionManager"/> which looks
    /// at a database table to obtain the values.  The database table schema/name and column
    /// names can be configured via <see cref="DbCommandSessionManagerSettings"/>.</para>
    /// </summary>
    public class DbCommandSessionManager : ISessionManager
    {
        private readonly DbCommandSessionManagerSettings _settings;

        public DbCommandSessionManager(DbCommandSessionManagerSettings settings = null)
        {
            _settings = settings ?? new DbCommandSessionManagerSettings();
        }

        public async Task<bool?> GetAsync(string featureName)
        {
            // "select FeatureName, Feature from [{schema}].[{tableName}]"

            return await Task.FromResult((bool?) null).ConfigureAwait(false);
        }

        /// <summary>This session manager does not write values back. It is a read-only provider.</summary>
        public Task SetAsync(string featureName, bool enabled)
        {
            return Task.CompletedTask;
        }
    }
}
