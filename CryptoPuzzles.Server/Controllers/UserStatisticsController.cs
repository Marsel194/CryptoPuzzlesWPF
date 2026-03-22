// ===== Server/Controllers/UserStatisticsController.cs (обновлённый с новыми полями) =====
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStatisticsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AUserStatistic>>> GetAll(
            [FromQuery] string? orderBy = "totalScore",
            [FromQuery] bool descending = true)
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new
                {
                    u.Id,
                    u.Login,
                    u.Username,
                    u.Email,
                    u.CreatedAt,
                    Statistic = u.Statistic != null ? new
                    {
                        u.Statistic.TotalSessions,
                        u.Statistic.TotalPuzzlesSolved,
                        u.Statistic.SolvedTrainingPuzzles,
                        u.Statistic.SolvedPracticePuzzles,
                        u.Statistic.TotalScore,
                        u.Statistic.TotalHintsUsed,
                        u.Statistic.AvgScorePerSession,
                        u.Statistic.LastActive
                    } : null
                })
                .ToListAsync();

            var result = users.Select(u => new AUserStatistic(
                u.Id,
                u.Login,
                u.Username,
                u.Email,
                u.Statistic?.TotalSessions ?? 0,
                u.Statistic?.TotalPuzzlesSolved ?? 0,
                u.Statistic?.SolvedTrainingPuzzles ?? 0,
                u.Statistic?.SolvedPracticePuzzles ?? 0,
                u.Statistic?.TotalScore ?? 0,
                u.Statistic?.TotalHintsUsed ?? 0,
                u.Statistic?.AvgScorePerSession ?? 0,
                u.Statistic?.LastActive,
                u.CreatedAt
            )).AsQueryable();

            result = orderBy?.ToLower() switch
            {
                "totalsessions" => descending
                    ? result.OrderByDescending(us => us.TotalSessions)
                    : result.OrderBy(us => us.TotalSessions),
                "totalpuzzlessolved" => descending
                    ? result.OrderByDescending(us => us.TotalPuzzlesSolved)
                    : result.OrderBy(us => us.TotalPuzzlesSolved),
                "totalscore" => descending
                    ? result.OrderByDescending(us => us.TotalScore)
                    : result.OrderBy(us => us.TotalScore),
                "totalhintsused" => descending
                    ? result.OrderByDescending(us => us.TotalHintsUsed)
                    : result.OrderBy(us => us.TotalHintsUsed),
                "lastactive" => descending
                    ? result.OrderByDescending(us => us.LastActive)
                    : result.OrderBy(us => us.LastActive),
                _ => descending
                    ? result.OrderByDescending(us => us.TotalScore)
                    : result.OrderBy(us => us.TotalScore)
            };

            return Ok(result.ToList());
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<AUserStatistic>> GetByUser(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Statistic)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
                return NotFound("Пользователь не найден");

            var stat = user.Statistic;
            var result = new AUserStatistic(
                user.Id,
                user.Login,
                user.Username,
                user.Email,
                stat?.TotalSessions ?? 0,
                stat?.TotalPuzzlesSolved ?? 0,
                stat?.SolvedTrainingPuzzles ?? 0,
                stat?.SolvedPracticePuzzles ?? 0,
                stat?.TotalScore ?? 0,
                stat?.TotalHintsUsed ?? 0,
                stat?.AvgScorePerSession ?? 0,
                stat?.LastActive,
                user.CreatedAt
            );

            return Ok(result);
        }

        [HttpGet("leaderboard")]
        public async Task<ActionResult<IEnumerable<AUserStatistic>>> GetLeaderboard(
            [FromQuery] int top = 10,
            [FromQuery] string criteria = "totalScore")
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new
                {
                    u.Id,
                    u.Login,
                    u.Username,
                    u.Email,
                    u.CreatedAt,
                    Statistic = u.Statistic != null ? new
                    {
                        u.Statistic.TotalSessions,
                        u.Statistic.TotalPuzzlesSolved,
                        u.Statistic.SolvedTrainingPuzzles,
                        u.Statistic.SolvedPracticePuzzles,
                        u.Statistic.TotalScore,
                        u.Statistic.TotalHintsUsed,
                        u.Statistic.AvgScorePerSession,
                        u.Statistic.LastActive
                    } : null
                })
                .ToListAsync();

            var result = users.Select(u => new AUserStatistic(
                u.Id,
                u.Login,
                u.Username,
                u.Email,
                u.Statistic?.TotalSessions ?? 0,
                u.Statistic?.TotalPuzzlesSolved ?? 0,
                u.Statistic?.SolvedTrainingPuzzles ?? 0,
                u.Statistic?.SolvedPracticePuzzles ?? 0,
                u.Statistic?.TotalScore ?? 0,
                u.Statistic?.TotalHintsUsed ?? 0,
                u.Statistic?.AvgScorePerSession ?? 0,
                u.Statistic?.LastActive,
                u.CreatedAt
            )).AsQueryable();

            result = criteria.ToLower() switch
            {
                "totalpuzzlessolved" => result.OrderByDescending(us => us.TotalPuzzlesSolved),
                "avgscore" => result.OrderByDescending(us => us.AvgScorePerSession),
                "totalsessions" => result.OrderByDescending(us => us.TotalSessions),
                _ => result.OrderByDescending(us => us.TotalScore)
            };

            var leaderboard = result
                .Take(top)
                .ToList();

            return Ok(leaderboard);
        }

        [HttpGet("user/{userId}/progress")]
        public async Task<ActionResult<object>> GetUserProgressOverTime(int userId)
        {
            var sessions = await _context.GameSessions
                .Include(s => s.Progresses)
                .Where(s => s.UserId == userId && !s.IsDeleted && s.CompletedAt.HasValue)
                .OrderBy(s => s.SessionStart)
                .Select(s => new
                {
                    Date = s.SessionStart,
                    Score = s.TotalScore,
                    PuzzlesSolved = s.Progresses.Count(p => p.Solved),
                    TotalPuzzles = s.Progresses.Count,
                    s.Duration
                })
                .ToListAsync();

            var cumulative = new List<object>();
            int totalSolved = 0;
            int totalScore = 0;

            foreach (var session in sessions)
            {
                totalSolved += session.PuzzlesSolved;
                totalScore += session.Score;
                cumulative.Add(new
                {
                    session.Date,
                    CumulativeSolved = totalSolved,
                    CumulativeScore = totalScore,
                    session.PuzzlesSolved,
                    session.Score,
                    session.TotalPuzzles,
                    session.Duration
                });
            }

            return Ok(new
            {
                Sessions = sessions,
                CumulativeProgress = cumulative,
                TotalSessions = sessions.Count,
                FirstSession = sessions.FirstOrDefault(),
                LastSession = sessions.LastOrDefault()
            });
        }

        [HttpPost("refresh/{userId}")]
        public async Task<IActionResult> RefreshStatistics(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
                return NotFound("Пользователь не найден");

            var sessions = await _context.GameSessions
                .Where(s => s.UserId == userId && !s.IsDeleted)
                .ToListAsync();

            var progress = await _context.SessionProgress
                .Include(sp => sp.Session)
                .Include(sp => sp.Puzzle)
                .Where(sp => sp.Session.UserId == userId && !sp.IsDeleted && !sp.Session.IsDeleted)
                .ToListAsync();

            var solvedPuzzles = progress.Count(sp => sp.Solved);
            var solvedTraining = progress.Count(sp => sp.Solved && sp.Puzzle.IsTraining);
            var solvedPractice = progress.Count(sp => sp.Solved && !sp.Puzzle.IsTraining);
            var totalScore = progress.Sum(sp => sp.ScoreEarned);
            var totalHints = progress.Sum(sp => sp.HintsUsed);
            var totalSessions = sessions.Count;
            var avgScore = totalSessions > 0 ? (decimal)totalScore / totalSessions : 0;
            var lastActive = sessions.Max(s => (DateTime?)s.SessionStart);

            var stats = await _context.UserStatistics.FindAsync(userId);
            if (stats == null)
            {
                stats = new UserStatistic
                {
                    UserId = userId,
                    TotalSessions = totalSessions,
                    TotalPuzzlesSolved = solvedPuzzles,
                    SolvedTrainingPuzzles = solvedTraining,
                    SolvedPracticePuzzles = solvedPractice,
                    TotalScore = totalScore,
                    TotalHintsUsed = totalHints,
                    AvgScorePerSession = avgScore,
                    LastActive = lastActive
                };
                _context.UserStatistics.Add(stats);
            }
            else
            {
                stats.TotalSessions = totalSessions;
                stats.TotalPuzzlesSolved = solvedPuzzles;
                stats.SolvedTrainingPuzzles = solvedTraining;
                stats.SolvedPracticePuzzles = solvedPractice;
                stats.TotalScore = totalScore;
                stats.TotalHintsUsed = totalHints;
                stats.AvgScorePerSession = avgScore;
                stats.LastActive = lastActive;
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Статистика обновлена" });
        }
    }
}