# Security Policy

## Supported versions

Security fixes go into the latest release only. If you use an older version, upgrade to the latest release first.

The repository publishes six packages. Each package follows the same rule.

| Version | Supported |
|---------|-----------|
| Latest release of each package on [nuget.org](https://www.nuget.org/packages/Lussatite.FeatureManagement) | Yes |
| Older releases | No |

## Report a vulnerability

Do not open a public issue for a security problem.

Use GitHub's private reporting instead:

1. Open the [Security tab](https://github.com/tgharold/Lussatite.FeatureManagement/security) of this repository.
2. Select **Report a vulnerability**.
3. Describe the problem and include a minimal code sample that reproduces it.

Please include:

- The package name, package version and target framework (for example, `net6.0` or `net48`).
- What you expected to happen and what happened instead.
- The impact, if you know it.

This is a one-person project. I aim to reply within 30 days. I will tell you whether I accept the report and agree on a disclosure date with you. I will credit you in the release notes unless you ask me not to.

## What counts as a vulnerability

These packages read feature flag values from sources such as SQL databases, claims and configuration. Applications often use the answer to decide who may see or do something. These are in scope:

- **SQL injection.** The SQL session manager settings build table, schema and column names at run time. Identifier names are limited to letters, numbers and underscores, up to 64 characters. A way to get other characters into a query is in scope.
- **Wrong answer for the wrong session.** A cache, such as `LussatiteLazyCacheFeatureManager` or `CachedSqlSessionManager`, returns a feature value that belongs to another user, group or request.

These are not in scope:

- Bugs that have no security impact. Open a normal [issue](https://github.com/tgharold/Lussatite.FeatureManagement/issues) for those.
- Identifier names that your application builds from untrusted input. The settings classes document that callers must pass string constants.
- Vulnerabilities in your own `ISessionManager` implementations, in your database or in .NET itself. Report those to the code's owner.
- Exceptions shown to end users. This is backend code. The calling application must catch exceptions and decide what to display.
- Vulnerabilities in dependencies. Dependabot tracks those for this repository.

## Disclosure

After a fix ships, I publish a [GitHub security advisory](https://github.com/tgharold/Lussatite.FeatureManagement/security/advisories) and add an entry to the [changelog](CHANGELOG.md).
