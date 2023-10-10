using Microsoft.EntityFrameworkCore;
using WebAPIUserApp.Models;

namespace WebAPIUserApp.Domain
{
    /// <summary>
    /// AppDbContext class
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">DbContext Options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Role>().HasData(new Role
            {
                Id = 1,
                RoleName = "User",
                Users = new List<User> { }
            });
            builder.Entity<Role>().HasData(new Role
            {
                Id = 2,
                RoleName = "Admin",
                Users = new List<User> { }
            });
            builder.Entity<Role>().HasData(new Role
            {
                Id = 3,
                RoleName = "Support",
                Users = new List<User> { }
            });
            builder.Entity<Role>().HasData(new Role
            {
                Id = 4,
                RoleName = "SuperAdmin",
                Users = new List<User> { }
            });
        }
    }
}
