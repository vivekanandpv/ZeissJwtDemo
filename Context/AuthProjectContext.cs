using Microsoft.EntityFrameworkCore;
using ZeissJwtDemo.Models;

namespace ZeissJwtDemo.Context
{
    public class AuthProjectContext : DbContext
    {
        public AuthProjectContext(DbContextOptions<AuthProjectContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AppUserRole> AppUserRoles { get; set; }
        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUserRole>()
                .HasKey(ur => new { ur.AppUserId, ur.RoleId });
            modelBuilder.Entity<Role>().HasData(new Role { RoleId = 1, Name = "admin" }, new Role { RoleId = 2, Name = "manager" }, new Role { RoleId = 3, Name = "user" }, new Role { RoleId = 4, Name = "operator" });
        }
    }
}
