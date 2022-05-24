using System;
using System.Data.Common;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers
{
    /// <summary>Settings class for the <see cref="SqlSessionManager"/> instance.</summary>
    public class SqlSessionManagerSettings
    {
        /// <summary>The database column which contains the feature name string.  This column's
        /// value will be checked during GetAsync() via a case-insensitive compare to ensure
        /// that the provided <see cref="DbCommand"/> returned the correct row.</summary>
        public string FeatureNameColumn { get; set; } = "FeatureName";

        /// <summary>The database column which contains the feature name value.  The underlying
        /// database column should be a nullable boolean (bool?) to allow for null/false/true.
        /// </summary>
        public string FeatureValueColumn { get; set; } = "Enabled";

        /// <summary>A method or lambda which returns a new <see cref="DbConnection"/> object
        /// which can be used to SELECT/INSERT/UPDATE from the database table.</summary>
        public Func<DbConnection> GetConnectionFactory { get; set; }

        /// <summary>A <see cref="DbCommand"/> query which must filter down to the single row
        /// matching the feature name string. </summary>
        public Func<string, DbCommand> GetValueCommandFactory { get; set; }

        /// <summary>An optional <see cref="DbCommand"/> query which must support being an INSERT/UPDATE (UPSERT)
        /// of the bool value into the database table. Note that you rarely want to use this in practice, unless
        /// your SQL table is already per-user or per-session. </summary>
        public Func<string, bool, DbCommand> SetValueCommandFactory { get; set; }

        /// <summary>An optional <see cref="DbCommand"/> query which must support being an INSERT/UPDATE (UPSERT)
        /// of the nullable bool value into the database table. </summary>
        public Func<string, bool?, DbCommand> SetNullableValueCommandFactory { get; set; }
    }
}
