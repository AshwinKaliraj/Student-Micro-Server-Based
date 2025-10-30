using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed admin user (without CreatedAt)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    AuthId = 1,
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Teacher",
                    DateOfBirth = new DateTime(1985, 5, 15)
                }
            );
        }
    }
}
