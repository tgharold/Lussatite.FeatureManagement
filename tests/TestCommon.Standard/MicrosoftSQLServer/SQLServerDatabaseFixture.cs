using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lussatite.FeatureManagement.SessionManagers;
using Lussatite.FeatureManagement.SessionManagers.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TestCommon.Standard.MicrosoftSQLServer
{
    public class SqlServerDatabaseFixture : IDisposable
    {
        public SqlSessionManagerSettings SqlSessionManagerSettings { get; }
        public string DbName { get; }

        /// <summary>Used to create the database to be used for the tests.  The initial catalog
        /// should be 'master' and the user account must have permissions to CREATE DATABASE.</summary>
        private readonly string _masterConnectionString;

        public SqlServerDatabaseFixture()
        {
            var now = DateTimeOffset.UtcNow;
            DbName = $"test-{now:yyyyMMdd}-{now:HHmm}-{UpperCaseAlphanumeric(8)}";

            var configuration = GetConfiguration();
            _masterConnectionString = configuration.GetConnectionString("TestSqlServerDatabaseMaster");

            var connectionString = CreateConnectionStringFromOriginal(_masterConnectionString, DbName);

            SqlSessionManagerSettings = new SQLServerSessionManagerSettings
            {
                ConnectionString = connectionString,

                EnableSetValueCommand = true,
            };

            CreateDatabase();
            SqlSessionManagerSettings.CreateDatabaseTable(connectionString);
        }

        private static readonly Random Random = new Random();

        private static string UpperCaseAlphanumeric(int size)
        {
            string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var chars = Enumerable.Range(0, size)
                .Select(x => input[Random.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }

        private IConfiguration GetConfiguration()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var testEnvironmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(baseDirectory)
                .AddJsonFile($"appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{testEnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            return configurationRoot;
        }

        private bool _databaseInitialized;
        private readonly object _lock = new object();

        private void CreateDatabase()
        {
            // https://en.wikipedia.org/wiki/Double-checked_locking
            if (_databaseInitialized) return;
            lock (_lock)
            {
                if (_databaseInitialized) return;

                try
                {
                    if (!DatabaseExists())
                    {
                        using (var connection = new SqlConnection(_masterConnectionString))
                        {
                            connection.Open();
                            using(var command = connection.CreateCommand())
                            {
                                command.CommandText = FormattableString.Invariant(
                                    $"CREATE DATABASE [{DbName}];");
                                command.ExecuteNonQuery();
                            }

                            var attempt = 1;
                            while (!DatabaseIsAcceptingQueries())
                            {
                                attempt++;
                                var sleepMilliseconds = Math.Min((int) (Math.Pow(1.2, attempt) * 100), 500);
                                Thread.Sleep(sleepMilliseconds);
                                if (attempt > 100)
                                    throw new Exception(
                                        $"Database '{DbName}' refused to execute queries!"
                                    );
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    e.Data["dbName"] = DbName;
                    throw;
                }

                _databaseInitialized = true;
            }
        }

        private bool DatabaseExists()
        {
            using (var connection = new SqlConnection(_masterConnectionString))
            {
                // https://docs.microsoft.com/en-us/sql/t-sql/functions/db-id-transact-sql
                // https://stackoverflow.com/a/2028955
                using (var command = new SqlCommand($@"select DB_ID('{DbName}') NOT NULL;", connection))
                {
                    try
                    {
                        connection.Open();
                        var result = (int?)command.ExecuteScalar();
                        connection.Close();
                        return (result > 0);
                    }
                    catch (SqlException)
                    {
                        return false;
                    }
                }
            }
        }

        private bool DatabaseIsAcceptingQueries()
        {
            using (var connection = new SqlConnection(SqlSessionManagerSettings.ConnectionString))
            {
                var command = new SqlCommand("select 1;", connection);
                try
                {
                    connection.Open();
                    var result = (int)command.ExecuteScalar();
                    return (result == 1);
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        private void DropDatabase()
        {
            using (var connection = new SqlConnection(_masterConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = FormattableString.Invariant(
                        $@"ALTER DATABASE [{DbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{DbName}];"
                        );
                    command.ExecuteNonQuery();
                }
            }
        }

        private static string CreateConnectionStringFromOriginal(string connectionString, string dbName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            if (string.IsNullOrWhiteSpace(dbName))
                throw new ArgumentNullException(nameof(dbName));

            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = dbName
            };

            return builder.ConnectionString;
        }

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

                    // https://en.wikipedia.org/wiki/Double-checked_locking#Usage_in_C#
                    if (!_databaseInitialized) return;
                    lock (_lock)
                    {
                        if (!_databaseInitialized) return;
                        if (DatabaseExists()) DropDatabase();
                        _databaseInitialized = false;
                    }
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
