using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PMS.Domain.Entities;

namespace PMS.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> Tasks { get; set; } 
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Id).ValueGeneratedOnAdd();
                b.Property(p => p.Name).IsRequired();
                b.Property(p => p.CreatedAt).IsRequired();
                b.HasMany(p => p.Tasks).WithOne(t => t.Project).HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TaskItem>(b =>
            {
                b.HasKey(t => t.Id);
                b.Property(t => t.Id).ValueGeneratedOnAdd();
                b.Property(t => t.Title).IsRequired();
                b.Property(t => t.Status).HasConversion<int>();
                b.Property(t => t.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Employee>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.Property(e => e.UserId).IsRequired();
                b.Property(e => e.Name).IsRequired();
                b.Property(e => e.Email).IsRequired();
                b.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
