# Lussatite.FeatureManagement.SessionManagers.Framework

Various read-only [ISessionManager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.isessionmanager) implementations which are compatible with .NET Framework 4.8.  These implementations do not write back to the underlying value store.

## ISessionManager Implementations

While these `ISessionManager` implementations are written for `Lussatite.FeatureManagement`, they are compatible with the Microsoft `ISessionManager` interface and could be used with the Microsoft `FeatureManager` implementation.

### ConfigurationValueSessionManager

A read-only `ISessionManager` implementation that uses the .NET Framework static class `ConfigurationManager` to obtain its values.

## Target

- .NET Framework 4.8

## References

- [Project README file](https://github.com/tgharold/Lussatite.FeatureManagement/blob/main/README.md)
- [Source code](https://github.com/tgharold/Lussatite.FeatureManagement/)
