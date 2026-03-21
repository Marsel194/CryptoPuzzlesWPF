using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;
using System.Security.Claims;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SessionProgressController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionProgressController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private bool IsAdmin()
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;
            return 0;
        }

        private ASessionProgress MapToDto(SessionProgress progress)
        {
            return new ASessionProgress(
                progress.Id,
                progress.SessionId,
                progress.Session?.User?.Login ?? string.Empty,
                progress.Session?.User?.Username ?? string.Empty,
                progress.PuzzleId,
                progress.Puzzle?.Title ?? string.Empty,
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
        }

        private SessionProgress MapToEntity(ASessionProgressCreate dto)
        {
            return new SessionProgress
            {
                SessionId = dto.SessionId,
                PuzzleId = dto.PuzzleId,
                PuzzleOrder = dto.PuzzleOrder,
                HintsUsed = dto.HintsUsed,
                ScoreEarned = dto.ScoreEarned,
                StartedAt = DateTime.UtcNow,
                Solved = false,
                IsDeleted = false
            };
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<IEnumerable<ASessionProgress>>> GetBySession(int sessionId)
        {
            var query = _context.SessionProgress
                .Include(sp => sp.Session).ThenInclude(s => s.User)
                .Include(sp => sp.Puzzle)
                .Where(sp => sp.SessionId == sessionId && !sp.IsDeleted);

            if (!IsAdmin())
            {
                query = query.Where(sp => !sp.Session.IsDeleted);
                var session = await _context.GameSessions.FindAsync(sessionId);
                if (session?.UserId != GetCurrentUserId())
                    return Forbid();
            }

            var progress = await query
                .OrderBy(sp => sp.PuzzleOrder)
                .ToListAsync();

            return Ok(progress.Select(MapToDto));
        }

        [HttpGet("user/{userId}/statistics")]
        public async Task<ActionResult<object>> GetUserPuzzleStatistics(int userId)
        {
            var query = _context.SessionProgress
                .Include(sp => sp.Session)
                .Where(sp => sp.Session.UserId == userId && !sp.IsDeleted && !sp.Session.IsDeleted);

            if (!IsAdmin() && userId != GetCurrentUserId())
                return Forbid();

            var statistics = await query
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

            if (!IsAdmin() && session.IsDeleted)
                return BadRequest("Нельзя добавлять прогресс в удалённую сессию");

            var puzzle = await _context.Puzzles
                .FirstOrDefaultAsync(p => p.Id == dto.PuzzleId && !p.IsDeleted);

            if (puzzle == null)
                return BadRequest("Головоломка не найдена");

            var exists = await _context.SessionProgress
                .AnyAsync(sp => sp.SessionId == dto.SessionId && sp.PuzzleId == dto.PuzzleId && !sp.IsDeleted);

            if (exists)
                return Conflict("Эта головоломка уже добавлена в сессию");

            var progress = MapToEntity(dto);
            _context.SessionProgress.Add(progress);
            await _context.SaveChangesAsync();

            await UpdateUserStatistics(session.UserId);

            await _context.Entry(progress).Reference(p => p.Session).LoadAsync();
            await _context.Entry(progress).Reference(p => p.Puzzle).LoadAsync();

            return CreatedAtAction(nameof(Get), new { id = progress.Id }, MapToDto(progress));
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

            if (!IsAdmin() && progress.Session.IsDeleted)
                return BadRequest("Нельзя обновлять прогресс в удалённой сессии.");

            if (dto.HintsUsed.HasValue)
                progress.HintsUsed = dto.HintsUsed.Value;

            if (dto.ScoreEarned.HasValue)
                progress.ScoreEarned = dto.ScoreEarned.Value;

            if (dto.Solved.HasValue && dto.Solved.Value && !progress.Solved)
            {
                progress.Solved = true;
                var solvedAt = dto.SolvedAt ?? DateTime.UtcNow;
                if (solvedAt.Kind != DateTimeKind.Utc)
                    solvedAt = DateTime.SpecifyKind(solvedAt, DateTimeKind.Utc);
                progress.SolvedAt = solvedAt;
                progress.TimeToSolve = progress.SolvedAt.Value - progress.StartedAt;

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
            var progress = await _context.SessionProgress
                .Include(sp => sp.Session).ThenInclude(s => s.User)
                .Include(sp => sp.Puzzle)
                .FirstOrDefaultAsync(sp => sp.Id == id && !sp.IsDeleted);

            if (progress == null)
                return NotFound();

            if (!IsAdmin() && progress.Session.IsDeleted)
                return NotFound();

            return Ok(MapToDto(progress));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ASessionProgress>>> GetAll(
            [FromQuery] int? userId = null,
            [FromQuery] int? sessionId = null,
            [FromQuery] bool? solved = null)
        {
            var query = _context.SessionProgress
                .Include(sp => sp.Session).ThenInclude(s => s.User)
                .Include(sp => sp.Puzzle)
                .Where(sp => !sp.IsDeleted);

            if (!IsAdmin())
                query = query.Where(sp => !sp.Session.IsDeleted);

            if (userId.HasValue)
                query = query.Where(sp => sp.Session.UserId == userId.Value);

            if (sessionId.HasValue)
                query = query.Where(sp => sp.SessionId == sessionId.Value);

            if (solved.HasValue)
                query = query.Where(sp => sp.Solved == solved.Value);

            var progress = await query
                .OrderByDescending(sp => sp.StartedAt)
                .ToListAsync();

            return Ok(progress.Select(MapToDto));
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

            int solvedTraining = progress.Count(sp => sp.Solved && sp.Puzzle != null && sp.Puzzle.IsTraining);
            int solvedPractice = progress.Count(sp => sp.Solved && sp.Puzzle != null && !sp.Puzzle.IsTraining);
            int solvedPuzzles = solvedTraining + solvedPractice;
            int totalScore = progress.Sum(sp => sp.ScoreEarned);
            int totalHints = progress.Sum(sp => sp.HintsUsed);
            int totalSessions = sessions.Count;
            decimal avgScore = totalSessions > 0 ? (decimal)totalScore / totalSessions : 0;
            DateTime? lastActive = sessions.Max(s => (DateTime?)s.SessionStart);

            if (lastActive.HasValue && lastActive.Value.Kind != DateTimeKind.Utc)
                lastActive = DateTime.SpecifyKind(lastActive.Value, DateTimeKind.Utc);

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
        }
    }
}