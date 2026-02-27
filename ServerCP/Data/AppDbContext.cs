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

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .HasDatabaseName("idx_users_login");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .HasDatabaseName("idx_users_email");

            // Admin
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Login)
                .HasDatabaseName("idx_admins_login");

            // Difficulty
            modelBuilder.Entity<Difficulty>()
                .HasIndex(d => d.DifficultyName)
                .HasDatabaseName("idx_difficulties_name");

            // EncryptionMethod
            modelBuilder.Entity<EncryptionMethod>()
                .HasIndex(e => e.Name)
                .HasDatabaseName("idx_methods_name");

            // Puzzle
            modelBuilder.Entity<Puzzle>()
                .HasIndex(p => p.DifficultyId)
                .HasDatabaseName("idx_puzzles_difficulty");

            modelBuilder.Entity<Puzzle>()
                .HasIndex(p => p.MethodId)
                .HasDatabaseName("idx_puzzles_method");

            modelBuilder.Entity<Puzzle>()
                .HasIndex(p => new { p.IsTraining, p.TutorialOrder })
                .HasDatabaseName("idx_puzzles_training");

            modelBuilder.Entity<Puzzle>()
                .HasIndex(p => p.CreatedByAdminId)
                .HasDatabaseName("idx_puzzles_created_by");

            modelBuilder.Entity<Puzzle>()
                .HasOne(p => p.Difficulty)
                .WithMany(d => d.Puzzles)
                .HasForeignKey(p => p.DifficultyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Puzzle>()
                .HasOne(p => p.Method)
                .WithMany(m => m.Puzzles)
                .HasForeignKey(p => p.MethodId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Puzzle>()
                .HasOne(p => p.CreatedByAdmin)
                .WithMany(a => a.CreatedPuzzles)
                .HasForeignKey(p => p.CreatedByAdminId)
                .OnDelete(DeleteBehavior.SetNull);

            // Hint
            modelBuilder.Entity<Hint>()
                .HasIndex(h => new { h.PuzzleId, h.HintOrder })
                .HasDatabaseName("idx_hints_puzzle");

            // GameSession
            modelBuilder.Entity<GameSession>()
                .HasIndex(g => new { g.UserId, g.SessionStartTime })
                .HasDatabaseName("idx_game_sessions_user")
                .IsDescending(new[] { false, true });

            modelBuilder.Entity<GameSession>()
                .HasIndex(g => g.CurrentPuzzleId)
                .HasDatabaseName("idx_game_sessions_puzzle");

            modelBuilder.Entity<GameSession>()
                .HasIndex(g => g.CompletedAt)
                .HasDatabaseName("idx_game_sessions_completed");

            // Tutorial
            modelBuilder.Entity<Tutorial>()
                .HasIndex(t => t.MethodId)
                .HasDatabaseName("idx_tutorials_method");

            modelBuilder.Entity<Tutorial>()
                .HasIndex(t => new { t.IsActive, t.SortOrder })
                .HasDatabaseName("idx_tutorials_active");
        }
    }
}