using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameSessionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GameSessionsController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AGameSession>>> GetAll([FromQuery] bool includeDeleted = false)
        {
            var query = _context.GameSessions
                .Include(s => s.User)
                .Include(s => s.Progresses)
                .AsQueryable();

            if (!includeDeleted)
                query = query.Where(s => !s.IsDeleted);

            var sessions = await query
                .OrderByDescending(s => s.SessionStart)
                .Select(s => new AGameSession(
                    s.Id,
                    s.UserId,
                    s.User.Login,
                    s.User.Username,
                    s.SessionType,
                    s.TotalScore,
                    s.SessionStart,
                    s.CompletedAt,
                    s.IsCompleted,
                    s.Progresses.Count,
                    s.Progresses.Count(p => p.Solved),
                    s.CurrentTutorialIndex,
                    s.IsDeleted,
                    s.DeletedAt
                ))
                .ToListAsync();
            return Ok(sessions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AGameSession>> Get(int id, [FromQuery] bool includeDeleted = false)
        {
            var query = _context.GameSessions
                .Include(s => s.User)
                .Include(s => s.Progresses)
                .Where(s => s.Id == id);

            if (!includeDeleted)
                query = query.Where(s => !s.IsDeleted);

            var session = await query
                .Select(s => new AGameSession(
                    s.Id,
                    s.UserId,
                    s.User.Login,
                    s.User.Username,
                    s.SessionType,
                    s.TotalScore,
                    s.SessionStart,
                    s.CompletedAt,
                    s.IsCompleted,
                    s.Progresses.Count,
                    s.Progresses.Count(p => p.Solved),
                    s.CurrentTutorialIndex,
                    s.IsDeleted,
                    s.DeletedAt
                ))
                .FirstOrDefaultAsync();

            if (session == null) return NotFound();
            return Ok(session);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AGameSession>>> GetByUser(int userId, [FromQuery] bool includeDeleted = false)
        {
            var query = _context.GameSessions
                .Include(s => s.User)
                .Include(s => s.Progresses)
                .Where(s => s.UserId == userId);

            if (!includeDeleted)
                query = query.Where(s => !s.IsDeleted);

            var sessions = await query
                .OrderByDescending(s => s.SessionStart)
                .Select(s => new AGameSession(
                    s.Id,
                    s.UserId,
                    s.User.Login,
                    s.User.Username,
                    s.SessionType,
                    s.TotalScore,
                    s.SessionStart,
                    s.CompletedAt,
                    s.IsCompleted,
                    s.Progresses.Count,
                    s.Progresses.Count(p => p.Solved),
                    s.CurrentTutorialIndex,
                    s.IsDeleted,
                    s.DeletedAt
                ))
                .ToListAsync();
            return Ok(sessions);
        }

        [HttpPost]
        public async Task<ActionResult<AGameSession>> Create([FromBody] AGameSessionCreate dto)
        {
            var session = new GameSession
            {
                UserId = dto.UserId,
                SessionType = dto.SessionType,
                TotalScore = dto.TotalScore,
                SessionStart = DateTime.UtcNow,
                IsCompleted = false,
                CurrentTutorialIndex = null
            };

            _context.GameSessions.Add(session);
            await _context.SaveChangesAsync();
            await _context.Entry(session).Reference(s => s.User).LoadAsync();

            var result = new AGameSession(
                session.Id,
                session.UserId,
                session.User.Login,
                session.User.Username,
                session.SessionType,
                session.TotalScore,
                session.SessionStart,
                session.CompletedAt,
                session.IsCompleted,
                0,
                0,
                session.CurrentTutorialIndex,
                session.IsDeleted,
                session.DeletedAt
            );

            return CreatedAtAction(nameof(Get), new { id = session.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AGameSessionUpdate dto)
        {
            if (id != dto.Id) return BadRequest();

            var session = await _context.GameSessions.FirstOrDefaultAsync(s => s.Id == id);
            if (session == null) return NotFound();

            if (dto.TotalScore.HasValue)
                session.TotalScore = dto.TotalScore.Value;

            if (dto.IsCompleted.HasValue)
                session.IsCompleted = dto.IsCompleted.Value;

            if (dto.CompletedAt.HasValue)
                session.CompletedAt = dto.CompletedAt;

            if (dto.CurrentTutorialIndex.HasValue)
                session.CurrentTutorialIndex = dto.CurrentTutorialIndex.Value;
            else if (dto.CurrentTutorialIndex == null)
                session.CurrentTutorialIndex = null;

            if (dto.IsDeleted.HasValue)
            {
                session.IsDeleted = dto.IsDeleted.Value;
                session.DeletedAt = dto.IsDeleted.Value ? DateTime.UtcNow : null;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/progress")]
        public async Task<ActionResult<IEnumerable<ASessionProgress>>> GetSessionProgress(int id, [FromQuery] bool includeDeleted = false)
        {
            var query = _context.SessionProgress
               .Include(sp => sp.Puzzle)
               .Include(sp => sp.Session).ThenInclude(s => s.User)
               .Where(sp => sp.SessionId == id);

            if (!includeDeleted)
                query = query.Where(sp => !sp.IsDeleted && !sp.Session.IsDeleted);

            var progress = await query
               .OrderBy(sp => sp.PuzzleOrder)
               .ToListAsync();

            var result = progress.Select(sp => new ASessionProgress(
                sp.Id,
                sp.SessionId,
                sp.Session.User.Login,
                sp.Session.User.Username,
                sp.PuzzleId,
                sp.Puzzle.Title,
                sp.PuzzleOrder,
                sp.Solved,
                sp.HintsUsed,
                sp.ScoreEarned,
                sp.StartedAt,
                sp.SolvedAt,
                sp.TimeToSolve,
                sp.IsDeleted,
                sp.DeletedAt
            )).ToList();

            return Ok(result);
        }
    }
}