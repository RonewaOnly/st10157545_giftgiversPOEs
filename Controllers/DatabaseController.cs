using Microsoft.EntityFrameworkCore;
using st10157545_giftgiversPOEs.Models;

namespace st10157545_giftgiversPOEs.Controllers
{
    public class DatabaseController : DbContext
    {
        public DatabaseController(DbContextOptions<DatabaseController> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Admins> Admins { get; set; }
        public DbSet<Volunteers> Volunteers { get; set; }
        public DbSet<ValidateAccess> ValidateAccesses { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<Report> Reports { get; set; }  // Add Report DbSet
        public DbSet<Resources> Resources { get; set; }
        public DbSet<ReliefProject> ReliefProjects { get; set; }
        public DbSet<Donation> Donations { get; set; }

        public DbSet<Events> Events { get; set; }

        public DbSet<Notification> Notifications { get; set; }


        public DbSet<Comment> Comments { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            base.OnConfiguring(optionsBuilder);
        }


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
                .HasIndex(u => u.username)
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.email)
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.phone)
                .IsUnique();

            // ADMINS entity configuration
            modelBuilder.Entity<Admins>(entity =>
            {
                entity.HasKey(a => a.admin_id);
                entity.Property(a => a.admin_id)
                    .HasColumnName("admin_id"); // Map FK column

                entity.HasIndex(a => a.username)
                .IsUnique();
                entity.HasIndex(a => a.email)
                .IsUnique();
                entity.HasIndex(a => a.phone)
                .IsUnique();
            });


            // VOLUNTEERS entity configuration
            modelBuilder.Entity<Volunteers>(entity =>
            {
                entity.HasKey(v => v.volunteer_id);
                entity.Property(v => v.volunteer_id)
                    .HasColumnName("volunteer_id"); // Map FK column
                entity.HasIndex(v => v.username)
                .IsUnique();
                entity.HasIndex(v => v.email)
                .IsUnique();
                entity.HasIndex(v => v.phone)
                .IsUnique();
                entity.Property(v => v.skills)
                .HasDefaultValue("none");
                entity.Property(v => v.gender)
                .HasDefaultValue("I prefer not to say");
                entity.Property(v => v.student)
                .HasDefaultValue(0);
            });


            // REPORT entity configuration
            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("REPORT");
                entity.Property(e => e.report_id)
                    .HasComputedColumnSql("CONCAT('report-', RIGHT('000' + CAST(Id AS VARCHAR(3)), 3))")
                    .IsRequired();
                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.user_id)
                    .OnDelete(DeleteBehavior.SetNull);

            });

            // RESOURCES entity configuration
            modelBuilder.Entity<Resources>(entity =>
            {
                entity.ToTable("RESOURCES");

                entity.HasKey(r => r.resource_id);

                entity.Property(r => r.resource_name)
                    .HasColumnName("resource_name")
                    .HasMaxLength(120)
                    .IsRequired();

                entity.Property(r => r.resource_quantity)
                    .HasColumnName("resource_quantity")
                    .IsRequired();

                entity.Property(r => r.available)
                    .HasColumnName("available")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(r => r.managingPerson)
                    .HasColumnName("managingPerson")
                    .HasColumnType("NVARCHAR(MAX)")
                    .IsRequired();

                entity.Property(r => r.categories)
                    .HasColumnName("categories")
                    .HasMaxLength(90)
                    .HasDefaultValue("unknown");

                entity.Property(r => r.projectUsed)
                    .HasColumnName("projectUsed")
                    .HasColumnType("NVARCHAR(MAX)")
                    .IsRequired();

                entity.Property(r => r.admin_id)
                    .HasColumnName("admin_id")
                    .HasMaxLength(100)
                    .IsRequired();

                // Explicitly configure the relationship
                entity.HasOne(r => r.Admin)
                    .WithMany(a => a.Resources)  // Admin can have multiple resources
                    .HasForeignKey(r => r.admin_id)  // admin_id is the FK
                    .OnDelete(DeleteBehavior.Cascade); // Handle delete behavior
            });

            // RELIEF_PROJECT entity configuration

            modelBuilder.Entity<ReliefProject>(entity =>
            {
                entity.ToTable("RELIEF_PROJECT");
                entity.HasKey(e => e.relief_id);

                entity.Property(e => e.relief_id).HasColumnName("relief_id");
                entity.Property(e => e.admin_id).HasColumnName("admin_id");
                entity.Property(e => e.volunteer_id).HasColumnName("volunteer_id");
                entity.Property(e => e.resource_id).HasColumnName("resource_id");

                entity.HasOne(rp => rp.Admin)
        .WithMany(a => a.ReliefProjects)
        .HasForeignKey(rp => rp.admin_id)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Volunteer)
                    .WithMany(p => p.ReliefProjects)
                    .HasForeignKey(d => d.volunteer_id)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Resource)
                    //.WithMany(p => p.ReliefProjects)
                    .WithMany()
                    .HasForeignKey(d => d.resource_id)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });


            // DONATIONS entity configuration
            modelBuilder.Entity<Donation>(entity =>
            {
                entity.ToTable("DONATIONS").HasKey(d => d.donation_id);
                entity.HasOne(d => d.User)
        .WithMany()
        .HasForeignKey(d => d.user_id)
        .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Admin)
                      .WithMany()
                      .HasForeignKey(d => d.admin_id)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Volunteer)
                      .WithMany()
                      .HasForeignKey(d => d.volunteer_id)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Event)
                      .WithMany(e => e.Donations)
                      .HasForeignKey(d => d.event_id)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ReliefProject).WithMany().HasForeignKey(rp => rp.relief_id);
                entity.Property(d => d.cash_amount)
    .HasColumnType("decimal(18,2)");
            });

            // EVENTS entity configuration
            modelBuilder.Entity<Events>(entity =>
            {
                entity.ToTable("EVENTS");

                entity.HasKey(e => e.event_id);

                entity.HasOne(e => e.Admin)
                    .WithMany(a => a.Events)
                    .HasForeignKey(e => e.admin_id)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //Comment entity 
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");

                entity.HasKey(com => com.Id);

                entity.HasOne(com  => com.Report).WithMany(a => a.Comments).HasForeignKey(com => com.report_id);
                entity.HasOne(com => com.User).WithMany().HasForeignKey(entity => entity.user_id);
            }
            );


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
