using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Models;

namespace CryptoPuzzles.Server.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<EncryptionMethod> EncryptionMethods { get; set; }
        public DbSet<Puzzle> Puzzles { get; set; }
        public DbSet<Hint> Hints { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<Tutorial> Tutorials { get; set; }

        public DbSet<SessionProgress> SessionProgress { get; set; }
        public DbSet<UserStatistic> UserStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.Property(u => u.Id).HasColumnName("id");
                entity.Property(u => u.Login).HasColumnName("login");
                entity.Property(u => u.Email).HasColumnName("email");
                entity.Property(u => u.Username).HasColumnName("username");
                entity.Property(u => u.PasswordHash).HasColumnName("password_hash");
                entity.Property(u => u.CreatedAt).HasColumnName("created_at");
                entity.Property(u => u.IsDeleted).HasColumnName("is_deleted");
                entity.Property(u => u.DeletedAt).HasColumnName("deleted_at");

                entity.HasIndex(u => u.Login).IsUnique().HasDatabaseName("idx_users_login");
                entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("idx_users_email");
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admins");
                entity.Property(a => a.Id).HasColumnName("id");
                entity.Property(a => a.Login).HasColumnName("login");
                entity.Property(a => a.CreatedAt).HasColumnName("created_at");
                entity.Property(a => a.PasswordHash).HasColumnName("password_hash");
                entity.Property(a => a.FirstName).HasColumnName("first_name");
                entity.Property(a => a.LastName).HasColumnName("last_name");
                entity.Property(a => a.MiddleName).HasColumnName("middle_name");
                entity.Property(a => a.IsDeleted).HasColumnName("is_deleted");
                entity.Property(a => a.DeletedAt).HasColumnName("deleted_at");
                entity.HasIndex(a => a.Login).IsUnique().HasDatabaseName("idx_admins_login");
            });

            modelBuilder.Entity<Difficulty>(entity =>
            {
                entity.ToTable("difficulties");
                entity.Property(d => d.Id).HasColumnName("id");
                entity.Property(d => d.DifficultyName).HasColumnName("difficulty").IsRequired();
                entity.Property(d => d.IsDeleted).HasColumnName("is_deleted");
                entity.Property(d => d.DeletedAt).HasColumnName("deleted_at");
                entity.HasIndex(d => d.DifficultyName).IsUnique().HasDatabaseName("idx_difficulties_name");
            });

            modelBuilder.Entity<EncryptionMethod>(entity =>
            {
                entity.ToTable("encryption_methods");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
                entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("idx_methods_name");
            });

            modelBuilder.Entity<Puzzle>(entity =>
            {
                entity.ToTable("puzzles");
                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.Title).HasColumnName("title").IsRequired();
                entity.Property(p => p.Content).HasColumnName("content").IsRequired();
                entity.Property(p => p.Answer).HasColumnName("answer").IsRequired();
                entity.Property(p => p.MaxScore).HasColumnName("max_score");
                entity.Property(p => p.DifficultyId).HasColumnName("difficulty_id");
                entity.Property(p => p.MethodId).HasColumnName("method_id");
                entity.Property(p => p.IsTraining).HasColumnName("is_training");
                entity.Property(p => p.TutorialOrder).HasColumnName("tutorial_order");
                entity.Property(p => p.CreatedByAdminId).HasColumnName("created_by_admin_id");
                entity.Property(p => p.CreatedAt).HasColumnName("created_at");
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

            modelBuilder.Entity<Hint>(entity =>
            {
                entity.ToTable("hints");
                entity.Property(h => h.Id).HasColumnName("id");
                entity.Property(h => h.PuzzleId).HasColumnName("puzzle_id");
                entity.Property(h => h.HintText).HasColumnName("hint_text").IsRequired();
                entity.Property(h => h.HintOrder).HasColumnName("hint_order");
                entity.Property(h => h.CreatedAt).HasColumnName("created_at");
                entity.Property(h => h.IsDeleted).HasColumnName("is_deleted");
                entity.Property(h => h.DeletedAt).HasColumnName("deleted_at");

                entity.HasOne(h => h.Puzzle)
                      .WithMany(p => p.Hints)
                      .HasForeignKey(h => h.PuzzleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(h => new { h.PuzzleId, h.HintOrder }).HasDatabaseName("idx_hints_puzzle");
            });

            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.ToTable("game_sessions");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.SessionType)
                    .HasColumnName("session_type")
                    .HasMaxLength(20);

                entity.Property(e => e.TotalScore)
                    .HasColumnName("total_score");

                entity.Property(e => e.SessionStart)
                    .HasColumnName("session_start");

                entity.Property(e => e.CompletedAt)
                    .HasColumnName("completed_at");

                entity.Property(e => e.IsCompleted)
                    .HasColumnName("is_completed");

                entity.Property(e => e.CurrentTutorialIndex)
                    .HasColumnName("current_tutorial_index");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted");

                entity.Property(e => e.DeletedAt)
                    .HasColumnName("deleted_at");
                entity.Ignore(e => e.PuzzlesCount);
                entity.Ignore(e => e.SolvedCount);
                entity.Ignore(e => e.Duration);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.GameSessions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Progresses)
                    .WithOne(p => p.Session)
                    .HasForeignKey(p => p.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.SessionStart })
                    .HasDatabaseName("idx_game_sessions_user");
                entity.HasIndex(e => e.CompletedAt)
                    .HasDatabaseName("idx_game_sessions_completed");
                entity.HasIndex(e => e.SessionType)
                    .HasDatabaseName("idx_game_sessions_type");
            });

            modelBuilder.Entity<SessionProgress>(entity =>
            {
                entity.ToTable("session_progress");
                entity.Property(sp => sp.Id).HasColumnName("id");
                entity.Property(sp => sp.SessionId).HasColumnName("session_id");
                entity.Property(sp => sp.PuzzleId).HasColumnName("puzzle_id");
                entity.Property(sp => sp.PuzzleOrder).HasColumnName("puzzle_order");
                entity.Property(sp => sp.Solved).HasColumnName("solved");
                entity.Property(sp => sp.HintsUsed).HasColumnName("hints_used");
                entity.Property(sp => sp.ScoreEarned).HasColumnName("score_earned");
                entity.Property(sp => sp.StartedAt).HasColumnName("started_at");
                entity.Property(sp => sp.TimeToSolve).HasColumnName("time_to_solve");
                entity.Property(sp => sp.SolvedAt).HasColumnName("solved_at");
                entity.Property(sp => sp.IsDeleted).HasColumnName("is_deleted");
                entity.Property(sp => sp.DeletedAt).HasColumnName("deleted_at");

                entity.HasOne(sp => sp.Session)
                      .WithMany(s => s.Progresses)
                      .HasForeignKey(sp => sp.SessionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sp => sp.Puzzle)
                      .WithMany(p => p.SessionProgresses)
                      .HasForeignKey(sp => sp.PuzzleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(sp => sp.SessionId).HasDatabaseName("idx_session_progress_session");
                entity.HasIndex(sp => sp.PuzzleId).HasDatabaseName("idx_session_progress_puzzle");
                entity.HasIndex(sp => sp.Solved).HasDatabaseName("idx_session_progress_solved");

                entity.HasIndex(sp => new { sp.SessionId, sp.PuzzleId })
                      .IsUnique()
                      .HasDatabaseName("idx_session_progress_unique");
            });

            modelBuilder.Entity<UserStatistic>(entity =>
            {
                entity.ToTable("user_statistics");
                entity.HasKey(us => us.UserId);
                entity.Property(us => us.UserId).HasColumnName("user_id");
                entity.Property(us => us.TotalSessions).HasColumnName("total_sessions");
                entity.Property(us => us.TotalPuzzlesSolved).HasColumnName("total_puzzles_solved");
                entity.Property(us => us.SolvedTrainingPuzzles).HasColumnName("solved_training_puzzles");
                entity.Property(us => us.SolvedPracticePuzzles).HasColumnName("solved_practice_puzzles");
                entity.Property(us => us.TotalScore).HasColumnName("total_score");
                entity.Property(us => us.TotalHintsUsed).HasColumnName("total_hints_used");
                entity.Property(us => us.AvgScorePerSession).HasColumnName("avg_score_per_session").HasPrecision(5, 2);
                entity.Property(us => us.LastActive).HasColumnName("last_active");

                entity.HasOne(us => us.User)
                      .WithOne(u => u.Statistic)
                      .HasForeignKey<UserStatistic>(us => us.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(us => us.TotalScore).HasDatabaseName("idx_user_stats_score");
                entity.HasIndex(us => us.TotalPuzzlesSolved).HasDatabaseName("idx_user_stats_solved");
                entity.HasIndex(us => us.LastActive).HasDatabaseName("idx_user_stats_active");
            });

            modelBuilder.Entity<Tutorial>(entity =>
            {
                entity.ToTable("tutorials");
                entity.Property(t => t.Id).HasColumnName("id");
                entity.Property(t => t.MethodId).HasColumnName("method_id");
                entity.Property(t => t.TheoryTitle).HasColumnName("theory_title").IsRequired();
                entity.Property(t => t.TheoryContent).HasColumnName("theory_content").IsRequired();
                entity.Property(t => t.SortOrder).HasColumnName("sort_order");
                entity.Property(t => t.CreatedAt).HasColumnName("created_at");
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