using Lussatite.FeatureManagement.SessionManagers.Framework.Tests.Testing.SQLite;
using System.Data.SQLite;
using System.Threading.Tasks;
using Xunit;

namespace Lussatite.FeatureManagement.SessionManagers.Framework.Tests.Sql
{
    [Collection(nameof(SQLiteDatabaseCollection))]
    public class SqlSessionManagerTests
    {
        private readonly SQLiteDatabaseFixture _dbFixture;

        public SqlSessionManagerTests(SQLiteDatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
        }

        private SqlSessionManager CreateSut()
        {
            return new SqlSessionManager(
                settings: _dbFixture.SqlSessionManagerSettings,
                getValueCommandFactory: s => _dbFixture.CreateGetValueCommand(s)
            );
        }

        [Fact]
        public async Task Return_null_for_nonexistent_key()
        {
            var sut = CreateSut();
            var result = await sut.GetAsync("someRandomFeatureNonExistentName");
            Assert.Null(result);
        }

        [Theory]
        [InlineData(null, "A125a_FeatureSetToNull", null)]
        [InlineData(false, "A125b_FeatureSetToFalse", 0)]
        [InlineData(true, "A125c_FeatureSetToTrue", 1)]
        public async Task Return_expected_for_inserted_key_value(
            bool? expected,
            string featureName,
            int? insertValue
            )
        {
            using (var conn = new SQLiteConnection(_dbFixture.GetConnectionString()))
            {
                conn.Open();

                var updateCommand = conn.CreateCommand();
                updateCommand.CommandText =
                $@"
                    INSERT INTO {SQLiteDatabaseFixture.TableName}
                    ({SQLiteDatabaseFixture.NameColumn}, {SQLiteDatabaseFixture.ValueColumn})
                    VALUES (@featureName, @featureValue)
                    ON CONFLICT({SQLiteDatabaseFixture.NameColumn})
                    DO UPDATE SET {SQLiteDatabaseFixture.ValueColumn}=@featureValue
                ";
                updateCommand.Parameters.Add(new SQLiteParameter("featureName", featureName));
                updateCommand.Parameters.Add(new SQLiteParameter("featureValue", insertValue));
                updateCommand.ExecuteNonQuery();

                conn.Close();
            }

            var featureTableValues = await _dbFixture.GetAllData();
            Assert.NotEmpty(featureTableValues);

            var sut = CreateSut();
            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }
    }
}
