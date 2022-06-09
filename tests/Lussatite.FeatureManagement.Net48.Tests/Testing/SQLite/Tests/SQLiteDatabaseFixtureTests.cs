using System.Data.SQLite;
using Lussatite.FeatureManagement.SessionManagers;
using TestCommon.Standard.SQLite;
using Xunit;

namespace Lussatite.FeatureManagement.Net48.Tests.Testing.SQLite.Tests
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
