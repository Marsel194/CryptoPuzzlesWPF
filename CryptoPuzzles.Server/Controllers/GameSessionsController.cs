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

        private AGameSession MapToDto(GameSession session)
        {
            return new AGameSession(
                session.Id,
                session.UserId,
                session.User?.Login ?? string.Empty,
                session.User?.Username ?? string.Empty,
                session.SessionType,
                session.TotalScore,
                session.SessionStart,
                session.CompletedAt,
                session.IsCompleted,
                session.Progresses?.Count ?? 0,
                session.Progresses?.Count(p => p.Solved) ?? 0,
                session.CurrentTutorialIndex,
                session.IsDeleted,
                session.DeletedAt
            );
        }

        private GameSession MapToEntity(AGameSessionCreate dto)
        {
            return new GameSession
            {
                UserId = dto.UserId,
                SessionType = dto.SessionType,
                TotalScore = dto.TotalScore,
                SessionStart = DateTime.UtcNow,
                IsCompleted = false,
                CurrentTutorialIndex = null
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AGameSession>>> GetAll()
        {
            var sessions = await _context.GameSessions
                .Include(s => s.User)
                .Include(s => s.Progresses)
                .OrderByDescending(s => s.SessionStart)
                .ToListAsync();
            return Ok(sessions.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AGameSession>> Get(int id)
        {
            var session = await _context.GameSessions
                .Include(s => s.User)
                .Include(s => s.Progresses)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (session == null) return NotFound();
            return Ok(MapToDto(session));
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AGameSession>>> GetByUser(int userId)
        {
            var sessions = await _context.GameSessions
                .Include(s => s.User)
                .Include(s => s.Progresses)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SessionStart)
                .ToListAsync();
            return Ok(sessions.Select(MapToDto));
        }

        [HttpPost]
        public async Task<ActionResult<AGameSession>> Create([FromBody] AGameSessionCreate dto)
        {
            var session = MapToEntity(dto);
            _context.GameSessions.Add(session);
            await _context.SaveChangesAsync();
            await _context.Entry(session).Reference(s => s.User).LoadAsync();
            return CreatedAtAction(nameof(Get), new { id = session.Id }, MapToDto(session));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AGameSessionUpdate dto)
        {
            if (id != dto.Id) return BadRequest();

            var session = await _context.GameSessions.FindAsync(id);
            if (session == null) return NotFound();

            if (dto.TotalScore.HasValue)
                session.TotalScore = dto.TotalScore.Value;

            if (dto.IsCompleted.HasValue)
                session.IsCompleted = dto.IsCompleted.Value;

            if (dto.CompletedAt.HasValue)
            {
                var completedAt = dto.CompletedAt.Value;
                if (completedAt.Kind != DateTimeKind.Utc)
                    completedAt = DateTime.SpecifyKind(completedAt, DateTimeKind.Utc);
                session.CompletedAt = completedAt;
            }

            if (dto.CurrentTutorialIndex.HasValue)
                session.CurrentTutorialIndex = dto.CurrentTutorialIndex.Value;

            if (dto.IsDeleted.HasValue)
            {
                session.IsDeleted = dto.IsDeleted.Value;
                session.DeletedAt = dto.IsDeleted.Value ? DateTime.UtcNow : null;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/progress")]
        public async Task<ActionResult<IEnumerable<ASessionProgress>>> GetSessionProgress(int id)
        {
            var progress = await _context.SessionProgress
               .Include(sp => sp.Puzzle)
               .Include(sp => sp.Session).ThenInclude(s => s.User)
               .Where(sp => sp.SessionId == id && !sp.IsDeleted)
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