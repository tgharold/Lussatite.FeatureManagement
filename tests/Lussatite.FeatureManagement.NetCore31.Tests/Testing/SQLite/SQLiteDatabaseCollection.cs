using Xunit;

namespace Lussatite.FeatureManagement.NetCore31.Tests.Testing.SQLite
{
    [CollectionDefinition(nameof(SQLiteDatabaseCollection))]
    public class SQLiteDatabaseCollection : ICollectionFixture<SQLiteDatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
