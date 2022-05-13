
// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers.Framework
{
    /// <summary>Settings class for the <see cref="DataReaderSessionManager"/> instance.</summary>
    public class DataReaderSessionManagerSettings
    {
        public string FeatureNameColumn { get; set; } = "FeatureName";
        public string FeatureValueColumn { get; set; } = "Enabled";
    }
}
