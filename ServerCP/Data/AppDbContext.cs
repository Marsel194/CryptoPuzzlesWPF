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

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToLower());
                }
            }

            // Пользователи
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.Property(u => u.Id).HasColumnName("id");
                entity.Property(u => u.Login).HasColumnName("login");
                entity.Property(u => u.Email).HasColumnName("email");
                entity.Property(u => u.Username).HasColumnName("username");
                entity.Property(u => u.PasswordHash).HasColumnName("password_hash");
                entity.Property(u => u.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(u => u.IsDeleted).HasColumnName("is_deleted");
                entity.Property(u => u.DeletedAt).HasColumnName("deleted_at");

                entity.HasIndex(u => u.Login).IsUnique().HasDatabaseName("idx_users_login");
                entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("idx_users_email");
            });

            // Администраторы
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admins");
                entity.Property(a => a.Id).HasColumnName("id");
                entity.Property(a => a.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(a => a.PasswordHash).HasColumnName("password_hash");
                entity.Property(a => a.FirstName).HasColumnName("first_name");
                entity.Property(a => a.LastName).HasColumnName("last_name");
                entity.Property(a => a.MiddleName).HasColumnName("middle_name");
                entity.Property(a => a.IsDeleted).HasColumnName("is_deleted");
                entity.Property(a => a.DeletedAt).HasColumnName("deleted_at");
                entity.HasIndex(a => a.Login).IsUnique().HasDatabaseName("idx_admins_login");
            });

            // Сложности
            modelBuilder.Entity<Difficulty>(entity =>
            {
                entity.ToTable("difficulties");
                entity.Property(d => d.Id).HasColumnName("id");
                entity.Property(d => d.DifficultyName).HasColumnName("difficulty").IsRequired();
                entity.Property(d => d.IsDeleted).HasColumnName("is_deleted");
                entity.Property(d => d.DeletedAt).HasColumnName("deleted_at");
                entity.HasIndex(d => d.DifficultyName).IsUnique().HasDatabaseName("idx_difficulties_name");
            });

            // Методы шифрования
            modelBuilder.Entity<EncryptionMethod>(entity =>
            {
                entity.ToTable("encryption_methods");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("idx_methods_name");
            });

            // Головоломки
            modelBuilder.Entity<Puzzle>(entity =>
            {
                entity.ToTable("puzzles");
                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.Title).HasColumnName("title").IsRequired();
                entity.Property(p => p.Content).HasColumnName("content").IsRequired();
                entity.Property(p => p.Answer).HasColumnName("answer").IsRequired();
                entity.Property(p => p.MaxScore).HasColumnName("max_score").HasDefaultValue(50);
                entity.Property(p => p.DifficultyId).HasColumnName("difficulty_id");
                entity.Property(p => p.MethodId).HasColumnName("method_id");
                entity.Property(p => p.IsTraining).HasColumnName("is_training");
                entity.Property(p => p.TutorialOrder).HasColumnName("tutorial_order");
                entity.Property(p => p.CreatedByAdminId).HasColumnName("created_by_admin_id");
                entity.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(p => p.IsDeleted).HasColumnName("is_deleted");
                entity.Property(p => p.DeletedAt).HasColumnName("deleted_at");

                entity.HasOne(p => p.Difficulty)
                      .WithMany(d => d.Puzzles)
                      .HasForeignKey(p => p.DifficultyId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Method)
                      .WithMany(m => m.Puzzles)
                      .HasForeignKey(p => p.MethodId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.CreatedByAdmin)
                      .WithMany(a => a.CreatedPuzzles)
                      .HasForeignKey(p => p.CreatedByAdminId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(p => p.DifficultyId).HasDatabaseName("idx_puzzles_difficulty");
                entity.HasIndex(p => p.MethodId).HasDatabaseName("idx_puzzles_method");
                entity.HasIndex(p => p.CreatedByAdminId).HasDatabaseName("idx_puzzles_created_by");
                entity.HasIndex(p => new { p.IsTraining, p.TutorialOrder }).HasDatabaseName("idx_puzzles_training");
            });

            // Подсказки
            modelBuilder.Entity<Hint>(entity =>
            {
                entity.ToTable("hints");
                entity.Property(h => h.Id).HasColumnName("id");
                entity.Property(h => h.PuzzleId).HasColumnName("puzzle_id");
                entity.Property(h => h.HintText).HasColumnName("hint_text").IsRequired();
                entity.Property(h => h.HintOrder).HasColumnName("hint_order").HasDefaultValue(1);
                entity.Property(h => h.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(h => h.IsDeleted).HasColumnName("is_deleted");
                entity.Property(h => h.DeletedAt).HasColumnName("deleted_at");

                entity.HasOne(h => h.Puzzle)
                      .WithMany(p => p.Hints)
                      .HasForeignKey(h => h.PuzzleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(h => new { h.PuzzleId, h.HintOrder }).HasDatabaseName("idx_hints_puzzle");
            });

            // Игровые сессии
            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.ToTable("game_sessions");
                entity.Property(g => g.Id).HasColumnName("id");
                entity.Property(g => g.UserId).HasColumnName("user_id");
                entity.Property(g => g.Score).HasColumnName("score").HasDefaultValue(0);
                entity.Property(g => g.SessionStartTime).HasColumnName("session_start_time").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(g => g.CurrentPuzzleId).HasColumnName("current_puzzle_id");
                entity.Property(g => g.TrainingCompleted).HasColumnName("training_completed");
                entity.Property(g => g.HintsUsed).HasColumnName("hints_used");
                entity.Property(g => g.CompletedAt).HasColumnName("completed_at");
                entity.Property(g => g.IsDeleted).HasColumnName("is_deleted");
                entity.Property(g => g.DeletedAt).HasColumnName("deleted_at");

                entity.HasOne(g => g.User)
                      .WithMany(u => u.GameSessions)
                      .HasForeignKey(g => g.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(g => g.CurrentPuzzle)
                      .WithMany(p => p.GameSessions)
                      .HasForeignKey(g => g.CurrentPuzzleId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(g => new { g.UserId, g.SessionStartTime }).HasDatabaseName("idx_game_sessions_user");
                entity.HasIndex(g => g.CurrentPuzzleId).HasDatabaseName("idx_game_sessions_puzzle");
                entity.HasIndex(g => g.CompletedAt).HasDatabaseName("idx_game_sessions_completed");
            });

            // Туториалы
            modelBuilder.Entity<Tutorial>(entity =>
            {
                entity.ToTable("tutorials");
                entity.Property(t => t.Id).HasColumnName("id");
                entity.Property(t => t.MethodId).HasColumnName("method_id");
                entity.Property(t => t.TheoryTitle).HasColumnName("theory_title").IsRequired();
                entity.Property(t => t.TheoryContent).HasColumnName("theory_content").IsRequired();
                entity.Property(t => t.SortOrder).HasColumnName("sort_order");
                entity.Property(t => t.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(t => t.IsDeleted).HasColumnName("is_deleted");
                entity.Property(t => t.DeletedAt).HasColumnName("deleted_at");

                entity.HasOne(t => t.Method)
                      .WithMany(m => m.Tutorials)
                      .HasForeignKey(t => t.MethodId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => t.MethodId).HasDatabaseName("idx_tutorials_method");
            });
        }
    }
}