# Lussatite.FeatureManagement
Extensions and replacements for Microsoft.FeatureManagement

- Compatibility with [IFeatureManager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.ifeaturemanager) and [IFeatureManagerSnapshot](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.ifeaturemanagersnapshot).  You should be able to upgrade to the full [Microsoft.FeatureManagement NuGet package](https://www.nuget.org/packages/Microsoft.FeatureManagement/) later!
- A basic implementation that only uses [ConfigurationManager](https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationmanager) and which is simpler to wire up then the full [Microsoft.FeatureManagement](https://github.com/microsoft/FeatureManagement-Dotnet) package.  This can be useful in situations where there's no support for the [AddFeatureManagement()](https://docs.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement.servicecollectionextensions.addfeaturemanagement) method in your application. 


## References

- [Andrew Lock: Introducing Microsoft.FeatureManagement](https://andrewlock.net/introducing-the-microsoft-featuremanagement-library-adding-feature-flags-to-an-asp-net-core-app-part-1/)