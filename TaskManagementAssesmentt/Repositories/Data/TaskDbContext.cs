
using Microsoft.EntityFrameworkCore;
using TaskManagementAssesmentt.Entities;
using TaskManagementAssesmentt.Entities._enum;

namespace TaskManagementAssesmentt.Repositories.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
        {
        }

        public DbSet<TaskEntity> Tasks { set; get; }
        public DbSet<TeamMemberEntity> TeamMembers { set; get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<TaskEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.Priority).HasConversion<int>();

                entity.HasOne(e => e.AssignedTo)
                    .WithMany(tm => tm.AssignedTasks)
                    .HasForeignKey(e => e.AssignedToId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.AssignedToId);
                entity.HasIndex(e => e.DueDate);
            });


            modelBuilder.Entity<TeamMemberEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasMaxLength(100);
            });


        }

    }
}