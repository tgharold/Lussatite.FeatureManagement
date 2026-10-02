# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Lussatite.FeatureManagement is a light implementation of the main `Microsoft.FeatureManagement` interfaces (`IFeatureManager`, `IFeatureManagerSnapshot`). It reads feature values from `ISessionManager` implementations such as SQL tables, claims and configuration.

The repository publishes six NuGet packages.

## Project Structure

1. **src/Lussatite.FeatureManagement/** - `LussatiteFeatureManager` and `LussatiteLazyCacheFeatureManager`. Targets `netstandard2.0`.
2. **src/Lussatite.FeatureManagement.SessionManagers/** - `ISessionManager` implementations: claims, SQL, cached SQL and static answer. Targets `netstandard2.0`.
3. **src/Lussatite.FeatureManagement.SessionManagers.SqlClient/** - Microsoft SQL Server commands. Targets `netstandard2.0`.
4. **src/Lussatite.FeatureManagement.SessionManagers.SQLite/** - SQLite commands. Targets `netstandard2.0`.
5. **src/Lussatite.FeatureManagement.SessionManagers.Core/** - `IConfiguration` session manager. Targets `netcoreapp3.1`.
6. **src/Lussatite.FeatureManagement.SessionManagers.Framework/** - `ConfigurationManager` session manager. Targets `net48`.
7. **tests/** - Test projects: `Lussatite.FeatureManagement.Tests` (`netcoreapp3.1`), `NetCore31.Tests`, `Net6.Tests` (`net6.0`) and `Net48.Tests` (`net48`). `TestCommon.Standard` holds the test code that the other test projects share.

Each package has its own `README.md`, which is packed into that package. Keep the package READMEs and the root `README.md` consistent.

## Commands for Development

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

The SQL Server tests need a SQL Server instance. The local `appsettings.json` files point at `localhost,11433`. CI sets `DOTNET_ENVIRONMENT=GitHubActions` and uses LocalDB, which exists only on Windows. The `net48` tests also run only on Windows.

### Run Single Test
```bash
dotnet test tests/Lussatite.FeatureManagement.Tests/Lussatite.FeatureManagement.Tests.csproj --filter "FullyQualifiedName~LussatiteLazyCacheFeatureManagerTests"
```

## Development Notes

- `SqlSessionManagerSettings` builds table, schema and column names at run time. Identifiers are limited to letters, numbers and underscores, up to 64 characters. Never weaken that check, and never build identifiers from untrusted input.
- `LussatiteLazyCacheFeatureManager` must keep its own private cache instance. Do not share it with the application.

## Changelog

`CHANGELOG.md` follows the Keep a Changelog format. When you change the public API of any package, feature value behavior, the SQL schema or NuGet package contents, add an entry under `Unreleased` in the same change. Use the headings Added, Changed, Deprecated, Removed, Fixed and Security.

Mark breaking changes with **BREAKING** and describe what callers must change. Breaking changes include changes to the SQL table layout, changed error messages that callers might parse, and removed or renamed public members. Keep entries short. Build, test and CI-only changes need an entry only when they affect what ships.

See `RELEASE-PROCESS.md` for how the changelog fits into a release.
