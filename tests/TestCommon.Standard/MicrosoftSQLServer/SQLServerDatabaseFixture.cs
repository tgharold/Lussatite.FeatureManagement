using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Threading.Tasks;
using Lussatite.FeatureManagement.SessionManagers;
using Lussatite.FeatureManagement.SessionManagers.SQLite;

namespace TestCommon.Standard.MicrosoftSQLServer
{
    public class SqlServerDatabaseFixture : IDisposable
    {
        public SqlSessionManagerSettings SqlSessionManagerSettings { get; }

        public SqlServerDatabaseFixture()
        {
            SqlSessionManagerSettings = new SQLiteSessionManagerSettings
            {
                // Using a name and a shared cache allows multiple connections to access the same in-memory database
                ConnectionString = $"Data Source=test-{Guid.NewGuid()};Mode=Memory;Cache=Shared",

                EnableSetValueCommand = true,
            };

            SqlSessionManagerSettings.CreateDatabaseTable(GetConnectionString());
        }

        public string GetConnectionString() => SqlSessionManagerSettings.ConnectionString;

        /// <summary>Meant to be used as a debug step, this returns all of the data in the table.</summary>
        public async Task<List<object[]>> GetAllData()
        {
            var result = new List<object[]>();
            using (var conn = new SqlConnection(SqlSessionManagerSettings.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = $@"SELECT * FROM {SqlSessionManagerSettings.FeatureTableName};";
                    using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (reader.Read())
                        {
                            var fieldCount = reader.FieldCount;
                            var rowResult = new List<object>();
                            for (int i = 0; i < fieldCount; i++)
                            {
                                rowResult.Add(reader.GetValue(i));
                            }
                            result.Add(rowResult.ToArray());
                        }
                        reader.Close();
                    }
                }
                conn.Close();
            }
            return result;
        }

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SQLiteDatabaseFixture()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
