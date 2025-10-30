using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<AuthUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        
            modelBuilder.Entity<AuthUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<AuthUser>()
                .HasIndex(u => u.Role);

          
            modelBuilder.Entity<AuthUser>().HasData(
                new AuthUser
                {
                    Id = 1,
                    Name = "Admin Teacher",
                    Email = "teacher@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = "Teacher",
                
                    DateOfBirth = new DateTime(1990, 1, 1),
                    CreatedAt = DateTime.UtcNow
                },
                new AuthUser
                {
                    Id = 2,
                    Name = "Test Student",
                    Email = "student@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("qwerty"),
                    Role = "Student",
                   
                    DateOfBirth = new DateTime(2000, 1, 1),
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
