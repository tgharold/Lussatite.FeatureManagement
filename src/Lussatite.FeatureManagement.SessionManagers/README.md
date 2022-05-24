# Lussatite.FeatureManagement.SessionManagers

Various read-only [ISessionManager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.isessionmanager) implementations which are compatible with .NET Standard 2.0.  These implementations do not write back to the underlying value store.

## ISessionManager Implementations

While these `ISessionManager` implementations are written for `Lussatite.FeatureManagement`, they are compatible with the Microsoft `ISessionManager` interface and could be used with the Microsoft `FeatureManager` implementation.

### ClaimsPrincipalSessionManager

A read-only `ISessionManager` implementation that examines `ClaimsPrincipal` claims of type "feature_flag" to obtain the feature value.  The value of the claim is either the feature name to indicate true, or the feature name prefixed with an exclamation point ("!") to indicate false.

### SqlSessionManager

An `ISessionManager` implementation that uses a user-provided `DbCommand` to obtain its values. It also supports write-back if configured to do so.

### CachedSqlSessionManager

A cached`ISessionManager` implementation that uses a user-provided `DbCommand` to obtain its values.  The results for a particular feature flag name will be cached for a period.  The default cache duration is 30 seconds. It also supports write-back if configured to do so.

## Target

- .NET Standard 2.0

## References

- [Project README file](https://github.com/tgharold/Lussatite.FeatureManagement/blob/main/README.md)
- [Source code](https://github.com/tgharold/Lussatite.FeatureManagement/)

