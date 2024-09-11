using DestinyLimoServer.Common.DB;
using DestinyLimoServer.Common.Repository;
using DestinyLimoServer.Models;
using DestinyLimoServer.Repositories;

namespace DestinyLimoServer.Repositories.impl
{
    public class UserRepository(DapperContext dapperContext) : BaseRepository<User>(dapperContext, "users", "user_id"), IUserRepository
    {
        public async Task<IEnumerable<Role>> GetUserRolesByUserId(int userId)
        {
            BaseRepository<Role> roleRepo = new BaseRepository<Role>(_context, "roles", "role_id");

            return await roleRepo.QueryAsync("SELECT r.role_id, r.role_name FROM roles r JOIN user_roles ur ON r.role_id = ur.role_id WHERE ur.user_id = @userId AND r.is_deleted = false", new { userId = userId });
        }

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
            return await QueryAsync("SELECT * FROM users is_approved = true");
        }

        public async Task<IEnumerable<User>> GetUsersByRole(string roleName, bool active = true, bool approved = true)
        {
            return await QueryAsync("SELECT * FROM users T1 INNER JOIN user_roles T2 ON T1.user_id = T2.user_id WHERE T1.is_active = @active AND T1.is_approved = @approved AND T2.role_id = (SELECT role_id FROM roles WHERE role_name = @roleName)", new { active = active, approved = approved, roleName = roleName });
        }

        public async Task<IEnumerable<User>> GetUsersByRoleId(int roleId, bool active = true, bool approved = true)
        {
            return await QueryAsync("SELECT * FROM users T1 INNER JOIN user_role T2 ON T1.user_id = T2.user_id WHERE T1.is_active = @active AND T2.is_approved = @approved AND T2.role_id = @roleId", new { active = active, approved = approved, roleId = roleId });
        }

        public async Task<User> RegisterUser(User user, UserProfile userProfile)
        {
            int userId = await AddAsync(user);
            if (userId == -1)
            {
                return null;
            }

            userProfile.user_id = userId;
            IUserProfileRepository userProfileRepo = new UserProfileRepository(_context);

            await userProfileRepo.AddAsync(userProfile);

            return await GetUserById(userId);
        }

        // --------------------------------------------
        public async Task<User> AuthenticateUser(string username, string password)
        {
            User user = await QueryFirstOrDefaultAsync("SELECT user_id FROM users WHERE username = @username AND password_hash = @password", new { username = username, password = password });
            return user;
        }

        public async Task<User> LogoutUser()
        {
            // TODO: add login related table and update the login status
            return null;
        }

        // TODO: hash the password
        public async Task<User> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            User user = await QueryFirstOrDefaultAsync("SELECT user_id FROM users WHERE user_id = @userId AND password_hash = @oldPassword", new { userId = userId, oldPassword = oldPassword });
            if (user == null)
            {
                return user;
            }

            await ExecuteAsync("UPDATE users SET password_hash = @newPassword WHERE user_id = @userId", new { userId = userId, newPassword = newPassword });

            return user;
        }

        public async Task<User> ForgotPassword(string email)
        {
            User user = await QueryFirstOrDefaultAsync("SELECT user_id FROM users WHERE email = @email", new { email = email });
            if (user == null)
            {
                return user;
            }

            // TODO send email with password reset link
            return user;
        }

        // TODO: hash the password
        public async Task<User> ResetPassword(int userId, string newPassword)
        {
            await ExecuteAsync("UPDATE users SET password_hash = @newPassword WHERE user_id = @userId", new { userId = userId, newPassword = newPassword });
            return await GetUserById(userId);
        }

        // --------------------------------------------
        public async Task<User> ApproveRejectUser(int userId, bool isApproved, string approveRejectReason, int approvedRejectedBy)
        {
            await ExecuteAsync("UPDATE users SET is_approved = @isApproved, approve_reject_reason = @approveRejectReason, approved_rejected_by = @approvedRejectedBy WHERE user_id = @userId", new { userId = userId, isApproved = isApproved, approveRejectReason = approveRejectReason, approvedRejectedBy = approvedRejectedBy });
            return await GetUserById(userId);
        }

        public async Task<User> LockUser(int userId, bool isLocked)
        {
            await ExecuteAsync("UPDATE users SET is_locked = @isLocked WHERE user_id = @userId", new { userId = userId, isLocked = isLocked });
            return await GetUserById(userId);
        }

        // --------------------------------------------
        public async Task<User> UpdateUser(User user)
        {
            await UpdateAsync(user, user.user_id);
            return user;
        }

        public async Task<User> DeleteUser(int userId)
        {
            await DeleteAsync(userId);
            return await GetUserById(userId);
        }

        public async Task<User> DeleteSoftUser(int userId)
        {
            await ExecuteAsync("UPDATE users SET is_deleted = true WHERE user_id = @userId", new { userId = userId });
            return await GetUserById(userId);
        }
    }
}