# Lussatite.FeatureManagement.SessionManagers

Various read-only [ISessionManager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.isessionmanager) implementations which are compatible with .NET Standard 2.0.  These implementations do not write back to the underlying value store.

## ISessionManager Implementations

While these `ISessionManager` implementations are written for `Lussatite.FeatureManagement`, they are compatible with the Microsoft `ISessionManager` interface and could be used with the Microsoft `FeatureManager` implementation.

### ClaimsPrincipalSessionManager

A read-only `ISessionManager` implementation that examines `ClaimsPrincipal` claims of type "feature_flag" to obtain the feature value.  The value of the claim is either the feature name to indicate true, or the feature name prefixed with an exclamation point ("!") to indicate false.

### SqlSessionManager

An `ISessionManager` implementation that uses user-provided `DbCommand` objects to obtain/update its values. Write-back is optional for the `SetValue()` call.

In order to construct the `SqlSessionManager`, you need to provide it with a `SqlSessionManagerSettings` object.  See the SessionManagers.SqlClient or SessionManagers.SQLite for default setting objects which already have the necessary SQL statements.

```C#
var settings = new SQLServerSessionManagerSettings
{
    ConnectionString = "some connection string",
    EnableSetValueCommand = true,
};
var sqlSessionManager = new SqlSessionManager(settings);
// inject the sqlSessionManager into the LussatiteLazyCacheFeatureManager constructor
```

### CachedSqlSessionManager

A cached`ISessionManager` implementation that uses a user-provided `DbCommand` to obtain its values.  The results for a particular feature flag name will be cached for 60 seconds.  This object is configured using a `CachedSqlSessionManagerSettings` object.

It uses LazyCache and you can pass in an `IAppCache` object to the constructor if you want it to share an application-wide cache. Be warned that if you use a shared-cache, that per-user / per-session feature values are not supported out of the box.  For per-user / per-session needs, make sure `CachedSqlSessionManager` is using a separate `IAppCache` for each user/session.

## Target

- .NET Standard 2.0

## References

- [Project README file](https://github.com/tgharold/Lussatite.FeatureManagement/blob/main/README.md)
- [Source code](https://github.com/tgharold/Lussatite.FeatureManagement/)

