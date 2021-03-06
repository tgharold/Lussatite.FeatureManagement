using System;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LazyCache;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers
{
    /// <summary>Settings class for the <see cref="SqlSessionManager"/> instance.</summary>
    public abstract class SqlSessionManagerSettings
    {
        private string _featureSchemaName;
        private string _featureTableName;
        private string _featureNameColumn;
        private string _featureValueColumn;
        private string _featureCreatedColumn;
        private string _featureModifiedColumn;

        private const string DefaultSchemaName = "dbo"; // not all SQL providers have the concept of schema
        protected virtual string DefaultTableName => "FeatureManagement";
        private const string DefaultNameColumn = "FeatureName";
        private const string DefaultValueColumn = "Enabled";
        private const string DefaultCreatedColumn = "Created";
        private const string DefaultModifiedColumn = "Modified";

        /// <summary>A restrictive regex for identifiers like schema/table/column names.
        /// Even with this regex, you should be passing in string constants and not
        /// something variable (and definitely not user input).  This is a modest attempt
        /// to prevent SQL injection.</summary>
        private readonly Regex _restrictiveRegex = new Regex(@"^[\p{L}_][\p{L}\p{N}_]{0,63}$");

        public virtual bool IsValidSchemaName(string schemaName) => _restrictiveRegex.IsMatch(schemaName);
        public virtual bool IsValidTableName(string tableName) => _restrictiveRegex.IsMatch(tableName);
        public virtual bool IsValidColumnName(string columnName) => _restrictiveRegex.IsMatch(columnName);

        protected virtual string CacheKeyPrefix => $"Lussatite.FeatureManagement-{nameof(SqlSessionManagerSettings)}";

        /// <summary>Calculate the cache key to be used when storing data in an application's global
        /// <see cref="IAppCache"/>.  By including the schema/table name, we generate a unique
        /// key for the feature name; even if there are multiple feature value tables in use.
        /// </summary>
        public virtual string CacheKey(string featureName) =>
            $"{CacheKeyPrefix}-{FeatureSchemaName}-{FeatureTableName}-{featureName}";

        /// <summary>The database schema name which holds the feature values table.  Not
        /// all providers have the concept of a schema, so this property is optional
        /// and not always used by sub-classes.</summary>
        public string FeatureSchemaName
        {
            get => string.IsNullOrEmpty(_featureSchemaName) ? DefaultSchemaName : _featureSchemaName;
            set
            {
                if (!IsValidSchemaName(value))
                    throw new Exception($"{nameof(FeatureSchemaName)} set to invalid value.")
                    {
                        Data = { ["value"] = value }
                    };

                _featureSchemaName = value;
            }
        }

        /// <summary>The database table name which contains the feature values.</summary>
        public string FeatureTableName
        {
            get => string.IsNullOrEmpty(_featureTableName) ? DefaultTableName : _featureTableName;
            set
            {
                if (!IsValidTableName(value))
                    throw new Exception($"{nameof(FeatureTableName)} set to invalid value.")
                    {
                        Data = { ["value"] = value }
                    };

                _featureTableName = value;
            }
        }

        /// <summary>The database column which contains the feature name string.  This column's
        /// value will be checked during GetAsync() via a case-insensitive compare to ensure
        /// that the provided <see cref="DbCommand"/> returned the correct row.</summary>
        public string FeatureNameColumn
        {
            get => string.IsNullOrEmpty(_featureNameColumn) ? DefaultNameColumn : _featureNameColumn;
            set
            {
                if (!IsValidColumnName(value))
                    throw new Exception($"{nameof(FeatureNameColumn)} set to invalid value.")
                    {
                        Data = { ["value"] = value }
                    };

                _featureNameColumn = value;
            }
        }

        /// <summary>The database column which contains the feature name value.  The underlying
        /// database column should be a nullable boolean (bool?) to allow for null/false/true.
        /// Alternately, it could be some column type that allows for three possible values.
        /// </summary>
        public string FeatureValueColumn
        {
            get => string.IsNullOrEmpty(_featureValueColumn) ? DefaultValueColumn : _featureValueColumn;
            set
            {
                if (!IsValidColumnName(value))
                    throw new Exception($"{nameof(FeatureValueColumn)} set to invalid value.")
                    {
                        Data = { ["value"] = value }
                    };

                _featureValueColumn = value;
            }
        }

        /// <summary>The database column which contains the created date/time (UTC).</summary>
        public string FeatureCreatedColumn
        {
            get => string.IsNullOrEmpty(_featureCreatedColumn) ? DefaultCreatedColumn : _featureCreatedColumn;
            set
            {
                if (!IsValidColumnName(value))
                    throw new Exception($"{nameof(FeatureCreatedColumn)} set to invalid value.")
                    {
                        Data = { ["value"] = value }
                    };

                _featureCreatedColumn = value;
            }
        }

        /// <summary>The database column which contains the modified date/time (UTC).</summary>
        public string FeatureModifiedColumn
        {
            get => string.IsNullOrEmpty(_featureModifiedColumn) ? DefaultModifiedColumn : _featureModifiedColumn;
            set
            {
                if (!IsValidColumnName(value))
                    throw new Exception($"{nameof(FeatureModifiedColumn)} set to invalid value.")
                    {
                        Data = { ["value"] = value }
                    };

                _featureModifiedColumn = value;
            }
        }

        /// <summary>The SQL connection string used by <see cref="GetConnectionFactory"/>
        /// to build the <see cref="DbConnection"/> object which can be used to execute
        /// SELECT/INSERT/UPDATE queries against the database table.  It's strongly
        /// recommended that this account has minimal permissions.</summary>
        public string ConnectionString { get; set; }

        /// <summary>Whether the SetValueDbCommand will return a DbCommand.  Under the Microsoft
        /// FeatureManager implementation, it writes back via SetValue() at the end of the method.
        /// Which may or may not be the desired behavior for your SqlSessionManager object.
        /// </summary>
        public bool EnableSetValueCommand { get; set; } = false;

        /// <summary>A method or lambda which returns a new <see cref="DbConnection"/> object
        /// which can be used to SELECT/INSERT/UPDATE from the database table.</summary>
        public abstract DbConnection GetConnectionFactory();

        /// <summary>A method which can create the feature values database table
        /// if it does not exist. The connection string requires CREATE SCHEMA/TABLE permissions.
        /// </summary>
        public abstract void CreateDatabaseTable(string tableCreateConnectionString);

        /// <summary>Async implementation of <see cref="CreateDatabaseTable"/>.</summary>
        public abstract Task CreateDatabaseTableAsync(string tableCreateConnectionString);

        /// <summary>A <see cref="DbCommand"/> query which must filter down to the single row
        /// matching the feature name string. </summary>
        public abstract DbCommand GetValueDbCommand(string featureName);

        /// <summary>An optional <see cref="DbCommand"/> query which must support being an INSERT/UPDATE (UPSERT)
        /// of the bool value into the database table. Note that you rarely want to use this in practice, unless
        /// your SQL table is already per-user or per-session. </summary>
        public abstract DbCommand SetValueDbCommand(string featureName, bool enabled);

        /// <summary>A  <see cref="DbCommand"/> query which must support being an INSERT/UPDATE (UPSERT)
        /// of the nullable bool value into the database table. </summary>
        public abstract DbCommand SetNullableValueDbCommand(string featureName, bool? enabled);
    }
}
