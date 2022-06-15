# Lussatite.FeatureManagement.SessionManagers.SqlClient

Classes which help interface Microsoft SQL Server with a SqlSessionManager.

## SQLServerSessionManagerSettings

Default implementations of the SQL commands needed to interface Microsoft SQL Server with a SqlSessionManager.

## SQLServerPerGuidSessionManagerSettings

Default implementations of the SQL commands needed to interface Microsoft SQL Server with a SqlSessionManager.  This implementation adds the concept of per-GUID feature settings by adding a UserGuid column to the database table.  The GUID might be related to a user, a role, a group, or whatever the needs are.

Note that because of how `ISessionManager` works, you must register/create a new settings object and associated SqlSessionManager / CachedSqlSessionManager for each GUID that you are tracking.  That will generally mean that the SQL `ISessionManager` must be registered as scoped.

If you use the CachedSqlSessionManager, you can pass in an application's global (singleton) LazyCache IAppCache object to get caching across requests and better performance.  The cache keys used to store the GUID's feature value include both the feature name and GUID and the underlying schema/table name.

## Target

- .NET Standard 2.0

## References

- [Project README file](https://github.com/tgharold/Lussatite.FeatureManagement/blob/main/README.md)
- [Source code](https://github.com/tgharold/Lussatite.FeatureManagement/)

