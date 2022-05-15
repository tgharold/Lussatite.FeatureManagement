# Lussatite.FeatureManagement

A lightweight implementation of the Microsoft.FeatureManagement [IFeatureManager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.ifeaturemanager) interface.  The dependencies are a list of feature name strings and an ordered set of  [ISessionManager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.isessionmanager) instances.  When a feature flag value is requested, the `ISessionManager` instances will be polled in order until one returns a definitive answer (true/false).

- If the feature name was not registered via the constructor, the `LussatiteFeatureManager` instance will always return false.
- The order of `ISessionManagers` matters, each takes priority over later ones.
- The `ISessionManager` should return null if it does not have a definitive answer.  This allows layering.
- This `LussatiteFeatureManager` implementation will not write back to the `ISessionManager` instances via `SetAsync(string featureName, bool enabled)`.

## Target

- .NET Standard 2.0

## References

- [Project README file](https://github.com/tgharold/Lussatite.FeatureManagement/blob/main/README.md)
- [Source code](https://github.com/tgharold/Lussatite.FeatureManagement/)
