using System.Threading.Tasks;
using LazyCache;
using Lussatite.FeatureManagement.Net6.Tests.Testing.SQLite;
using Lussatite.FeatureManagement.SessionManagers;
using TestCommon.Standard;
using TestCommon.Standard.SQLite;
using Xunit;

namespace Lussatite.FeatureManagement.Net6.Tests.SessionManagers.Sql
{
    [Collection(nameof(SQLiteDatabaseCollection))]
    public class CachedSqlSessionManagerSQLiteTests
    {
        private readonly SQLiteDatabaseFixture _dbFixture;
        private readonly IAppCache _appCache = new CachingService();

        public CachedSqlSessionManagerSQLiteTests(SQLiteDatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
        }

        private CachedSqlSessionManager CreateSut()
        {
            var settings = _dbFixture.SqlSessionManagerSettings;

            return new CachedSqlSessionManager(
                cache: _appCache,
                settings: settings
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
        [InlineData(null, "Net6_A125a_FeatureSetToNull", null)]
        [InlineData(false, "Net6_A125b_FeatureSetToFalse", false)]
        [InlineData(true, "Net6_A125c_FeatureSetToTrue", true)]
        public async Task Return_expected_for_inserted_key_value(
            bool? expected,
            string featureName,
            bool? insertValue
            )
        {
            var sut = CreateSut();
            await sut.SetNullableAsync(featureName, insertValue);

            var featureTableValues = await _dbFixture.GetAllData();
            Assert.NotEmpty(featureTableValues);

            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, "Net6_A349x_FeatureSetToNull", null)]
        [InlineData(false, "Net6_A349y_FeatureSetToFalse", false)]
        [InlineData(true, "Net6_A349z_FeatureSetToTrue", true)]
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
        [InlineData(null, "Net6_A359jx_FeatureSetToNull", null)]
        [InlineData(false, "Net6_A359ky_FeatureSetToFalse", false)]
        [InlineData(true, "Net6_A359lz_FeatureSetToTrue", true)]
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

        [Fact]
        public async Task Exercise_SetNullableValue()
        {
            var sut = CreateSut();
            const string baseName = "Net6_C997_ExerciseRepeatedly";
            const int maxIterations = 1500;
            for (var i = 0; i < maxIterations; i++)
            {
                var callSet = Rng.GetInteger(0, 20) == 0;
                var value = Rng.GetNullableBoolean();
                var featureName = $"{baseName}{Rng.GetInteger(0, 10)}";
                if (callSet) await sut.SetNullableAsync(featureName, value);
                var result = await sut.GetAsync(featureName);
                if (callSet) Assert.Equal(value, result);
            }
        }

        [Fact]
        public async Task Exercise_SetValue()
        {
            var sut = CreateSut();
            const string baseName = "Net6_C877_ExerciseRepeatedly";
            const int maxIterations = 1500;
            for (var i = 0; i < maxIterations; i++)
            {
                var callSet = Rng.GetInteger(0, 20) == 0;
                var value = Rng.GetBoolean();
                var featureName = $"{baseName}{Rng.GetInteger(0, 10)}";
                if (callSet) await sut.SetAsync(featureName, value);
                var result = await sut.GetAsync(featureName);
                if (callSet) Assert.Equal(value, result);
            }
        }
    }
}
