using System.Data.SQLite;
using System.Threading.Tasks;
using Lussatite.FeatureManagement.NetCore31.Tests.Testing.SQLite;
using Lussatite.FeatureManagement.SessionManagers;
using Xunit;

namespace Lussatite.FeatureManagement.NetCore31.Tests.SessionManagers.Sql
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
                getValueCommandFactory: s => _dbFixture.CreateGetValueCommand(s),
                setValueCommandFactory: (s, e) => _dbFixture.CreateSetValueCommand(s, e),
                setNullableValueCommandFactory: (s, e) => _dbFixture.CreateSetNullableValueCommand(s, e)
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
        [InlineData(null, "NetCore31_A125a_FeatureSetToNull", null)]
        [InlineData(false, "NetCore31_A125b_FeatureSetToFalse", 0)]
        [InlineData(true, "NetCore31_A125c_FeatureSetToTrue", 1)]
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

        [Theory]
        [InlineData(null, "NetCore31_A129x_FeatureSetToNull", null)]
        [InlineData(false, "NetCore31_A129y_FeatureSetToFalse", false)]
        [InlineData(true, "NetCore31_A129z_FeatureSetToTrue", true)]
        public async Task Return_expected_for_SetNullableValue(
            bool? expected,
            string featureName,
            bool? enabled
            )
        {
            var sut = CreateSut();
            await sut.SetNullableAsync(featureName, enabled);

            var featureTableValues = await _dbFixture.GetAllData();
            Assert.NotEmpty(featureTableValues);

            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, "NetCore31_A139jx_FeatureSetToNull", null)]
        [InlineData(false, "NetCore31_A139ky_FeatureSetToFalse", false)]
        [InlineData(true, "NetCore31_A139lz_FeatureSetToTrue", true)]
        public async Task Return_expected_for_SetValue(
            bool? expected,
            string featureName,
            bool? enabled
            )
        {
            var sut = CreateSut();
            if (enabled.HasValue) await sut.SetAsync(featureName, enabled.Value);
            else await sut.SetNullableAsync(featureName, null);

            var featureTableValues = await _dbFixture.GetAllData();
            Assert.NotEmpty(featureTableValues);

            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }
    }
}