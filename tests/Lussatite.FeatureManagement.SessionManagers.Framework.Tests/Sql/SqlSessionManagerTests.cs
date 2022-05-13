using Lussatite.FeatureManagement.SessionManagers.Framework.Tests.Testing.SQLite;
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
                commandFactory: s => _dbFixture.CreateCommand(s)
            );
        }

        [Fact]
        public async Task Return_null_for_nonexistent_key()
        {
            var sut = CreateSut();
            var result = await sut.GetAsync("someRandomFeatureNonExistentName");
            Assert.Null(result);
        }
    }
}
