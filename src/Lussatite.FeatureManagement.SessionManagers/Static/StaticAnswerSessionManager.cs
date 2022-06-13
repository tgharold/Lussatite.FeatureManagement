using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;

// ReSharper disable once CheckNamespace
namespace Lussatite.FeatureManagement.SessionManagers
{
    /// <summary>The <see cref="StaticAnswerSessionManager"/> is an <see cref="ISessionManager"/>
    /// which returns a static pre-defined (during the constructor) answer for the feature.
    /// This is mostly useful when doing unit/integration testing and not as a production use case.
    /// </summary>
    public class StaticAnswerSessionManager : ISessionManager
    {
        private readonly IDictionary<string, bool?> _features;

        public StaticAnswerSessionManager(
            IDictionary<string, bool?> features
            )
        {
            _features = features;
        }

        /// <summary>This is a read-only <see cref="ISessionManager"/> so this method does nothing. </summary>
        public Task SetAsync(string featureName, bool enabled)
        {
            return Task.CompletedTask;
        }

        public Task<bool?> GetAsync(string featureName)
        {
            return _features.TryGetValue(featureName, out var result)
                ? Task.FromResult(result)
                : Task.FromResult((bool?)null);
        }
    }
}
