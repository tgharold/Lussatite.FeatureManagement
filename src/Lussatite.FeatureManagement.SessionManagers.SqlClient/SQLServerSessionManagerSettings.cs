using System;
using System.Data;
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
if not exists
    (select * from INFORMATION_SCHEMA.TABLES
    where TABLE_SCHEMA = '{FeatureSchemaName}'
    and TABLE_NAME = '{FeatureTableName}')
begin
  create table [{FeatureSchemaName}].[{FeatureTableName}]
  (
    [{FeatureNameColumn}] nvarchar(255) not null,
    [{FeatureValueColumn}] bit,
    CONSTRAINT PK_{FeatureTableName}_{FeatureNameColumn} PRIMARY KEY CLUSTERED ({FeatureNameColumn})
  )
end
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
FROM [{FeatureSchemaName}].[{FeatureTableName}]
WHERE {FeatureNameColumn} = @featureName;
                ";
            queryCommand.Parameters.Add(new SqlParameter("featureName", featureName));
            return queryCommand;
        }

        /// <inheritdoc cref="SetValueDbCommand"/>
        public override DbCommand SetValueDbCommand(string featureName, bool enabled)
        {
            var queryCommand = new SqlCommand();

            // https://sqlperformance.com/2020/09/locking/upsert-anti-pattern
            queryCommand.CommandText =
                $@"
BEGIN TRANSACTION;

UPDATE [{FeatureSchemaName}].[{FeatureTableName}] WITH (UPDLOCK, SERIALIZABLE)
SET [{FeatureValueColumn}] = @featureEnabled
WHERE [{FeatureNameColumn}] = @featureName;

IF @@ROWCOUNT = 0
BEGIN
  INSERT [{FeatureSchemaName}].[{FeatureTableName}]
  ([{FeatureNameColumn}], [{FeatureValueColumn}])
  VALUES (@featureName, @featureEnabled);
END

COMMIT TRANSACTION;
                ";

            queryCommand.Parameters.Add(new SqlParameter("featureName", featureName));
            queryCommand.Parameters.Add(new SqlParameter("featureEnabled", enabled));

            return queryCommand;
        }

        /// <inheritdoc cref="SetNullableValueDbCommand"/>
        public override DbCommand SetNullableValueDbCommand(string featureName, bool? enabled)
        {
            var queryCommand = new SqlCommand();

            // https://sqlperformance.com/2020/09/locking/upsert-anti-pattern
            queryCommand.CommandText =
                $@"
BEGIN TRANSACTION;

UPDATE [{FeatureSchemaName}].[{FeatureTableName}] WITH (UPDLOCK, SERIALIZABLE)
SET [{FeatureValueColumn}] = @featureEnabled
WHERE [{FeatureNameColumn}] = @featureName;

IF @@ROWCOUNT = 0
BEGIN
  INSERT [{FeatureSchemaName}].[{FeatureTableName}]
  ([{FeatureNameColumn}], [{FeatureValueColumn}])
  VALUES (@featureName, @featureEnabled);
END

COMMIT TRANSACTION;
                ";

            queryCommand.Parameters.Add(new SqlParameter("featureName", featureName));
            queryCommand.Parameters.Add(new SqlParameter("featureEnabled", SqlDbType.Bit)
            {
                Value = (object) enabled ?? DBNull.Value,
                IsNullable=true
            });

            return queryCommand;
        }
    }
}
