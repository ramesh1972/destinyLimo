using DestinyLimoServer.Models;

namespace DestinyLimoServer.Repositories
{
    public interface IUserProfileRepository
    {
        Task<IEnumerable<UserProfile>> GetUsers(bool inactive = true, bool is_deleted = false);
        Task<IEnumerable<UserProfile>> GetApprovedUsers();
        Task<UserProfile> GetUserById(int userId);
        Task<UserProfile> GetUserByUsername(string username);
        Task<UserProfile> GetUserByEmail(string email);
        Task<UserProfile> GetUserByPhoneNumber(string phoneNumber);
        Task<IEnumerable<UserProfile>> GetUsersByRole(string roleName, bool active = true, bool approved = true);
        Task<IEnumerable<UserProfile>> GetUsersByRoleId(int roleId, bool active = true, bool approved = true);

        Task<int> AddAsync(UserProfile entity, string[]? cols = null);
        Task<bool> UpdateAsync(UserProfile entity, int id, string[]? cols = null);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteSoftAsync(int id);
    }
}