using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebAPIUserApp.Domain.Repositories.Interfaces;
using WebAPIUserApp.Models;

namespace WebAPIUserApp.Domain.Repositories.EntityFramework
{
    /// <summary>
    /// EntityFramework realization of IRoleRepository
    /// </summary>
    public class EFRoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public EFRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public Role GetRoleByName(string roleName)
        {
            return _context.Roles.FirstOrDefault(role => role.RoleName.ToLower() == roleName.ToLower());
        }

        public bool IsRoleExist(string roleName)
        {
            return _context.Roles.Any(role => role.RoleName.ToLower() == roleName.ToLower());
        }

        public bool UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            return Save();
        }
        private bool Save()
        {
            var isSaved = _context.SaveChanges();
            return isSaved > 0;
        }
    }
}
