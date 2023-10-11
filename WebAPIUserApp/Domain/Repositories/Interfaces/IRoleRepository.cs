using WebAPIUserApp.Models;

namespace WebAPIUserApp.Domain.Repositories.Interfaces
{
    /// <summary>
    /// IRoleRepository interface
    /// </summary>
    public interface IRoleRepository
    {
        public Role GetRoleByName(string roleName);
        public bool IsRoleExist(string roleName);
        public bool UpdateRole(Role role);
    }
}
