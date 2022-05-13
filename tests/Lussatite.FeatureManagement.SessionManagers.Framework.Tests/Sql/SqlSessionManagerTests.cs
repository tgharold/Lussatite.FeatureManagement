using System.Data.SqlClient;

namespace Lussatite.FeatureManagement.SessionManagers.Framework.Tests.Sql
{
    public class SqlSessionManagerTests
    {
        private SqlSessionManager CreateSut(
            SqlSessionManagerSettings settings = null
            )
        {
            return new SqlSessionManager(
                settings: settings,
                sqlCommandFactory: (s =>
                {
                    return new SqlCommand();
                })
            );
        }
    }
}
