using DestinyLimoServer.Common.DB;
using DestinyLimoServer.Common.Repository;
using DestinyLimoServer.Models;

namespace DestinyLimoServer.Repositories.impl
{
    public class UserRepository(DapperContext dapperContext) : BaseRepository<User>(dapperContext, "users", "user_id"), IUserRepository
    {
        public async Task<IEnumerable<User>> GetUsers(bool inactive = true, bool is_deleted = false) 
        {
            return await GetAllAsync(inactive, is_deleted);
        }

        public async Task<User> GetUserById(int userId)
        {
            return await GetByIdAsync(userId);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await QueryFirstOrDefaultAsync("SELECT * FROM users WHERE username = @username", new { username = username });
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await QueryFirstOrDefaultAsync("SELECT * FROM users WHERE email = @email", new { email = email });
        }

        public async Task<IEnumerable<User>> GetApprovedUsers()
        {
            return await QueryAsync("SELECT * FROM users WHERE active = true AND approved = true");
        }
    }
}