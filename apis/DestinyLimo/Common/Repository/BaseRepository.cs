using Dapper;
using DestinyLimo.Common.DB;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DestinyLimo.Common.Repository
{
    public class BaseRepository<T>(DapperContext context, string tableName, string idColumn) : IBaseRepository<T> where T : class
    {
        private DapperContext _context = context;
        private readonly string? _tableName = tableName;
        private readonly string? _idColumn = idColumn;

        private MySqlConnection CreateConnection()
        {
            return _context.CreateConnection();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = $"SELECT * FROM {_tableName}";
            return await connection.QueryAsync<T>(sql);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE {_idColumn} = @Id";
            var result = await connection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
            return result ?? default!;
        }

        public async Task AddAsync(T? entity)
        {
            using var connection = CreateConnection();
            var sql = $"INSERT INTO {_tableName} ({GetColumns()}) VALUES ({GetParameters()})";
            await connection.ExecuteAsync(sql, entity);
        }

        public async Task UpdateAsync(T? entity)
        {
            using var connection = CreateConnection();
            var sql = $"UPDATE {_tableName} SET {GetUpdateColumns()} WHERE {_idColumn} = @Id";
            await connection.ExecuteAsync(sql, entity);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = $"DELETE FROM {_tableName} WHERE {_idColumn} = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<T>> QueryAsync(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(sql, parameters);
        }

        public async Task<T> QuerySingleAsync(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
            return result ?? default!;
        }

        public async Task<T> QueryFirstOrDefaultAsync(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
            return result ?? default!;
        }
        public async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<IEnumerable<dynamic>> QueryDynamicAsync(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync(sql, parameters);
        }

        private string GetColumns()
        {
            return string.Join(", ", typeof(T).GetProperties().Select(p => p.Name));
        }

        private string GetParameters()
        {
            return string.Join(", ", typeof(T).GetProperties().Select(p => "@" + p.Name));
        }
        private string GetUpdateColumns()
        {
            return string.Join(", ", typeof(T).GetProperties().Select(p => p.Name + " = @" + p.Name));
        }
    }
}