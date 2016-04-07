using System;
using System.Data;
using System.Data.SqlClient;

namespace SQLCover.Gateway
{
    public class DatabaseGateway
    {
        private readonly string _connectionString;
        private readonly string _databaseName;

        public DatabaseGateway()
        {
            //for mocking.
        }
        public DatabaseGateway(string connectionString, string databaseName)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
        }

        public virtual string GetString(string query)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                conn.ChangeDatabase(_databaseName);
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    return cmd.ExecuteScalar().ToString();
                }
            }
        }

        public virtual DataTable GetRecords(string query)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                conn.ChangeDatabase(_databaseName);
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    using (var reader = cmd.ExecuteReader())
                    {
                        var ds = new DataTable();
                        ds.Load(reader);
                        return ds;
                    }
                }
            }
        }

        public void Execute(string command)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                conn.ChangeDatabase(_databaseName);
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = command;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
