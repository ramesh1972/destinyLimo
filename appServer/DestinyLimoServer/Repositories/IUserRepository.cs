using DestinyLimoServer.Models;

namespace DestinyLimoServer.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers(bool inactive = true, bool is_deleted = false);
        Task<IEnumerable<User>> GetApprovedUsers();
        Task<User> GetUserById(int userId);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserByEmail(string email);
        Task<int> AddAsync(User entity, string[]? cols = null);
        Task<bool> UpdateAsync(User entity, int id, string[]? cols = null);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteSoftAsync(int id);
    }
}