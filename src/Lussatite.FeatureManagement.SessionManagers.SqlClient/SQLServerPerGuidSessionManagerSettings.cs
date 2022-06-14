using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement.SessionManagers.SqlClient
{
    /// <summary>Default settings for a Microsoft SQL Server backend.  This one differs from
    /// <see cref="SQLServerSessionManagerSettings"/> in that it uses a GUID to separate
    /// out per-user feature values.  Note that if you use this with
    /// <see cref="SqlSessionManager"/> or <see cref="CachedSqlSessionManager"/> that
    /// everything needs to be registered as "scoped" / "per-request" in the DI container.</summary>
    // ReSharper disable once InconsistentNaming
    public class SQLServerPerGuidSessionManagerSettings : SqlSessionManagerSettings
    {
        public Guid UserGuid { get; set; }

        protected override string DefaultTableName => "FeatureManagementByGuid";

        private const string DefaultUserGuidColumn = "UserGuid";
        private string _featureUserGuidColumn = DefaultUserGuidColumn;

        /// <summary>The database column which contains the user (or session) GUID.</summary>
        public string FeatureUserGuidColumn
        {
            get => string.IsNullOrEmpty(_featureUserGuidColumn) ? DefaultUserGuidColumn : _featureUserGuidColumn;
            set
            {
                if (!IsValidColumnName(value))
                    throw new Exception($"{nameof(FeatureUserGuidColumn)} set to invalid value.")
                    {
                        Data = { ["value"] = value }
                    };

                _featureUserGuidColumn = value;
            }
        }

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
    [{FeatureUserGuidColumn}] UNIQUEIDENTIFIER not null,
    [{FeatureNameColumn}] nvarchar(255) not null,
    [{FeatureValueColumn}] bit,
    [{FeatureCreatedColumn}] datetimeoffset DEFAULT GETUTCDATE(),
    [{FeatureModifiedColumn}] datetimeoffset DEFAULT GETUTCDATE(),
    CONSTRAINT PK_{FeatureTableName} PRIMARY KEY CLUSTERED ({FeatureUserGuidColumn},{FeatureNameColumn})
  );
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
WHERE [{FeatureUserGuidColumn}] = @userGuid and [{FeatureNameColumn}] = @featureName;
                ";
            queryCommand.Parameters.Add(new SqlParameter("userGuid", UserGuid));
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
SET [{FeatureValueColumn}] = @featureEnabled, [{FeatureModifiedColumn}] = GETUTCDATE()
WHERE [{FeatureUserGuidColumn}] = @userGuid and [{FeatureNameColumn}] = @featureName;

IF @@ROWCOUNT = 0
BEGIN
  INSERT [{FeatureSchemaName}].[{FeatureTableName}]
  ([{FeatureUserGuidColumn}], [{FeatureNameColumn}], [{FeatureValueColumn}])
  VALUES (@userGuid, @featureName, @featureEnabled);
END

COMMIT TRANSACTION;
                ";

            queryCommand.Parameters.Add(new SqlParameter("userGuid", UserGuid));
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
SET [{FeatureValueColumn}] = @featureEnabled, [{FeatureModifiedColumn}] = GETUTCDATE()
WHERE [{FeatureUserGuidColumn}] = @userGuid and [{FeatureNameColumn}] = @featureName;

IF @@ROWCOUNT = 0
BEGIN
  INSERT [{FeatureSchemaName}].[{FeatureTableName}]
  ([{FeatureUserGuidColumn}], [{FeatureNameColumn}], [{FeatureValueColumn}])
  VALUES (@userGuid, @featureName, @featureEnabled);
END

COMMIT TRANSACTION;
                ";

            queryCommand.Parameters.Add(new SqlParameter("userGuid", UserGuid));
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
