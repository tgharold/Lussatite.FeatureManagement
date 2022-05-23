using Microsoft.FeatureManagement;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers
{
    /// <summary>
    /// <para>A read-only or read-write implementation of <see cref="ISessionManager"/>.</para>
    /// </summary>
    public class SqlSessionManager : ILussatiteSessionManager
    {
        private readonly Func<string, DbCommand> _dbCommandGetValueFactory;
        private readonly Func<string, bool, DbCommand> _dbCommandSetValueFactory;
        private readonly Func<string, bool?, DbCommand> _dbCommandSetNullableValueFactory;
        private readonly SqlSessionManagerSettings _settings;

        /// <summary>Construct the <see cref="SqlSessionManager"/> instance.</summary>
        /// <param name="getValueCommandFactory">A <see cref="DbCommand"/> query which must
        /// filter down to the single row matching the feature name string.</param>
        /// <param name="setValueCommandFactory">An optional <see cref="DbCommand"/> query which
        /// must support being an INSERT/UPDATE (UPSERT) of the bool value into the database table.
        /// Note that you rarely want to use this in practice, unless your SQL table is already
        /// per-user or per-session.</param>
        /// <param name="setNullableValueCommandFactory">An optional <see cref="DbCommand"/> query which
        /// must support being an INSERT/UPDATE (UPSERT) of the nullable bool value into the database table.
        /// </param>
        /// <param name="settings"><see cref="SqlSessionManagerSettings"/></param>
        public SqlSessionManager(
            Func<string, DbCommand> getValueCommandFactory,
            Func<string, bool, DbCommand> setValueCommandFactory = null,
            Func<string, bool?, DbCommand> setNullableValueCommandFactory = null,
            SqlSessionManagerSettings settings = null
            )
        {
            _dbCommandGetValueFactory = getValueCommandFactory;
            _dbCommandSetValueFactory = setValueCommandFactory;
            _dbCommandSetNullableValueFactory = setNullableValueCommandFactory;
            _settings = settings ?? new SqlSessionManagerSettings();
        }

        public virtual async Task<bool?> GetAsync(string featureName)
        {
            var dbCommand = _dbCommandGetValueFactory(featureName);
            if (dbCommand is null)
                throw new Exception($"Unable to obtain {nameof(DbCommand)} from command factory.");

            var dataTable = await FillDataTableAsync(dbCommand).ConfigureAwait(false);
            if (dataTable is null) return null;
            if (dataTable.Rows.Count == 0) return null;

            var keyColumnValue = dataTable.Rows[0][_settings.FeatureNameColumn] as string;
            var value = dataTable.Rows[0][_settings.FeatureValueColumn] as bool?;

            dbCommand.Connection.Close();

            if (string.IsNullOrWhiteSpace(keyColumnValue)
                || !keyColumnValue.Equals(featureName, StringComparison.OrdinalIgnoreCase))
            {
                var e = new Exception($"Did not find feature name in result row during {nameof(GetAsync)}.");
                e.Data["FeatureNameColumn"] = _settings.FeatureNameColumn;
                e.Data["FeatureNameColumnValue"] = keyColumnValue;
                e.Data["FeatureValueColumn"] = _settings.FeatureValueColumn;
                throw e;
            }

            return value;
        }

        /// <summary>This session manager does not write values back unless the "setValueCommandFactory"
        /// <see cref="DbCommand"/> was specified in the constructor arguments.</summary>
        public virtual async Task SetAsync(string featureName, bool enabled)
        {
            if (_dbCommandSetValueFactory is null) return;
            var dbCommand = _dbCommandSetValueFactory(featureName, enabled);
            if (dbCommand is null)
                throw new Exception($"Unable to obtain {nameof(DbCommand)} from command factory.");

            var resultCount = await dbCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            dbCommand.Connection.Close();

            if (resultCount <= 0)
            {
                var e = new Exception($"Zero rows were affected by {nameof(SetAsync)}.");
                e.Data["FeatureNameColumn"] = _settings.FeatureNameColumn;
                e.Data["FeatureValueColumn"] = _settings.FeatureValueColumn;
                e.Data["FeatureName"] = featureName;
                e.Data["Enabled"] = enabled;
                throw e;
            }
        }

        /// <summary>This method does nothing unless the "setNullableValueCommandFactory"
        /// <see cref="DbCommand"/> was specified in the constructor arguments.</summary>
        public virtual async Task SetNullableAsync(string featureName, bool? enabled)
        {
            if (_dbCommandSetNullableValueFactory is null) return;
            var dbCommand = _dbCommandSetNullableValueFactory(featureName, enabled);
            if (dbCommand is null)
                throw new Exception($"Unable to obtain {nameof(DbCommand)} from command factory.");

            var resultCount = await dbCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            dbCommand.Connection.Close();

            if (resultCount <= 0)
            {
                var e = new Exception($"Zero rows were affected by {nameof(SetAsync)}.");
                e.Data["FeatureNameColumn"] = _settings.FeatureNameColumn;
                e.Data["FeatureValueColumn"] = _settings.FeatureValueColumn;
                e.Data["FeatureName"] = featureName;
                e.Data["Enabled"] = enabled;
                throw e;
            }
        }

        private async Task<DataTable> FillDataTableAsync(DbCommand cmd)
        {
            DataTable dataTable = null;

            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
            {
                dataTable = new DataTable();

                var featureNameColumn = new DataColumn(_settings.FeatureNameColumn, typeof(string));
                featureNameColumn.AllowDBNull = true;
                dataTable.Columns.Add(featureNameColumn);

                var featureValueColumn = new DataColumn(_settings.FeatureValueColumn, typeof(bool));
                featureValueColumn.AllowDBNull = true;
                dataTable.Columns.Add(featureValueColumn);

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var dataRow = dataTable.Rows.Add();
                    foreach (DataColumn dataColumn in dataTable.Columns)
                        dataRow[dataColumn.ColumnName] = reader[dataColumn.ColumnName];
                }

                reader.Close();
            }

            return dataTable;
        }
    }
}
