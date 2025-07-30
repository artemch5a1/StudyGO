using Microsoft.EntityFrameworkCore;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Entites;

namespace StudyGO.infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<UserEntity> UsersEntity { get; set; }

        public DbSet<FormatEntity> FormatsEntity { get; set; }

        public DbSet<TutorProfileEntity> TutorProfilesEntity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.UserID).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.Name).HasMaxLength(256);
                entity.Property(e => e.Surname).HasMaxLength(256);
                entity.Property(e => e.Patronymic).HasMaxLength(256);
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.Number).HasMaxLength(11);
            });

            modelBuilder.Entity<FormatEntity>(entity =>
            {
                entity.HasKey(e => e.FormatID);
            });

            modelBuilder.Entity<TutorProfileEntity>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.HasOne(e => e.User).WithOne(f => f.TutorProfile);
            });
        }
    }
}
