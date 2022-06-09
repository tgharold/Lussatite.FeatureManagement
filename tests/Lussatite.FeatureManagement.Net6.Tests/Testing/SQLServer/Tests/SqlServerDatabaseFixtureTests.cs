using System.Data.SQLite;
using Lussatite.FeatureManagement.SessionManagers;
using TestCommon.Standard.MicrosoftSQLServer;
using Xunit;

namespace Lussatite.FeatureManagement.Net6.Tests.Testing.SQLServer.Tests
{
    [Collection(nameof(SQLServerDatabaseCollection))]
    public class SqlServerDatabaseFixtureTests
    {
        private readonly SqlServerDatabaseFixture _dbFixture;

        public SqlServerDatabaseFixtureTests(SqlServerDatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
        }

        [Fact]
        public void Can_execute_select_star_query_against_TableName()
        {
            var connectionString = _dbFixture.SqlSessionManagerSettings.ConnectionString;

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                var queryCommand = conn.CreateCommand();
                queryCommand.CommandText =
                $@"SELECT * FROM {SqlSessionManagerSettings.DefaultTableName};";
                var value = (string)queryCommand.ExecuteScalar();
                conn.Close();
            }
        }
    }
}
