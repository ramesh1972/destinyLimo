using MySql.Data.MySqlClient;
using System.Data;

namespace DestinyLimo.Common.DB
{
    public class DapperContext
    {
        private readonly string? _connectionString;

        public DapperContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        public MySqlConnection CreateConnection()
            => new(_connectionString);
    }
}