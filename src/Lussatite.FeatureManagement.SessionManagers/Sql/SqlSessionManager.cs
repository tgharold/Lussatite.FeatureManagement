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
    public class SqlSessionManager : ILussatiteSessionManager, IHasNameProperty
    {
        private string _name;
        public string Name
        {
            get => string.IsNullOrEmpty(_name) ? GetType().Name : _name;
            set => _name = value;
        }

        public SqlSessionManagerSettings Settings { get; }

        public SqlSessionManager(
            SqlSessionManagerSettings settings
            )
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public virtual async Task<bool?> GetAsync(string featureName)
        {
            using (var conn = Settings.GetConnectionFactory())
            {
                if (conn is null)
                    throw new Exception($"Unable to obtain {nameof(DbConnection)} from connection factory.");

                using (var dbCommand = Settings.GetValueDbCommand(featureName))
                {
                    if (dbCommand is null)
                        throw new Exception($"Unable to obtain {nameof(DbCommand)} from command factory.");
                    dbCommand.Connection = conn;

                    await conn.OpenAsync().ConfigureAwait(false);
                    var dataTable = await FillDataTableAsync(dbCommand).ConfigureAwait(false);
                    if (dataTable is null) return null;
                    if (dataTable.Rows.Count == 0) return null;

                    var keyColumnValue = dataTable.Rows[0][Settings.FeatureNameColumn] as string;
                    var value = dataTable.Rows[0][Settings.FeatureValueColumn] as bool?;
                    conn.Close();

                    if (string.IsNullOrWhiteSpace(keyColumnValue)
                        || !keyColumnValue.Equals(featureName, StringComparison.OrdinalIgnoreCase))
                    {
                        var e = new Exception($"Did not find feature name in result row during {nameof(GetAsync)}.");
                        e.Data["FeatureNameColumn"] = Settings.FeatureNameColumn;
                        e.Data["FeatureNameColumnValue"] = keyColumnValue;
                        e.Data["FeatureValueColumn"] = Settings.FeatureValueColumn;
                        throw e;
                    }

                    return value;
                }
            }
        }

        /// <summary><para>This session manager does not write values back unless the
        /// <see cref="SqlSessionManagerSettings.EnableSetValueCommand"/> property on
        /// <see cref="SqlSessionManagerSettings"/> is set to true.</para>
        /// <para>This is because the Microsoft implementation always writes values back to the
        /// session manager at the end of the method.  Which is not always what you want to happen.</para>
        /// </summary>
        public virtual async Task SetAsync(string featureName, bool enabled)
        {
            if (Settings.EnableSetValueCommand != true) return;

            using (var conn = Settings.GetConnectionFactory())
            {
                if (conn is null)
                    throw new Exception($"Unable to obtain {nameof(DbConnection)} from connection factory.");

                using (var dbCommand = Settings.SetValueDbCommand(featureName, enabled))
                {
                    if (dbCommand is null)
                        throw new Exception($"Unable to obtain {nameof(DbCommand)} from command factory.");
                    dbCommand.Connection = conn;

                    await conn.OpenAsync().ConfigureAwait(false);
                    var resultCount = await dbCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                    conn.Close();

                    if (resultCount <= 0)
                    {
                        var e = new Exception($"Zero rows were affected by {nameof(SetAsync)}.");
                        e.Data["FeatureNameColumn"] = Settings.FeatureNameColumn;
                        e.Data["FeatureValueColumn"] = Settings.FeatureValueColumn;
                        e.Data["FeatureName"] = featureName;
                        e.Data["Enabled"] = enabled;
                        throw e;
                    }
                }
            }
        }

        /// <summary>Write values back to the SQL data storage.  This method mirrors the
        /// <see cref="GetAsync"/> with the use of a nullable boolean, because a
        /// <see cref="ISessionManager"/> should be able to store "don't care" / "undefined" as an answer.
        /// </summary>
        public virtual async Task SetNullableAsync(string featureName, bool? enabled)
        {
            using (var conn = Settings.GetConnectionFactory())
            {
                if (conn is null)
                    throw new Exception($"Unable to obtain {nameof(DbConnection)} from connection factory.");

                using (var dbCommand = Settings.SetNullableValueDbCommand(featureName, enabled))
                {
                    if (dbCommand is null)
                        throw new Exception($"Unable to obtain {nameof(DbCommand)} from command factory.");
                    dbCommand.Connection = conn;

                    await conn.OpenAsync().ConfigureAwait(false);
                    var resultCount = await dbCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                    conn.Close();

                    if (resultCount <= 0)
                    {
                        var e = new Exception($"Zero rows were affected by {nameof(SetAsync)}.");
                        e.Data["FeatureNameColumn"] = Settings.FeatureNameColumn;
                        e.Data["FeatureValueColumn"] = Settings.FeatureValueColumn;
                        e.Data["FeatureName"] = featureName;
                        e.Data["Enabled"] = enabled;
                        throw e;
                    }
                }
            }
        }

        private async Task<DataTable> FillDataTableAsync(DbCommand cmd)
        {
            DataTable dataTable = null;

            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false))
            {
                dataTable = new DataTable();

                var featureNameColumn = new DataColumn(Settings.FeatureNameColumn, typeof(string));
                featureNameColumn.AllowDBNull = true;
                dataTable.Columns.Add(featureNameColumn);

                var featureValueColumn = new DataColumn(Settings.FeatureValueColumn, typeof(bool));
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
