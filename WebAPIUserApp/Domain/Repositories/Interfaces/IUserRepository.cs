using WebAPIUserApp.Models;

namespace WebAPIUserApp.Domain.Repositories.Interfaces
{
    /// <summary>
    /// IUserRepository interface
    /// </summary>
    public interface IUserRepository
    {
        public IQueryable<User> GetAllUsers();
        public IQueryable<User> GetUsers(int page, int pageSize, string nameFilter, string emailFilter, int minAge, int maxAge, string roleFilter, string propertyForOrdering, Direction direction);
        public User GetUserById(Guid id);
        public bool AddUser(User user);
        public bool UpdateUser(User user);
        public bool IsUserExistById(Guid id);
        public bool IsUserExistByEmail(string email);
        public bool DeleteUser(User user);
        public IQueryable<Role> GetRolesByUserId(Guid id);
        public int GetCount();
    }
}
