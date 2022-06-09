using TestCommon.Standard.MicrosoftSQLServer;
using Xunit;

namespace Lussatite.FeatureManagement.Net6.Tests.Testing.SQLServer
{
    [CollectionDefinition(nameof(SQLServerDatabaseCollection))]
    public class SQLServerDatabaseCollection : ICollectionFixture<SqlServerDatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
