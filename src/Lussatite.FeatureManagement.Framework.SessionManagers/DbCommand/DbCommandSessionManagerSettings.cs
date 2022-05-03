using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement.Framework
{
    public class DbCommandSessionManagerSettings
    {
        public string SchemaName { get; set; } = "dbo";
        public string TableName { get; set; } = "FeatureManagement";
        public string FeatureNameColumn { get; set; } = "FeatureName";
        public string FeatureValueColumn { get; set; } = "Enabled";
    }
}
