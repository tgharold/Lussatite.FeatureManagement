using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyCache;
using Lussatite.FeatureManagement.Net48.Tests.Testing.SQLServer;
using Lussatite.FeatureManagement.SessionManagers;
using TestCommon.Standard;
using TestCommon.Standard.MicrosoftSQLServer;
using Xunit;

namespace Lussatite.FeatureManagement.Net48.Tests.SessionManagers.Sql
{
    [Collection(nameof(SQLServerDatabaseCollection))]
    public class CachedSqlPerGuidSessionManagerSqlClientTests
    {
        private readonly SqlServerDatabaseFixture _dbFixture;
        private Guid PrimaryUser => _userGuids.First();
        private readonly List<Guid> _userGuids = new List<Guid>();
        private readonly Random _random = new Random();

        public CachedSqlPerGuidSessionManagerSqlClientTests(SqlServerDatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            for (var i = 0; i < 20; i++) _userGuids.Add(Guid.NewGuid());
            _userSessionManagers = CreateSuts();
        }

        private Guid GetRandomUserGuid()
        {
            var index = _random.Next(0, _userGuids.Count);
            return _userGuids[index];
        }

        private readonly IDictionary<Guid, SqlSessionManager> _userSessionManagers;
        private readonly IAppCache _appCache = new CachingService();

        private IDictionary<Guid,SqlSessionManager> CreateSuts()
        {
            var baseSetting = _dbFixture.SQLServerPerGuidSessionManagerSettings;
            var suts = new Dictionary<Guid, SqlSessionManager>();
            foreach (var userGuid in _userGuids)
            {
                var setting = baseSetting.JsonClone();
                setting.UserGuid = userGuid;
                suts[userGuid] = new CachedSqlSessionManager(setting, appCache: _appCache);
            }

            return suts;
        }

        private SqlSessionManager GetSutForUser(Guid userGuid) => _userSessionManagers[userGuid];

        [Fact]
        public async Task Return_null_for_nonexistent_key()
        {
            var sut = GetSutForUser(PrimaryUser);
            var result = await sut.GetAsync("someRandomFeatureNonExistentName");
            Assert.Null(result);
        }

        [Theory]
        [InlineData(null, "Net48_A125a_FeatureSetToNull", null)]
        [InlineData(false, "Net48_A125b_FeatureSetToFalse", false)]
        [InlineData(true, "Net48_A125c_FeatureSetToTrue", true)]
        public async Task Return_expected_for_inserted_key_value(
            bool? expected,
            string featureName,
            bool? insertValue
            )
        {
            var sut = GetSutForUser(PrimaryUser);
            await sut.SetNullableAsync(featureName, insertValue);

            var featureTableValues = await _dbFixture.GetAllData(sut.Settings);
            Assert.NotEmpty(featureTableValues);

            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, "Net48_A349x_FeatureSetToNull", null)]
        [InlineData(false, "Net48_A349y_FeatureSetToFalse", false)]
        [InlineData(true, "Net48_A349z_FeatureSetToTrue", true)]
        public async Task Return_expected_for_SetNullableValue(
            bool? expected,
            string featureName,
            bool? enabled
            )
        {
            var sut = GetSutForUser(PrimaryUser);
            await sut.SetNullableAsync(featureName, enabled);

            var featureTableValues = await _dbFixture.GetAllData(sut.Settings);
            Assert.NotEmpty(featureTableValues);

            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, "Net48_A359jx_FeatureSetToNull", null)]
        [InlineData(false, "Net48_A359ky_FeatureSetToFalse", false)]
        [InlineData(true, "Net48_A359lz_FeatureSetToTrue", true)]
        public async Task Return_expected_for_SetValue(
            bool? expected,
            string featureName,
            bool? enabled
            )
        {
            var sut = GetSutForUser(PrimaryUser);
            if (enabled.HasValue) await sut.SetAsync(featureName, enabled.Value);
            else await sut.SetNullableAsync(featureName, null);

            var featureTableValues = await _dbFixture.GetAllData(sut.Settings);
            Assert.NotEmpty(featureTableValues);

            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task Exercise_SetNullableValue()
        {
            const string baseName = "Net48_C997_ExerciseRepeatedly";
            const int maxIterations = 1500;
            for (var i = 0; i < maxIterations; i++)
            {
                var userGuid = GetRandomUserGuid();
                var sut = GetSutForUser(userGuid);
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
            const string baseName = "Net48_C877_ExerciseRepeatedly";
            const int maxIterations = 1500;
            for (var i = 0; i < maxIterations; i++)
            {
                var userGuid = GetRandomUserGuid();
                var sut = GetSutForUser(userGuid);
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
