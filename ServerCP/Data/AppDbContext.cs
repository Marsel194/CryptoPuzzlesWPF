using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Models;

namespace CryptoPuzzles.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<EncryptionMethod> EncryptionMethods { get; set; }
        public DbSet<Puzzle> Puzzles { get; set; }
        public DbSet<Hint> Hints { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<Tutorial> Tutorials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.CreatedAt)
                      .HasColumnName("created_at")
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(u => u.PasswordHash)
                      .HasColumnName("password_hash");

                entity.Property(u => u.IsDeleted)
                      .HasColumnName("is_deleted");

                entity.Property(u => u.DeletedAt)
                      .HasColumnName("deleted_at");

                entity.HasIndex(u => u.Login).IsUnique().HasDatabaseName("idx_users_login");
                entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("idx_users_email");
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admins");
                entity.HasKey(a => a.Id);

                entity.Property(a => a.CreatedAt)
                      .HasColumnName("created_at")
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(a => a.PasswordHash)
                      .HasColumnName("password_hash");

                entity.Property(a => a.FirstName)
                      .HasColumnName("first_name");

                entity.Property(a => a.LastName)
                      .HasColumnName("last_name");

                entity.Property(a => a.MiddleName)
                      .HasColumnName("middle_name");

                entity.Property(a => a.IsDeleted)
                      .HasColumnName("is_deleted");

                entity.Property(a => a.DeletedAt)
                      .HasColumnName("deleted_at");

                entity.HasIndex(a => a.Login)
                      .IsUnique()
                      .HasDatabaseName("idx_admins_login");
            });

            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.Property(g => g.SessionStartTime)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(g => new { g.UserId, g.SessionStartTime }).HasDatabaseName("idx_game_sessions_user");
            });

            modelBuilder.Entity<Tutorial>(entity =>
            {
                entity.Property(t => t.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(t => t.UpdatedAt)
                      .ValueGeneratedOnAddOrUpdate()
                      .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Puzzle>()
                .HasOne(p => p.Difficulty)
                .WithMany(d => d.Puzzles)
                .HasForeignKey(p => p.DifficultyId);
        }
    }
}