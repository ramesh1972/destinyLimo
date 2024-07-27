using DestinyLimo.Common.DB;
using DestinyLimo.Common.Repository;
using DestinyLimo.Models;

namespace DestinyLimo.Repositories.impl
{
    public class UserRepository(DapperContext dapperContext) : BaseRepository<User>(dapperContext, "users", "user_id"), IUserRepository
    {
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await GetAllAsync();
        }

        public async Task<User> GetUserById(int userId)
        {
            return await GetByIdAsync(userId);
        }

        public Task<User> GetUserByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserRole(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUsers(bool active = true, bool approved = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUsersByRole(string roleName, bool active = true, bool approved = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUsersByRoleId(int roleId, bool active = true, bool approved = true)
        {
            throw new NotImplementedException();
        }

        public async Task<User> AddUser(User user)
        {
            await AddAsync(user);
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            await UpdateAsync(user);
            return user;
        }

        public async Task<bool> DeleteUser(int userId)
        {
            await DeleteAsync(userId);
            return true;
        }
    }
}