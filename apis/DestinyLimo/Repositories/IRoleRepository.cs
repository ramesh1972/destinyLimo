using DestinyLimo.Models;

namespace DestinyLimo.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetRoles();
        Task<Role> GetRole(int roleId);
        Task<Role> GetRole(string roleName);
        Task AddRole(Role role);
        Task UpdateRole(Role role);
        Task DeleteRole(int roleId);
    }
}