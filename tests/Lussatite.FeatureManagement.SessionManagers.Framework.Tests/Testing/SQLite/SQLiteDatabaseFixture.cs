using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Lussatite.FeatureManagement.SessionManagers.Framework.Tests.Testing.SQLite
{
    public class SQLiteDatabaseFixture : IDisposable
    {
        // ref: https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/in-memory-databases

        private bool disposedValue;

        // Using a name and a shared cache allows multiple connections to access the same
        // in-memory database
        private readonly string connectionString = $"Data Source=test-{Guid.NewGuid()};Mode=Memory;Cache=Shared";

        // The in-memory database only persists while a connection is open to it.To manage
        // its lifetime, keep one open connection around for as long as you need it.
        private readonly SQLiteConnection _masterConnection;

        public SqlSessionManagerSettings SqlSessionManagerSettings { get; } = new SqlSessionManagerSettings
        {
            FeatureNameColumn = NameColumn,
            FeatureValueColumn = ValueColumn
        };

        public const string TableName = "featureData";
        public const string NameColumn = "featureName";
        public const string ValueColumn = "featureValue";

        public SQLiteDatabaseFixture()
        {
            _masterConnection = new SQLiteConnection(connectionString);
            _masterConnection.Open();

            var createCommand = _masterConnection.CreateCommand();
            createCommand.CommandText =
            $@"
                CREATE TABLE IF NOT EXISTS {TableName} (
                    {NameColumn} TEXT PRIMARY KEY,
                    {ValueColumn} BOOLEAN CHECK ({ValueColumn} IN (0, 1))
                );
            ";
            createCommand.ExecuteNonQuery();
        }

        public string GetConnectionString() => connectionString;

        public DbCommand CreateGetValueCommand(string featureName)
        {
            var conn = new SQLiteConnection(connectionString);
            conn.Open();

            var queryCommand = conn.CreateCommand();
            queryCommand.CommandText =
                $@"SELECT {NameColumn}, {ValueColumn} FROM {TableName} WHERE {ValueColumn} = @featureName;";
            queryCommand.Parameters.Add(new SQLiteParameter("featureName", featureName));

            return queryCommand;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _masterConnection.Close();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                disposedValue = true;
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
