using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using st10157545_giftgiversPOEs.Models;

namespace st10157545_giftgiversPOEs.Controllers
{
    public class DatabaseController : DbContext
    {
        public DatabaseController(DbContextOptions<DatabaseController> options):base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Admins> Admins { get; set; }
        public DbSet<Volunteers> Volunteers { get;set; }
        public DbSet<ValidateAccess> ValidateAccesses { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<Report> Reports { get; set; }  // Add Report DbSet
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ReliefProject> ReliefProjects { get; set; }
        public DbSet<Donation> Donations { get; set; }

        public DbSet<Events> Events { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // VALIDATEACCESS entity configuration
            modelBuilder.Entity<ValidateAccess>()
                .ToTable("VALIDATEACCESS")
                .HasKey(v => v.Val_Id);

            modelBuilder.Entity<ValidateAccess>()
                .Property(v => v.Admin_Id)
                .HasColumnName("admin_id");

            modelBuilder.Entity<ValidateAccess>()
                .Property(v => v.Volunteer_Id)
                .HasColumnName("volunteer_id");

            modelBuilder.Entity<ValidateAccess>()
                .Property(v => v.User_Id)
                .HasColumnName("user_id");

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

            // USERS entity configuration
            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Phone)
                .IsUnique();

            // ADMINS entity configuration
            modelBuilder.Entity<Admins>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Admins>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Admins>()
                .HasIndex(a => a.Phone)
                .IsUnique();

            // VOLUNTEERS entity configuration
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

            // REPORT entity configuration
            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("REPORT");

                // Corrected SQL for computed column
                entity.Property(e => e.report_id)
                    .HasComputedColumnSql("CONCAT('report-', RIGHT('000' + CAST(Id AS VARCHAR(3)), 3))")
                    .IsRequired();

                // Set up foreign key relationship with nullable delete behavior
                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.user_id)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // RESOURCES entity configuration
            modelBuilder.Entity<Resource>()
                .ToTable("RESOURCES")
                .HasKey(r => r.ResourceId);

            modelBuilder.Entity<Resource>()
                .Property(r => r.ResourceId)
                .HasColumnName("resource_id");

            modelBuilder.Entity<Resource>()
                .Property(r => r.AdminId)
                .HasColumnName("admin_id");

            // RELIEF_PROJECT entity configuration
            modelBuilder.Entity<ReliefProject>()
                .ToTable("RELIEF_PROJECT")
                .HasKey(rp => rp.relief_id);

            modelBuilder.Entity<ReliefProject>()
                .Property(rp => rp.relief_id)
                .HasColumnName("relief_id");

            modelBuilder.Entity<ReliefProject>()
                .HasOne(rp => rp.Resource)
                .WithMany()
                .HasForeignKey(rp => rp.resource_id)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ReliefProject>()
                .HasOne(rp => rp.Admin)
                .WithMany()
                .HasForeignKey(rp => rp.admin_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReliefProject>()
                .HasOne(rp => rp.Volunteer)
                .WithMany()
                .HasForeignKey(rp => rp.volunteer_id)
                .OnDelete(DeleteBehavior.SetNull);

            // DONATIONS entity configuration
            modelBuilder.Entity<Donation>()
                .ToTable("DONATIONS")
                .HasKey(d => d.DonationId);

            modelBuilder.Entity<Donation>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Admin)
                .WithMany()
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Volunteer)
                .WithMany()
                .HasForeignKey(d => d.VolunteerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Event)
                .WithMany()
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // EVENTS entity configuration
            modelBuilder.Entity<Events>()
                .ToTable("EVENTS")
                .HasKey(e => e.event_id);

            modelBuilder.Entity<Events>()
                .HasOne(e => e.Admin)
                .WithMany(a => a.Events)
                .HasForeignKey(e => e.admin_id)
                .OnDelete(DeleteBehavior.Cascade);
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
