using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionProgressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SessionProgressController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<IEnumerable<ASessionProgress>>> GetBySession(int sessionId)
        {
            var results = await (from sp in _context.SessionProgress
                                 join s in _context.GameSessions on sp.SessionId equals s.Id
                                 join u in _context.Users on s.UserId equals u.Id
                                 join p in _context.Puzzles on sp.PuzzleId equals p.Id
                                 where sp.SessionId == sessionId && !sp.IsDeleted && !s.IsDeleted && !u.IsDeleted && !p.IsDeleted
                                 orderby sp.PuzzleOrder
                                 select new
                                 {
                                     SessionProgress = sp,
                                     UserLogin = u.Login,
                                     Username = u.Username,
                                     PuzzleTitle = p.Title
                                 }).ToListAsync();

            var progress = results.Select(x => new ASessionProgress(
                x.SessionProgress.Id,
                x.SessionProgress.SessionId,
                x.UserLogin,
                x.Username,
                x.SessionProgress.PuzzleId,
                x.PuzzleTitle,
                x.SessionProgress.PuzzleOrder,
                x.SessionProgress.Solved,
                x.SessionProgress.HintsUsed,
                x.SessionProgress.ScoreEarned,
                x.SessionProgress.StartedAt,
                x.SessionProgress.SolvedAt,
                x.SessionProgress.TimeToSolve,
                x.SessionProgress.IsDeleted,
                x.SessionProgress.DeletedAt
            )).ToList();

            return Ok(progress);
        }

        [HttpGet("user/{userId}/statistics")]
        public async Task<ActionResult<object>> GetUserPuzzleStatistics(int userId)
        {
            var statistics = await _context.SessionProgress
                .Include(sp => sp.Session)
                .Where(sp => sp.Session.UserId == userId && !sp.IsDeleted && !sp.Session.IsDeleted)
                .GroupBy(sp => 1)
                .Select(g => new
                {
                    TotalPuzzles = g.Count(),
                    SolvedPuzzles = g.Count(sp => sp.Solved),
                    TotalScore = g.Sum(sp => sp.ScoreEarned),
                    TotalHints = g.Sum(sp => sp.HintsUsed),
                    AverageTimePerPuzzle = g.Where(sp => sp.SolvedAt != null)
                        .Average(sp => ((DateTime)sp.SolvedAt! - sp.StartedAt).TotalSeconds),
                    SuccessRate = g.Count() > 0
                        ? (double)g.Count(sp => sp.Solved) / g.Count() * 100
                        : 0
                })
                .FirstOrDefaultAsync();

            if (statistics == null)
            {
                return Ok(new
                {
                    TotalPuzzles = 0,
                    SolvedPuzzles = 0,
                    TotalScore = 0,
                    TotalHints = 0,
                    AverageTimePerPuzzle = 0,
                    SuccessRate = 0
                });
            }

            return Ok(statistics);
        }

        [HttpPost]
        public async Task<ActionResult<ASessionProgress>> Create([FromBody] ASessionProgressCreate dto)
        {
            var session = await _context.GameSessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == dto.SessionId && !s.IsDeleted);

            if (session == null)
                return BadRequest("Сессия не найдена");

            var puzzle = await _context.Puzzles
                .FirstOrDefaultAsync(p => p.Id == dto.PuzzleId && !p.IsDeleted);

            if (puzzle == null)
                return BadRequest("Головоломка не найдена");

            var exists = await _context.SessionProgress
                .AnyAsync(sp => sp.SessionId == dto.SessionId && sp.PuzzleId == dto.PuzzleId && !sp.IsDeleted);

            if (exists)
                return Conflict("Эта головоломка уже добавлена в сессию");

            var progress = new SessionProgress
            {
                SessionId = dto.SessionId,
                PuzzleId = dto.PuzzleId,
                PuzzleOrder = dto.PuzzleOrder,
                HintsUsed = dto.HintsUsed,
                ScoreEarned = dto.ScoreEarned,
                Solved = false,
                StartedAt = DateTime.UtcNow
            };

            _context.SessionProgress.Add(progress);
            await _context.SaveChangesAsync();

            await UpdateUserStatistics(session.UserId);

            var result = new ASessionProgress(
                progress.Id,
                progress.SessionId,
                session.User.Login,
                session.User.Username,
                progress.PuzzleId,
                puzzle.Title,
                progress.PuzzleOrder,
                progress.Solved,
                progress.HintsUsed,
                progress.ScoreEarned,
                progress.StartedAt,
                progress.SolvedAt,
                progress.TimeToSolve,
                progress.IsDeleted,
                progress.DeletedAt
            );

            return CreatedAtAction(nameof(Get), new { id = progress.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ASessionProgressUpdate dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var progress = await _context.SessionProgress
                .Include(sp => sp.Session)
                .FirstOrDefaultAsync(sp => sp.Id == id && !sp.IsDeleted);

            if (progress == null)
                return NotFound();

            if (dto.HintsUsed.HasValue)
                progress.HintsUsed = dto.HintsUsed.Value;

            if (dto.ScoreEarned.HasValue)
                progress.ScoreEarned = dto.ScoreEarned.Value;

            if (dto.Solved.HasValue && dto.Solved.Value && !progress.Solved)
            {
                progress.Solved = true;
                progress.SolvedAt = dto.SolvedAt ?? DateTime.UtcNow;

                var session = await _context.GameSessions.FindAsync(progress.SessionId);
                if (session != null)
                    session.TotalScore += progress.ScoreEarned;
            }

            if (dto.IsDeleted.HasValue)
            {
                progress.IsDeleted = dto.IsDeleted.Value;
                progress.DeletedAt = dto.IsDeleted.Value ? DateTime.UtcNow : null;
            }

            await _context.SaveChangesAsync();
            await UpdateUserStatistics(progress.Session.UserId);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ASessionProgress>> Get(int id)
        {
            var result = await (from sp in _context.SessionProgress
                                join s in _context.GameSessions on sp.SessionId equals s.Id
                                join u in _context.Users on s.UserId equals u.Id
                                join p in _context.Puzzles on sp.PuzzleId equals p.Id
                                where sp.Id == id && !sp.IsDeleted && !s.IsDeleted && !u.IsDeleted && !p.IsDeleted
                                select new
                                {
                                    SessionProgress = sp,
                                    UserLogin = u.Login,
                                    Username = u.Username,
                                    PuzzleTitle = p.Title
                                }).FirstOrDefaultAsync();

            if (result == null)
                return NotFound();

            var progress = new ASessionProgress(
                result.SessionProgress.Id,
                result.SessionProgress.SessionId,
                result.UserLogin,
                result.Username,
                result.SessionProgress.PuzzleId,
                result.PuzzleTitle,
                result.SessionProgress.PuzzleOrder,
                result.SessionProgress.Solved,
                result.SessionProgress.HintsUsed,
                result.SessionProgress.ScoreEarned,
                result.SessionProgress.StartedAt,
                result.SessionProgress.SolvedAt,
                result.SessionProgress.TimeToSolve,
                result.SessionProgress.IsDeleted,
                result.SessionProgress.DeletedAt
            );

            return Ok(progress);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ASessionProgress>>> GetAll(
            [FromQuery] int? userId = null,
            [FromQuery] int? sessionId = null,
            [FromQuery] bool? solved = null)
        {
            var query = from sp in _context.SessionProgress
                        join s in _context.GameSessions on sp.SessionId equals s.Id
                        join u in _context.Users on s.UserId equals u.Id
                        join p in _context.Puzzles on sp.PuzzleId equals p.Id
                        where !sp.IsDeleted && !s.IsDeleted && !u.IsDeleted && !p.IsDeleted
                        select new
                        {
                            SessionProgress = sp,
                            UserLogin = u.Login,
                            Username = u.Username,
                            PuzzleTitle = p.Title
                        };

            if (userId.HasValue)
                query = query.Where(x => x.SessionProgress.Session.UserId == userId.Value);

            if (sessionId.HasValue)
                query = query.Where(x => x.SessionProgress.SessionId == sessionId.Value);

            if (solved.HasValue)
                query = query.Where(x => x.SessionProgress.Solved == solved.Value);

            var results = await query
                .OrderByDescending(x => x.SessionProgress.StartedAt)
                .ToListAsync();

            var progress = results.Select(x => new ASessionProgress(
                x.SessionProgress.Id,
                x.SessionProgress.SessionId,
                x.UserLogin,
                x.Username,
                x.SessionProgress.PuzzleId,
                x.PuzzleTitle,
                x.SessionProgress.PuzzleOrder,
                x.SessionProgress.Solved,
                x.SessionProgress.HintsUsed,
                x.SessionProgress.ScoreEarned,
                x.SessionProgress.StartedAt,
                x.SessionProgress.SolvedAt,
                x.SessionProgress.TimeToSolve,
                x.SessionProgress.IsDeleted,
                x.SessionProgress.DeletedAt
            )).ToList();

            return Ok(progress);
        }

        private async Task UpdateUserStatistics(int userId)
        {
            var stats = await _context.UserStatistics.FindAsync(userId);

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

            if (lastActive.HasValue && lastActive.Value.Kind == DateTimeKind.Unspecified)
                lastActive = DateTime.SpecifyKind(lastActive.Value, DateTimeKind.Utc);

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
        }
    }
}