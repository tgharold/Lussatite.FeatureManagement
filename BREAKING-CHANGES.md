# Lussatite.FeatureManagement Breaking Changes

This is only a list of breaking changes.

## v1.6.0

The SQL database table defined in the `SqlSessionManager` classes now have `Created`/`Modified` columns to track the UTC instant of creation / last-updated.  There is no automatic migration process; so users will need to manually add those columns to their existing feature value table.

## v1.5.0

This version [reworked a lot of things](https://github.com/tgharold/Lussatite.FeatureManagement/pull/40) around `SqlSessionManagerSettings` is constructed.  The other session manager implementations (such as `IConfiguration`) were not impacted.

1. Rework how the settings classes work to do less magic around `Func<T>`. Instead, these are now abstract methods on the SQL settings class.
2. Split the cached SQL session manager settings out to a separate class. Inheritance was causing issues. Especially the attempt to copy values from A to B inside the ctor when some of the properties were virtual.
3. Add SQLite / MS SQL default implementations of the necessary database commands.
4. Move to dynamic definition of the schema/table/column names. This introduces the risk of SQL Injection; so be cautious. The identifier names are limited to letters/numbers and underscores with a maximum length of 64 characters. Library users should take care to only pass in string constants when creating the settings object.
