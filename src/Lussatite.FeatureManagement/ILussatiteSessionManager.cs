using System.Threading.Tasks;
using Microsoft.FeatureManagement;

namespace Lussatite.FeatureManagement
{
    public interface ILussatiteSessionManager : ISessionManager
    {
        /// <summary>
        /// <para>Set the nullable state of a feature to be used for a session.</para>
        /// <para>This differs from the base <see cref="ISessionManager.SetAsync"/> method in that
        /// it takes a nullable boolean and is not called by the Microsoft <see cref="IFeatureManager"/>
        /// or <see cref="IFeatureManagerSnapshot"/> implementations.  The Microsoft implementations
        /// assume that you always want to write back the value at the end of the
        /// <see cref="IFeatureManager.IsEnabledAsync"/> method after calculating the various
        /// answers provided by <see cref="FeatureDefinition"/>.</para>
        /// <para>In my experience, setting the value in the session manager should be done prior
        /// to the call to <see cref="IFeatureManager.IsEnabledAsync"/>.</para>
        /// </summary>
        /// <param name="featureName">The name of the feature.</param>
        /// <param name="enabled">The nullable state of the feature.</param>
        Task SetNullableAsync(string featureName, bool? enabled);
    }
}
