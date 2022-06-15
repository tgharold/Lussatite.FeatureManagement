# Lussatite.FeatureManagement Changes

For a list of breaking changes, see [BREAKING-CHANGES](BREAKING-CHANGES.md).

## v1.6.0

- The SQL database table defined in the `SqlSessionManager` classes now have `Created`/`Modified` columns to track the UTC instant of creation / last-updated.  There is no automatic migration process; so users will need to manually add those columns to their existing feature value table.
- Add `SQLServerPerGuidSessionManagerSettings` which defines a SQL Server table where feature values are stored on a per-GUID basis.  This allows per-user / per-group / per-role features.
- `CachedSqlSessionManager` will now use a global LazyCache `IAppCache` if provided.  This will be useful for cases where it is constructed using `SQLServerPerGuidSessionManagerSettings` so that per-GUID feature values are cached across multiple requests.
- Addition of `StaticAnswerSessionManager` which is useful for testing scenarios where you need to set the feature to have a static answer.


