using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers.Framework
{
    /// <summary>Settings class for the <see cref="DbCommandSessionManager"/> instance.</summary>
    public class DbCommandSessionManagerSettings
    {
        public string TableName { get; set; } = "dbo.FeatureManagement";

        public string FeatureNameColumn { get; set; } = "FeatureName";

        public string FeatureValueColumn { get; set; } = "Enabled";
    }
}
