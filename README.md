# Lussatite.FeatureManagement
Extensions and replacements for Microsoft.FeatureManagement

- Compatibility with [IFeatureManager]() and [IFeatureManagerSnapshot]().  You should be able to upgrade to the full [Microsoft.FeatureManagement]() package later!
- A basic implementation that only uses [ConfigurationManager]() and which is simpler to wire up then the full [Microsoft.FeatureManagement]() package.  This can be useful in situations where there's no support for the [AddFeatureManagement()]() method in your application. 

