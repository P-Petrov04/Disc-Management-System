using Common.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Disc> Discs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rental> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relationship between Rentals and Users
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.UserId);

            // Relationship between Rentals and Discs
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Disc)
                .WithMany(d => d.Rentals)
                .HasForeignKey(r => r.DiscId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>().HasData(
                    new User
                    {
                        UserId = 1, // Ensure this ID is unique
                        FirstName = "Admin",
                        LastName = "User",
                        Email = "admin@email.com",
                        Password = "adminPassword",
                        PhoneNumber = "1234567890",
                        RegistrationDate = DateTime.UtcNow,
                        IsActive = true
                    }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
