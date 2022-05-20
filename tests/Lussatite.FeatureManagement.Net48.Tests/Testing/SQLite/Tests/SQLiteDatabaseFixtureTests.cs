using System.Data.SQLite;
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
            var connectionString = _dbFixture.GetConnectionString();

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                var queryCommand = conn.CreateCommand();
                queryCommand.CommandText =
                $@"SELECT * FROM {SQLiteDatabaseFixture.TableName};";
                var value = (string)queryCommand.ExecuteScalar();
                conn.Close();
            }
        }
    }
}
