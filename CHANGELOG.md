# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/). The packages follow [Semantic Versioning](https://semver.org/).

## Unreleased

## 1.6.5 - 2026-10-02

### Security

- The SqlClient package now depends on `System.Data.SqlClient` 4.8.6, up from 4.8.3. This picks up fixes for an information disclosure issue and a security feature bypass. If your project pins an older `System.Data.SqlClient`, raise the pin to 4.8.6 or later to avoid a downgrade error (NU1109).

## 1.6.4 - 2026-10-01

### Fixed

- Each package now places its README at the package root by name. The old `PackagePath` could leave nuget.org showing a missing-README warning.

## 1.6.3 - 2022-06-29

### Added

- `LussatiteFeatureManager.WhyIsEnabledAsync()` returns a `WhyEnabledResponse`. It shows which `ISessionManager` answered for a feature and what each session manager returned. Use it to debug feature values.
- `IHasNameProperty`. `SqlSessionManager`, `StaticAnswerSessionManager` and `ClaimsPrincipalSessionManager` implement it, so `WhyIsEnabledAsync()` can report them by name.

## 1.6.2 - 2022-06-28

### Fixed

- `LussatiteLazyCacheFeatureManager` now uses its own private cache. Before, `LazyCache` fell back to a static default cache provider that other code in the application could share.

## 1.6.1 - 2022-06-28

### Added

- All six packages now set `IncludeSymbols`, so releases include symbol packages.

## 1.6.0 - 2022-06-15

### Added

- `SQLServerPerGuidSessionManagerSettings` defines a SQL Server table where feature values are stored per GUID. This allows per-user, per-group and per-role features.
- `StaticAnswerSessionManager` always returns the same answer for a feature. Use it in tests.

### Changed

- **BREAKING:** The table defined by the `SqlSessionManager` classes now has `Created` and `Modified` columns. They hold the UTC instant of creation and of the last update. There is no automatic migration, so add the columns to your existing feature value table by hand.
- `CachedSqlSessionManager` uses a global LazyCache `IAppCache` when you provide one. This lets per-GUID feature values stay cached across requests when you build it with `SQLServerPerGuidSessionManagerSettings`.

## 1.5.0 - 2022-06-09

### Changed

- **BREAKING:** The SQL settings classes were [reworked](https://github.com/tgharold/Lussatite.FeatureManagement/pull/40). Other session managers, such as the `IConfiguration` one, are not affected.
  - The settings classes no longer use `Func<T>`. These are now abstract methods on the SQL settings class.
  - The cached SQL session manager settings moved to a separate class. Inheritance caused problems, such as copying values in the constructor while some properties were virtual.
  - The schema, table and column names are now set at run time. This adds a risk of SQL injection. Identifier names may contain only letters, numbers and underscores, up to 64 characters. Pass only string constants when you create the settings object.

### Added

- Default SQLite and Microsoft SQL Server implementations of the database commands.
