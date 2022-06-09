using System.Data.SQLite;
using TestCommon.Standard.SQLite;
using Xunit;

namespace Lussatite.FeatureManagement.NetCore31.Tests.Testing.SQLite.Tests
{
    [Collection(nameof(SQLiteDatabaseCollection))]
    public class SQLiteDatabaseFixtureTests
    {
        private readonly SQLiteDatabaseFixture _dbFixture;

        public SQLiteDatabaseFixtureTests(SQLiteDatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
        }

        [Fact]
        public void Can_execute_select_star_query_against_TableName()
        {
            var connectionString = _dbFixture.SqlSessionManagerSettings.ConnectionString;
            var settings = _dbFixture.SqlSessionManagerSettings;

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                var queryCommand = conn.CreateCommand();
                queryCommand.CommandText =
                    $@"SELECT * FROM {settings.FeatureTableName};";
                var value = (string)queryCommand.ExecuteScalar();
                conn.Close();
            }
        }
    }
}
