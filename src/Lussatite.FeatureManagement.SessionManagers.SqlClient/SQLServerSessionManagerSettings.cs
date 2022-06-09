using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement.SessionManagers.SqlClient
{
    /// <summary>Default settings for a Microsoft SQL Server backend.</summary>
    // ReSharper disable once InconsistentNaming
    public class SQLServerSessionManagerSettings : SqlSessionManagerSettings
    {
        /// <inheritdoc cref="GetConnectionFactory"/>
        public override DbConnection GetConnectionFactory()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new Exception(
                    $"Missing {nameof(ConnectionString)} value in {nameof(GetConnectionFactory)}()."
                    );
            return new SqlConnection(ConnectionString);
        }

        private string GetCreateDatabaseTableSql() =>
            $@"
CREATE TABLE IF NOT EXISTS [{FeatureTableName}] (
    [{FeatureNameColumn}] TEXT PRIMARY KEY,
    [{FeatureValueColumn}] BOOLEAN
        CHECK ([{FeatureValueColumn}] IN (0, 1))
);
            ";

        /// <inheritdoc cref="CreateDatabaseTable"/>
        public override void CreateDatabaseTable(string tableCreateConnectionString)
        {
            if (string.IsNullOrWhiteSpace(tableCreateConnectionString))
                throw new Exception($"{nameof(tableCreateConnectionString)} was not set.");

            using (var conn = new SqlConnection(tableCreateConnectionString))
            {
                conn.Open();
                using (var createCommand = conn.CreateCommand())
                {
                    createCommand.CommandText = GetCreateDatabaseTableSql();
                    createCommand.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        /// <inheritdoc cref="CreateDatabaseTableAsync"/>
        public override async Task CreateDatabaseTableAsync(string tableCreateConnectionString)
        {
            if (string.IsNullOrWhiteSpace(tableCreateConnectionString))
                throw new Exception($"{nameof(tableCreateConnectionString)} was not set.");

            using (var conn = new SqlConnection(tableCreateConnectionString))
            {
                await conn.OpenAsync();
                using (var createCommand = conn.CreateCommand())
                {
                    createCommand.CommandText = GetCreateDatabaseTableSql();
                    await createCommand.ExecuteNonQueryAsync();
                }
                conn.Close();
            }
        }

        /// <inheritdoc cref="GetValueDbCommand"/>
        public override DbCommand GetValueDbCommand(string featureName)
        {
            var queryCommand = new SqlCommand();
            queryCommand.CommandText =
                $@"
SELECT [{FeatureNameColumn}], [{FeatureValueColumn}]
FROM [{FeatureTableName}]
WHERE [{FeatureNameColumn}] = @featureName;
                ";
            queryCommand.Parameters.Add(new SqlParameter("featureName", featureName));
            return queryCommand;
        }

        /// <inheritdoc cref="SetValueDbCommand"/>
        public override DbCommand SetValueDbCommand(string featureName, bool enabled)
        {
            var featureValue = enabled ? 1 : 0;
            var queryCommand = new SqlCommand();

            queryCommand.CommandText =
                $@"
INSERT INTO [{FeatureTableName}]
([{FeatureNameColumn}], [{FeatureValueColumn}])
VALUES (@featureName, @featureValue)
ON CONFLICT([{FeatureNameColumn}])
DO UPDATE SET [{FeatureValueColumn}]=@featureValue
                ";

            queryCommand.Parameters.Add(new SqlParameter("featureName", featureName));
            queryCommand.Parameters.Add(new SqlParameter("featureValue", featureValue));

            return queryCommand;
        }

        /// <inheritdoc cref="SetNullableValueDbCommand"/>
        public override DbCommand SetNullableValueDbCommand(string featureName, bool? enabled)
        {
            int? featureValue = null;
            if (enabled.HasValue) featureValue = enabled.Value ? 1 : 0;
            var queryCommand = new SqlCommand();

            queryCommand.CommandText =
                $@"
INSERT INTO [{FeatureTableName}]
([{FeatureNameColumn}], [{FeatureValueColumn}])
VALUES (@featureName, @featureValue)
ON CONFLICT([{FeatureNameColumn}])
DO UPDATE SET [{FeatureValueColumn}]=@featureValue
                ";

            queryCommand.Parameters.Add(new SqlParameter("featureName", featureName));
            queryCommand.Parameters.Add(new SqlParameter("featureValue", featureValue));

            return queryCommand;
        }
    }
}
