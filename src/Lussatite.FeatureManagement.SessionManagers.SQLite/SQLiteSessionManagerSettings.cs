using System;
using System.Data.Common;
using System.Data.SQLite;

namespace Lussatite.FeatureManagement.SessionManagers.SQLite
{
    /// <summary>A default set of settings for a SQLite backend.</summary>
    // ReSharper disable once InconsistentNaming
    public class SQLiteSessionManagerSettings : SqlSessionManagerSettings
    {
        /// <inheritdoc cref="GetConnectionFactory"/>
        public override Func<DbConnection> GetConnectionFactory => () =>
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new Exception(
                    $"Missing {nameof(ConnectionString)} value in {nameof(GetConnectionFactory)}()."
                    );
            return new SQLiteConnection(ConnectionString);
        };

        /// <inheritdoc cref="CreateDatabaseTableFactory"/>
        public override Func<DbCommand> CreateDatabaseTableFactory => () =>
        {
            var queryCommand = new SQLiteCommand();
            queryCommand.CommandText =
                $@"
CREATE TABLE IF NOT EXISTS [{FeatureTableName}] (
    [{FeatureNameColumn}] TEXT PRIMARY KEY,
    [{FeatureValueColumn}] BOOLEAN
        CHECK ([{FeatureValueColumn}] IN (0, 1))
);
            ";
            return queryCommand;
        };

        /// <inheritdoc cref="GetValueCommandFactory"/>
        public override Func<string, DbCommand> GetValueCommandFactory => featureName =>
        {
            var queryCommand = new SQLiteCommand();
            queryCommand.CommandText =
                $@"
SELECT [{FeatureNameColumn}], [{FeatureValueColumn}]
FROM [{FeatureTableName}]
WHERE [{FeatureNameColumn}] = @featureName;
                ";
            queryCommand.Parameters.Add(new SQLiteParameter("featureName", featureName));
            return queryCommand;
        };

        /// <inheritdoc cref="SetValueCommandFactory"/>
        public override Func<string, bool, DbCommand> SetValueCommandFactory => (featureName, enabled) =>
        {
            var featureValue = enabled ? 1 : 0;
            var queryCommand = new SQLiteCommand();

            queryCommand.CommandText =
                $@"
INSERT INTO [{FeatureTableName}]
([{FeatureNameColumn}], [{FeatureValueColumn}])
VALUES (@featureName, @featureValue)
ON CONFLICT([{FeatureNameColumn}])
DO UPDATE SET [{FeatureValueColumn}]=@featureValue
                ";

            queryCommand.Parameters.Add(new SQLiteParameter("featureName", featureName));
            queryCommand.Parameters.Add(new SQLiteParameter("featureValue", featureValue));

            return queryCommand;
        };

        /// <inheritdoc cref="SetNullableValueCommandFactory"/>
        public override Func<string, bool?, DbCommand> SetNullableValueCommandFactory => (featureName, enabled) =>
        {
            int? featureValue = null;
            if (enabled.HasValue) featureValue = enabled.Value ? 1 : 0;
            var queryCommand = new SQLiteCommand();

            queryCommand.CommandText =
                $@"
INSERT INTO [{FeatureTableName}]
([{FeatureNameColumn}], [{FeatureValueColumn}])
VALUES (@featureName, @featureValue)
ON CONFLICT([{FeatureNameColumn}])
DO UPDATE SET [{FeatureValueColumn}]=@featureValue
                ";

            queryCommand.Parameters.Add(new SQLiteParameter("featureName", featureName));
            queryCommand.Parameters.Add(new SQLiteParameter("featureValue", featureValue));

            return queryCommand;
        };
    }
}
