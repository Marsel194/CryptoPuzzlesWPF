using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.SharedDTO;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameSessionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GameSessionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AGameSession>>> GetAll()
        {
            var sessions = await _context.GameSessions
                .Include(s => s.User)
                .Include(s => s.CurrentPuzzle)
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Id)
                .Select(s => new AGameSession(
                    s.Id,
                    s.UserId,
                    s.User.Login,
                    s.Score,
                    s.SessionStartTime,
                    s.CurrentPuzzleId,
                    s.CurrentPuzzle != null ? s.CurrentPuzzle.Title : null,
                    s.TrainingCompleted,
                    s.HintsUsed,
                    s.CompletedAt))
                .ToListAsync();
            return Ok(sessions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AGameSession>> Get(int id)
        {
            var session = await _context.GameSessions
                .Include(s => s.User)
                .Include(s => s.CurrentPuzzle)
                .Where(s => s.Id == id && !s.IsDeleted)
                .Select(s => new AGameSession(
                    s.Id,
                    s.UserId,
                    s.User.Login,
                    s.Score,
                    s.SessionStartTime,
                    s.CurrentPuzzleId,
                    s.CurrentPuzzle != null ? s.CurrentPuzzle.Title : null,
                    s.TrainingCompleted,
                    s.HintsUsed,
                    s.CompletedAt))
                .FirstOrDefaultAsync();
            if (session == null) return NotFound();
            return Ok(session);
        }

        [HttpPost]
        public async Task<ActionResult<AGameSession>> Create([FromBody] AGameSessionCreate dto)
        {
            var session = new GameSession
            {
                UserId = dto.UserId,
                Score = dto.Score,
                CurrentPuzzleId = dto.CurrentPuzzleId,
                TrainingCompleted = dto.TrainingCompleted,
                HintsUsed = dto.HintsUsed,
                CompletedAt = dto.CompletedAt,
                SessionStartTime = DateTime.UtcNow
            };
            _context.GameSessions.Add(session);
            await _context.SaveChangesAsync();

            await _context.Entry(session).Reference(s => s.User).LoadAsync();
            await _context.Entry(session).Reference(s => s.CurrentPuzzle).LoadAsync();

            var result = new AGameSession(
                session.Id,
                session.UserId,
                session.User.Login,
                session.Score,
                session.SessionStartTime,
                session.CurrentPuzzleId,
                session.CurrentPuzzle?.Title,
                session.TrainingCompleted,
                session.HintsUsed,
                session.CompletedAt);

            return CreatedAtAction(nameof(Get), new { id = session.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AGameSessionUpdate dto)
        {
            if (id != dto.Id) return BadRequest();
            var session = await _context.GameSessions
                .Where(s => s.Id == id && !s.IsDeleted)
                .FirstOrDefaultAsync();
            if (session == null) return NotFound();

            if (dto.Score.HasValue)
                session.Score = dto.Score.Value;
            if (dto.CurrentPuzzleId.HasValue)
                session.CurrentPuzzleId = dto.CurrentPuzzleId.Value;
            if (dto.TrainingCompleted.HasValue)
                session.TrainingCompleted = dto.TrainingCompleted.Value;
            if (dto.HintsUsed.HasValue)
                session.HintsUsed = dto.HintsUsed.Value;
            session.CompletedAt = dto.CompletedAt;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _context.GameSessions.FindAsync(id);
            if (session == null || session.IsDeleted) return NotFound();

            session.IsDeleted = true;
            session.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}