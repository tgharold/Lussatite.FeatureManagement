using Microsoft.FeatureManagement;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers.Framework
{
    /// <summary><para>WARN: This class is still under active development and will likely change
    /// names/implementation before the final release.</para>
    /// <para>A read-only implementation of <see cref="ISessionManager"/> which looks
    /// at a <see cref="SqlCommand"/> to obtain the values.  The database table column
    /// names can be configured via <see cref="DataReaderSessionManagerSettings"/>.</para>
    /// </summary>
    public class DataReaderSessionManager : ISessionManager
    {
        private readonly Func<string, SqlCommand> sqlCommandFactory;
        private readonly DataReaderSessionManagerSettings _settings;

        /// <summary>Construct the <see cref="DataReaderSessionManager"/> by offloading how to retrieve
        /// the data row using SqlCommand.</summary>
        /// <param name="sqlCommandFactory">Provides a <see cref="SqlCommand"/> which filters down to
        /// the single row matching the feature name string.</param>
        /// <param name="settings"><see cref="DataReaderSessionManagerSettings"/></param>
        public DataReaderSessionManager(
            Func<string, SqlCommand> sqlCommandFactory,
            DataReaderSessionManagerSettings settings = null
            )
        {
            this.sqlCommandFactory = sqlCommandFactory;
            _settings = settings ?? new DataReaderSessionManagerSettings();
        }

        public async Task<bool?> GetAsync(string featureName)
        {
            var sqlCommand = sqlCommandFactory(featureName);
            if (sqlCommand is null)
                throw new Exception($"Unable to obtain {nameof(SqlCommand)} from {nameof(sqlCommandFactory)}.");

            var table = await FillDataTableAsync(sqlCommand);
            if (table is null) return null;
            if (table.Rows.Count == 0) return null;

            var value = table.Rows[0][_settings.FeatureValueColumn] as bool?;

            return value;
        }

        /// <summary>This session manager does not write values back. It is a read-only provider.</summary>
        public Task SetAsync(string featureName, bool enabled)
        {
            return Task.CompletedTask;
        }

        private async Task<DataTable> FillDataTableAsync(SqlCommand cmd)
        {
            DataTable dt = null;
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
            {

                dt = new DataTable();
                dt.Columns.Add(_settings.FeatureNameColumn, typeof(string));
                dt.Columns.Add(_settings.FeatureValueColumn, typeof(bool?));

                while (await reader.ReadAsync())
                {
                    DataRow dr = dt.Rows.Add();
                    foreach (DataColumn col in dt.Columns)
                        dr[col.ColumnName] = reader[col.ColumnName];
                }
            }
            return dt;
        }
    }
}
