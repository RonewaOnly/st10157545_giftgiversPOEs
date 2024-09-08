using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10157545_GIFTGIVERS.Models;

namespace ST10157545_GIFTGIVERS.Controllers
{
    public class DatabaseController : DbContext
    {
        public DatabaseController(DbContextOptions<DatabaseController> options):base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Admins> Admins { get; set; }
        public DbSet<Volunteers> Volunteers { get;set; }
        public DbSet<ValidateAccess> ValidateAccesses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ValidateAccess>()
                .ToTable("VALIDATEACCESS")
                .HasKey(v => v.Val_Id);
            modelBuilder.Entity<ValidateAccess>()
        .Property(v => v.Admin_Id).HasColumnName("admin_id"); // Ensure column names match
            modelBuilder.Entity<ValidateAccess>()
                .Property(v => v.Volunteer_Id).HasColumnName("volunteer_id"); // Ensure column names match
            modelBuilder.Entity<ValidateAccess>()
                .Property(v => v.User_Id).HasColumnName("user_id");
            modelBuilder.Entity<Users>()
               .HasIndex(u => u.Username)
               .IsUnique();
            modelBuilder.Entity<ValidateAccess>()
       .HasOne(v => v.User)
       .WithMany()
       .HasForeignKey(v => v.User_Id);

            modelBuilder.Entity<ValidateAccess>()
                .HasOne(v => v.Admin)
                .WithMany()
                .HasForeignKey(v => v.Admin_Id);

            modelBuilder.Entity<ValidateAccess>()
                .HasOne(v => v.Volunteer)
                .WithMany()
                .HasForeignKey(v => v.Volunteer_Id);

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Phone)
                .IsUnique();

            modelBuilder.Entity<Admins>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Admins>()
                .HasIndex(a => a.Email)
                .IsUnique();
            modelBuilder.Entity<Admins>()
                .HasIndex(a => a.Phone)
                .IsUnique();

            modelBuilder.Entity<Volunteers>()
                .HasIndex(v => v.Username)
                .IsUnique();

            modelBuilder.Entity<Volunteers>()
                .HasIndex(v => v.Email)
                .IsUnique();

            modelBuilder.Entity<Volunteers>()
                .HasIndex(v => v.Phone)
                .IsUnique();

            modelBuilder.Entity<Volunteers>()
                .Property(v => v.Skills)
                .HasDefaultValue("none");

            modelBuilder.Entity<Volunteers>()
                .Property(v => v.Gender)
                .HasDefaultValue("I prefer not to say");

            modelBuilder.Entity<Volunteers>()
                .Property(v => v.Student)
                .HasDefaultValue(0);
        }

        // Manage transactions
        public async Task SaveChangesAsync()
        {
            // Handle transactions manually if needed
            using var transaction = await Database.BeginTransactionAsync();
            try
            {
                await base.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
