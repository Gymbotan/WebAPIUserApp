using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebAPIUserApp.Domain.Repositories.Interfaces;
using WebAPIUserApp.Models;

namespace WebAPIUserApp.Domain.Repositories.EntityFramework
{
    /// <summary>
    /// EntityFramework realization of IUserRepository
    /// </summary>
    public class EFUserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public EFUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool AddUser(User user)
        {
            _context.Add(user);
            return Save();
        }

        public bool DeleteUser(User user)
        {
            _context.Remove(user);
            return Save();
        }

        public int GetCount()
        {
            return _context.Users.Count();
        }

        public IQueryable<Role> GetRolesByUserId(Guid id)
        {
            return _context.Users.Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Age = u.Age,
                Roles = u.Roles.Select(r => new Role
                { Id = r.Id, RoleName = r.RoleName }).ToList()
            })
                .FirstOrDefault(user => user.Id == id).Roles.AsQueryable();
        }

        public User GetUserById(Guid id)
        {
            return _context.Users.Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Age = u.Age,
                Roles = u.Roles.Select(r => new Role
                { Id = r.Id, RoleName = r.RoleName }).ToList()
            })
                .FirstOrDefault(user => user.Id == id);
        }

        public IQueryable<User> GetAllUsers()
        {
            return _context.Users.Select(u => new User
            {
                Id = u.Id, 
                Name = u.Name, 
                Email = u.Email,
                Age = u.Age,
                Roles = u.Roles.Select(r => new Role
                {Id = r.Id, RoleName = r.RoleName})
                .ToList()
            }).AsQueryable();
        }

        public bool IsUserExistByEmail(string email)
        {
            return _context.Users.Any(user => user.Email.ToLower() == email.ToLower());
        }

        public bool IsUserExistById(Guid id)
        {
            return _context.Users.Any(user => user.Id == id);
        }

        public bool UpdateUser(User user)
        {
            _context.Update(user);
            return Save();
        }

        private bool Save()
        {
            var isSaved = _context.SaveChanges();
            return isSaved > 0;
        }

        public IQueryable<User> GetUsers(int page, int pageSize, string nameFilter, string emailFilter, int minAge, int maxAge, string roleFilter, string propertyForOrdering, Direction direction)
        {
            return _context.Users.Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Age = u.Age,
                Roles = u.Roles.Select(r => new Role
                { Id = r.Id, RoleName = r.RoleName })
                .ToList()
            })
                .Where(u => u.Name.ToLower().Contains(nameFilter.ToLower()))
                .Where(u => u.Email.ToLower().Contains(emailFilter.ToLower()))
                .Where(u => u.Age >= minAge && u.Age <= maxAge)
                .Where(u => u.Roles.Any(r => r.RoleName.ToLower().Contains(roleFilter.ToLower())))
                .ApplyOrder(propertyForOrdering, direction)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsQueryable();
        }
    }
}
