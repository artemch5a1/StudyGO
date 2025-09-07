using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using StudyGO.infrastructure.Entities;

namespace StudyGO.infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseExceptionProcessor();
        }

        public DbSet<UserEntity> UsersEntity { get; set; }

        public DbSet<FormatEntity> FormatsEntity { get; set; }

        public DbSet<TutorProfileEntity> TutorProfilesEntity { get; set; }

        public DbSet<UserProfileEntity> UserProfilesEntity { get; set; }

        public DbSet<SubjectEntity> SubjectsEntity { get; set; }

        public DbSet<TutorSubjectsEntity> TutorSubjectsEntity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasDefaultValueSql("gen_random_uuid()");
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<FormatEntity>(entity =>
            {
                entity.HasKey(e => e.FormatId);
            });

            modelBuilder.Entity<TutorProfileEntity>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity
                    .HasOne(e => e.User)
                    .WithOne(f => f.TutorProfile)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(e => e.Format)
                    .WithMany(f => f.TutorProfiles)
                    .HasForeignKey(x => x.FormatId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SubjectEntity>(entity =>
            {
                entity.HasKey(e => e.SubjectId);
                entity.Property(e => e.SubjectId).HasDefaultValueSql("gen_random_uuid()");
            });

            modelBuilder.Entity<UserProfileEntity>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity
                    .HasOne(e => e.User)
                    .WithOne(f => f.UserProfile)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(e => e.FavoriteSubject)
                    .WithMany(f => f.UserProfiles)
                    .HasForeignKey(e => e.SubjectId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TutorSubjectsEntity>(entity =>
            {
                entity.HasKey(e => new {e.SubjectId, e.TutorId});

                entity.HasOne(e => e.Subject)
                    .WithMany(e => e.TutorSubjects)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Tutor)
                    .WithMany(e => e.TutorSubjects)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
