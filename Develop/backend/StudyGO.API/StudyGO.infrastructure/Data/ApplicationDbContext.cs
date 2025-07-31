using Microsoft.EntityFrameworkCore;
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

        public DbSet<SubjectEntity> SubjectsEntity { get; set; }

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
                entity
                    .HasOne(e => e.User)
                    .WithOne(f => f.TutorProfile)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(e => e.Format)
                    .WithMany(f => f.TutorProfiles).HasForeignKey(x => x.FormatID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SubjectEntity>(entity =>
            {
                entity.HasKey(e => e.SubjectID);
                entity.Property(e => e.SubjectID).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.Title).HasMaxLength(100);
            });

            modelBuilder.Entity<UserProfileEntity>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity
                    .HasOne(e => e.User)
                    .WithOne(f => f.UserProfile)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(e => e.FavoriteSubject)
                    .WithMany(f => f.UserProfiles).HasForeignKey(e => e.SubjectID)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
