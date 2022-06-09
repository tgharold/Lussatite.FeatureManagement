using System.Data.SqlClient;
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
            var settings = _dbFixture.SqlSessionManagerSettings;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var queryCommand = conn.CreateCommand();
                queryCommand.CommandText =
                    $@"SELECT * FROM [{settings.FeatureSchemaName}].[{settings.FeatureTableName}];";
                var value = (string)queryCommand.ExecuteScalar();
                conn.Close();
            }
        }
    }
}
