# Lussatite.FeatureManagement

## Lussatite.FeatureManagement

A lightweight implementation of the Microsoft.FeatureManagement [IFeatureManager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.ifeaturemanager) interface.  The dependencies are a list of feature name strings and an ordered set of  [ISessionManager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.isessionmanager) instances.  When a feature flag value is requested, the `ISessionManager` instances will be polled in order until one returns a definitive answer (true/false).

- If the feature name was not registered via the constructor, the `LussatiteFeatureManager` instance will always return false.
- The order of `ISessionManagers` matters, each takes priority over later ones.
- The `ISessionManager` should return null if it does not have a definitive answer.  This allows layering.
- This `LussatiteFeatureManager` implementation will not write back to the `ISessionManager` instances via `SetAsync(string featureName, bool enabled)`.

## Lussatite.FeatureManagement.LazyCache

A lightweight implementation of the Microsoft.FeatureManagement [IFeatureManagerSnapshot](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.ifeaturemanagersnapshot) interface.  It uses [LazyCache](https://github.com/alastairtree/LazyCache) to track the first read value.  When a feature flag value is requested, the `ISessionManager` instances will be polled in order until one returns a definitive answer (true/false).

Because this is a caching implementation, the first value read by the `LussatiteLazyCacheFeatureManager` instance will continue to be the value returned for the life of the instance.  Even if a new value is registered in one of the `ISessionManager` instances.

- If the feature name was not registered via the constructor, the `LussatiteLazyCacheFeatureManager` instance will always return false.
- The order of `ISessionManagers` matters, each takes priority over later ones.
- The `ISessionManager` should return null if it does not have a definitive answer.  This allows layering.
- This `LussatiteLazyCacheFeatureManager` implementation will not write back to the `ISessionManager` instances via `SetAsync(string featureName, bool enabled)`.

### CacheAllFeatureValuesAsync()

The non-standard `Task CacheAllFeatureValuesAsync()` method can be used after construction to poll all `ISessionManager` instances for values for each feature name registered within the `LussatiteLazyCacheFeatureManager` instance.  This sets the values of all known features at the time of the call.

## Target

- .NET Standard 2.0

## References

- [Project README file](https://github.com/tgharold/Lussatite.FeatureManagement/blob/main/README.md)
- [Source code](https://github.com/tgharold/Lussatite.FeatureManagement/)
