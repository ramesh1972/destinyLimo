using DestinyLimo.Common.Repository;
using DestinyLimo.Models;
using DestinyLimo.Common.DB;
using DestinyLimo.Repositories;

namespace DestinyLimo.Repositories.impl
{
    public class RoleRepository(DapperContext dapperContext) : BaseRepository<Role>(dapperContext, "roles", "role_id"), IRoleRepository
    {
        public async Task<IEnumerable<Role>> GetRoles()
        {
            return await GetAllAsync();
        }

        public async Task<Role> GetRole(int roleId)
        {
            return await GetByIdAsync(roleId);
        }

        public async Task<Role> GetRole(string roleName)
        {
            return await QueryFirstOrDefaultAsync("SELECT * FROM Role WHERE RoleName = @RoleName", new { RoleName = roleName });
        }

        public async Task AddRole(Role role)
        {
            await AddAsync(role);
        }

        public async Task UpdateRole(Role role)
        {
            await UpdateRole(role);
        }

        public async Task DeleteRole(int roleId)
        {
            await DeleteAsync(roleId);
        }
    }
}