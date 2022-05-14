using System.Data.Common;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers.Framework
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
    }
}
