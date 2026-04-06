using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint on Email
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            // Seed data
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 1,
                    Name = "Alice Johnson",
                    Email = "alice@example.com",
                    Age = 21,
                    Course = "Computer Science",
                    CreatedDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                new Student
                {
                    Id = 2,
                    Name = "Bob Smith",
                    Email = "bob@example.com",
                    Age = 23,
                    Course = "Information Technology",
                    CreatedDate = new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
