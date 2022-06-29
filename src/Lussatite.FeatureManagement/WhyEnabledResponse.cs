using System.Collections.Generic;
using Microsoft.FeatureManagement;

namespace Lussatite.FeatureManagement
{
    /// <summary>Explain why a particular feature name comes back as enabled/disabled including
    /// which <see cref="ISessionManager"/> was responsible.  This response is designed
    /// to help advanced users diagnose issues.</summary>
    public class WhyEnabledResponse
    {
        public string FeatureName { get; set; }

        public bool Enabled { get; set; }

        /// <summary>The "name" of the ISessionManager which returned the first definitive
        /// response for the feature.</summary>
        public string SessionManagerName { get; set; }

        /// <summary>Details about the various session managers and what values they returned.</summary>
        public ICollection<WhyEnabledSessionManagerResponse> SessionManagers { get; }
            = new List<WhyEnabledSessionManagerResponse>();
    }

    public class WhyEnabledSessionManagerResponse
    {
        /// <summary>Name of the <see cref="ISessionManager"/> if it implements <see cref="IHasNameProperty"/>.
        /// Otherwise it will return the Type name.</summary>
        public string Name { get; set; }

        /// <summary>Feature value in this <see cref="ISessionManager"/>.</summary>
        public bool? Enabled { get; set; }
    }
}
