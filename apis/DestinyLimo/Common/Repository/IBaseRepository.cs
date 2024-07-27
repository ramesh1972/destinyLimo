using System.Collections.Generic;
using System.Threading.Tasks;

namespace DestinyLimo.Common.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T? entity);
        Task UpdateAsync(T? entity);
        Task DeleteAsync(int id);

        // Additional methods
        Task<IEnumerable<T>> QueryAsync(string sql, object? parameters = null);
        Task<T> QuerySingleAsync(string sql, object? parameters = null);
        Task<T> QueryFirstOrDefaultAsync(string sql, object? parameters = null);
        Task<int> ExecuteAsync(string sql, object? parameters = null);
        Task<IEnumerable<dynamic>> QueryDynamicAsync(string sql, object? parameters = null);
    }
}