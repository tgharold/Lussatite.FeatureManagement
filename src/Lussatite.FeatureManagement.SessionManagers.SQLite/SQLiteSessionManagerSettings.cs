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
        public override DbConnection GetConnectionFactory()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new Exception(
                    $"Missing {nameof(ConnectionString)} value in {nameof(GetConnectionFactory)}()."
                    );
            return new SQLiteConnection(ConnectionString);
        }

        /// <inheritdoc cref="CreateDatabaseTableFactory"/>
        public override DbCommand CreateDatabaseTableFactory()
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
        }

        /// <inheritdoc cref="GetValueCommandFactory"/>
        public override DbCommand GetValueCommandFactory(string featureName)
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
        }

        /// <inheritdoc cref="SetValueCommandFactory"/>
        public override DbCommand SetValueCommandFactory(string featureName, bool enabled)
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
        }

        /// <inheritdoc cref="SetNullableValueCommandFactory"/>
        public override DbCommand SetNullableValueCommandFactory(string featureName, bool? enabled)
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
        }
    }
}
