namespace Lussatite.FeatureManagement.TestingCommon;

/// <summary>Feature flag definitions that are *not* registered in the feature manager instance.
/// All of these feature names/keys should return false?</summary>
public static class NotRegisteredTestFeatures
{
    public const string NotRegisteredAndNotInAppConfig = nameof(NotRegisteredAndNotInAppConfig);
    public const string NotRegisteredButInAppConfig = nameof(NotRegisteredButInAppConfig);
    public const string NotRegisteredButGarbageValueInAppConfig = nameof(NotRegisteredButGarbageValueInAppConfig);
}
