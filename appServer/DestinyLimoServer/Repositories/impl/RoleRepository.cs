using DestinyLimoServer.Common.Repository;
using DestinyLimoServer.Models;
using DestinyLimoServer.Common.DB;
using DestinyLimoServer.Repositories;

namespace DestinyLimoServer.Repositories.impl
{
    public class RoleRepository(DapperContext dapperContext) : BaseRepository<Role>(dapperContext, "roles", "role_id"), IRoleRepository
    {
        public async Task<IEnumerable<Role>> GetRoles(bool inactive = true, bool is_deleted = false)
        {
            return await GetAllAsync(inactive, is_deleted);
        }

        public async Task<Role> GetRole(int roleId)
        {
            return await GetByIdAsync(roleId);
        }

        public async Task<Role> GetRole(string roleName)
        {
            return await QueryFirstOrDefaultAsync("SELECT * FROM Role WHERE RoleName = @RoleName", new { RoleName = roleName });
        }
    }
}