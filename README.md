# Lussatite.FeatureManagement

A light implementation of Microsoft.FeatureManagement interfaces.

- Compatibility with [IFeatureManager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.ifeaturemanager) and [IFeatureManagerSnapshot](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.ifeaturemanagersnapshot).  You should be able to upgrade to the full [Microsoft.FeatureManagement NuGet package](https://www.nuget.org/packages/Microsoft.FeatureManagement/) later.
- A basic implementation that is simpler to wire up then the full [Microsoft.FeatureManagement](https://github.com/microsoft/FeatureManagement-Dotnet) package.  This can be useful in situations where there's no support for the [AddFeatureManagement()](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.servicecollectionextensions.addfeaturemanagement) method in your application.
- A few read-only `ISessionManager` implementations.  Note that this is not quite what Microsoft had in mind for `ISessionManager`, but it serves as a good extension point for injecting values into the IFeatureManager result.

## Quickstart

1. Take a dependency on `Lussatite.FeatureManagement` and `Lussatite.FeatureManagement.LazyCache`.

2. Define a set of string constants for the names of your features.  Look at the various static TestFeatures.cs files for ideas.

3. Create a value provider by implementing [ISessionManager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.isessionmanager) or use one or more of the `Lussatite.FeatureManagement.SessionManagers.*` packages.  Note that Lussaite.FeatureManagement does **not** write back to the session manager (unlike the Microsoft.FeatureManagement implementation).

4. Wire-up the following in your IoC/DI container:

    - Session manager classes will need to be injected into the feature manager manually, since order does matter.
    - `LussatiteLazyCacheFeatureManager` as `IFeatureManagerSnapshot` (usually per-request / scoped lifetime).

5. Inject `IFeatureManagerSnapshot` into your controller or service classes.

6. `await _feature.IsEnabledAsync(string feature)` to obtain the status of the feature flag.

## Packages

### Lussatite.FeatureManagement:

Target: .NET Standard 2.0

- `LussatiteFeatureManager`: A basic implementation of `IFeatureManager`.
- This does **not** implement the `Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)` method.
- This does **not** make any calls back to the ISessionManager `Task SetAsync(string featureName, bool enabled)` method.

### Lussatite.FeatureManagement.LazyCache:

Target: .NET Standard 2.0

- `LussatiteLazyCacheFeatureManager`: A caching `IFeatureManagerSnapshot` implementation that uses `LazyCache`.
- This does **not** implement the `Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)` method.
- This does **not** make any calls back to the ISessionManager `Task SetAsync(string featureName, bool enabled)` method.

### Lussatite.FeatureManagement.SessionManagers.Configuration:

Target: .NET Core 3.1 (compatible with .NET 5+)

- `ConfigurationValueSessionManager`: A read-only ISessionManager implementation that uses `IConfiguration` to obtain its values.

### Lussatite.FeatureManagement.SessionManagers.Framework:

Target: .NET Framework 4.8

All .NET Framework session managers are contained in a single package (for now).

- `ConfigurationValueSessionManager`: A read-only ISessionManager implementation that uses the static class `ConfigurationManager` to obtain its values.

## Build Status

![Build-Test](https://github.com/tgharold/Lussatite.FeatureManagement/actions/workflows/dotnet.yml/badge.svg)

## References

- [Andrew Lock: Introducing Microsoft.FeatureManagement](https://andrewlock.net/introducing-the-microsoft-featuremanagement-library-adding-feature-flags-to-an-asp-net-core-app-part-1/)
