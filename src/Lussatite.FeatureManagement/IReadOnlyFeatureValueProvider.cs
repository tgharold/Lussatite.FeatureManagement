using System.Threading.Tasks;
using Microsoft.FeatureManagement;

namespace Lussatite.FeatureManagement
{
    /// <summary>Defines a value provider that can fetch the value for a feature.
    /// This operates very similar to <see cref="ISessionManager"/> but without
    /// the ability to write the feature value back to the provider.</summary>
    public interface IReadOnlyFeatureValueProvider
    {
        /// <summary>Queries the value provider for the feature state, if any, for the given feature.</summary>
        /// <param name="featureName">The name of the feature.</param>
        /// <returns>The state of the feature if it is present, otherwise null.</returns>
        Task<bool?> GetAsync(string featureName);
    }
}