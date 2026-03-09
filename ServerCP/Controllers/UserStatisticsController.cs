using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStatisticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserStatisticsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AUserStatistic>>> GetAll(
            [FromQuery] string? orderBy = "totalScore",
            [FromQuery] bool descending = true)
        {
            var query = _context.UserStatistics
                .Include(us => us.User)
                .Where(us => !us.User.IsDeleted)
                .Select(us => new AUserStatistic(
                    us.UserId,
                    us.User.Login,
                    us.User.Username,
                    us.User.Email,
                    us.TotalSessions,
                    us.TotalPuzzlesSolved,
                    us.TotalScore,
                    us.TotalHintsUsed,
                    us.AvgScorePerSession,
                    us.LastActive,
                    us.User.CreatedAt
                ));

            query = orderBy?.ToLower() switch
            {
                "totalsessions" => descending
                    ? query.OrderByDescending(us => us.TotalSessions)
                    : query.OrderBy(us => us.TotalSessions),
                "totalpuzzlessolved" => descending
                    ? query.OrderByDescending(us => us.TotalPuzzlesSolved)
                    : query.OrderBy(us => us.TotalPuzzlesSolved),
                "totalscore" => descending
                    ? query.OrderByDescending(us => us.TotalScore)
                    : query.OrderBy(us => us.TotalScore),
                "totalhintsused" => descending
                    ? query.OrderByDescending(us => us.TotalHintsUsed)
                    : query.OrderBy(us => us.TotalHintsUsed),
                "lastactive" => descending
                    ? query.OrderByDescending(us => us.LastActive)
                    : query.OrderBy(us => us.LastActive),
                _ => descending
                    ? query.OrderByDescending(us => us.TotalScore)
                    : query.OrderBy(us => us.TotalScore)
            };

            query = query.OrderByDescending(us => us.TotalScore);
            return Ok(await query.ToListAsync());
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<AUserStatistic>> GetByUser(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Statistic)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
                return NotFound("Пользователь не найден");

            if (user.Statistic == null)
            {
                return Ok(new AUserStatistic(
                    user.Id,
                    user.Login,
                    user.Username,
                    user.Email,
                    0, 0, 0, 0, 0, null, user.CreatedAt
                ));
            }

            var stat = user.Statistic;
            var result = new AUserStatistic(
                stat.UserId,
                user.Login,
                user.Username,
                user.Email,
                stat.TotalSessions,
                stat.TotalPuzzlesSolved,
                stat.TotalScore,
                stat.TotalHintsUsed,
                stat.AvgScorePerSession,
                stat.LastActive,
                user.CreatedAt
            );

            return Ok(result);
        }

        [HttpGet("leaderboard")]
        public async Task<ActionResult<IEnumerable<AUserStatistic>>> GetLeaderboard(
            [FromQuery] int top = 10,
            [FromQuery] string criteria = "totalScore")
        {
            var query = _context.UserStatistics
                .Include(us => us.User)
                .Where(us => !us.User.IsDeleted);

            query = criteria.ToLower() switch
            {
                "totalpuzzlessolved" => query.OrderByDescending(us => us.TotalPuzzlesSolved),
                "avgscore" => query.OrderByDescending(us => us.AvgScorePerSession),
                "totalsessions" => query.OrderByDescending(us => us.TotalSessions),
                _ => query.OrderByDescending(us => us.TotalScore)
            };

            var leaderboard = await query
                .Take(top)
                .Select(us => new AUserStatistic(
                    us.UserId,
                    us.User.Login,
                    us.User.Username,
                    us.User.Email,
                    us.TotalSessions,
                    us.TotalPuzzlesSolved,
                    us.TotalScore,
                    us.TotalHintsUsed,
                    us.AvgScorePerSession,
                    us.LastActive,
                    us.User.CreatedAt
                ))
                .ToListAsync();

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
                    Duration = s.Duration
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
                .Where(sp => sp.Session.UserId == userId && !sp.IsDeleted && !sp.Session.IsDeleted)
                .ToListAsync();

            var solvedPuzzles = progress.Count(sp => sp.Solved);
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