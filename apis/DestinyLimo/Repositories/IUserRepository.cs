using DestinyLimo.Models;

namespace DestinyLimo.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserById(int userId);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByPhoneNumber(string phoneNumber);
        Task<User> GetUserRole(int userId);
        Task<List<User>> GetUsers(bool active = true, bool approved = true);
        Task<List<User>> GetUsersByRole(string roleName, bool active = true, bool approved = true);
        Task<List<User>> GetUsersByRoleId(int roleId, bool active = true, bool approved = true);
        Task<User> AddUser(User user);
        Task<User> UpdateUser(User user);
        Task<bool> DeleteUser(int userId);
    }
}