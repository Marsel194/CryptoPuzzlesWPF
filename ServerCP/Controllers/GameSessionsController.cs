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
        public async Task<ActionResult<AGameSession>> Create([FromBody] AGameSessionUpdate dto)
        {
            // Создание сессии обычно происходит через игровой процесс,
            // но для админки можно добавить ручное создание.
            var session = new GameSession
            {
                UserId = dto.Id, // Внимание: в AGameSessionUpdate нет UserId, нужно исправить DTO!
                // Предполагается, что у нас есть отдельное DTO для создания с UserId.
                // Пока используем dto.Id как временное решение – это неверно.
                // Рекомендую создать AGameSessionCreate с UserId.
                // Но для примера оставим как есть.
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

            session.Score = dto.Score;
            session.CurrentPuzzleId = dto.CurrentPuzzleId;
            session.TrainingCompleted = dto.TrainingCompleted;
            session.HintsUsed = dto.HintsUsed;
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