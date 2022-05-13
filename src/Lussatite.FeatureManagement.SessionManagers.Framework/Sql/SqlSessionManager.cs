using Microsoft.FeatureManagement;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers.Framework
{
    /// <summary>
    /// <para>A read-only implementation of <see cref="ISessionManager"/> which looks
    /// at a <see cref="SqlCommand"/> to obtain the value for a feature.  The database table
    /// column names can be configured via <see cref="SqlSessionManagerSettings"/>.</para>
    /// </summary>
    public class SqlSessionManager : ISessionManager
    {
        private readonly Func<string, SqlCommand> _sqlCommandFactory;
        private readonly SqlSessionManagerSettings _settings;

        /// <summary>Construct the <see cref="SqlSessionManager"/> instance.</summary>
        /// <param name="sqlCommandFactory">Provides a <see cref="SqlCommand"/> which must
        /// filter down to the single row matching the feature name string.</param>
        /// <param name="settings"><see cref="SqlSessionManagerSettings"/></param>
        public SqlSessionManager(
            Func<string, SqlCommand> sqlCommandFactory,
            SqlSessionManagerSettings settings = null
            )
        {
            _sqlCommandFactory = sqlCommandFactory;
            _settings = settings ?? new SqlSessionManagerSettings();
        }

        public async Task<bool?> GetAsync(string featureName)
        {
            var sqlCommand = _sqlCommandFactory(featureName);
            if (sqlCommand is null)
                throw new Exception($"Unable to obtain {nameof(SqlCommand)} from {nameof(_sqlCommandFactory)}.");

            var dataTable = await FillDataTableAsync(sqlCommand).ConfigureAwait(false);
            if (dataTable is null) return null;
            if (dataTable.Rows.Count == 0) return null;

            var keyColumnValue = dataTable.Rows[0][_settings.FeatureNameColumn] as string;
            if (string.IsNullOrWhiteSpace(keyColumnValue)
                || !keyColumnValue.Equals(featureName, StringComparison.OrdinalIgnoreCase))
            {
                var e = new Exception("Did not find feature name in result row.");
                e.Data["FeatureNameColumn"] = _settings.FeatureNameColumn;
                e.Data["FeatureNameColumnValue"] = keyColumnValue;
                throw e;
            }

            var value = dataTable.Rows[0][_settings.FeatureValueColumn] as bool?;

            return value;
        }

        /// <summary>This session manager does not write values back. It is a read-only provider.</summary>
        public Task SetAsync(string featureName, bool enabled)
        {
            return Task.CompletedTask;
        }

        private async Task<DataTable> FillDataTableAsync(SqlCommand cmd)
        {
            DataTable dataTable = null;
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess).ConfigureAwait(false))
            {

                dataTable = new DataTable();
                dataTable.Columns.Add(_settings.FeatureNameColumn, typeof(string));
                dataTable.Columns.Add(_settings.FeatureValueColumn, typeof(bool?));

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var dataRow = dataTable.Rows.Add();
                    foreach (DataColumn dataColumn in dataTable.Columns)
                        dataRow[dataColumn.ColumnName] = reader[dataColumn.ColumnName];
                }
            }
            return dataTable;
        }
    }
}