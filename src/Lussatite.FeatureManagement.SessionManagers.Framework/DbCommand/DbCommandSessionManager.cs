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
    public class DbCommandSessionManager : ISessionManager
    {
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
