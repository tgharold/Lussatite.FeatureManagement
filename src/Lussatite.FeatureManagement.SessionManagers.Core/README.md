# Lussatite.FeatureManagement.SessionManagers.Core

Various read-only [ISessionManager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.isessionmanager) implementations which are compatible with .NET Core 3.1 and .NET 5+.  These implementations do not write back to the underlying value store.

## ISessionManager Implementations

While these `ISessionManager` implementations are written for `Lussatite.FeatureManagement`, they are compatible with the Microsoft `ISessionManager` interface and could be used with the Microsoft `FeatureManager` implementation.

### ConfigurationValueSessionManager

A read-only ISessionManager implementation that uses `IConfiguration` to obtain its values.

### SqlSessionManager

A read-only `ISessionManager` implementation that uses a user-provided `DbCommand` to obtain its values.

## Target

- .NET Core 3.1
- .NET 5, .NET 6, etc.

## References

- [Project README file](https://github.com/tgharold/Lussatite.FeatureManagement/blob/main/README.md)
- [Source code](https://github.com/tgharold/Lussatite.FeatureManagement/)

