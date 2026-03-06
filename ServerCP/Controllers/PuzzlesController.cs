using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PuzzlesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PuzzlesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<APuzzle>>> GetAll()
        {
            var puzzles = await _context.Puzzles
                .Include(p => p.Difficulty)
                .Include(p => p.Method)
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .Select(p => new APuzzle(
                    p.Id,
                    p.Title,
                    p.Content,
                    p.Answer,
                    p.MaxScore,
                    p.DifficultyId,
                    p.Difficulty.DifficultyName,
                    p.MethodId,
                    p.Method != null ? p.Method.Name : null,
                    p.IsTraining,
                    p.TutorialOrder,
                    p.CreatedByAdminId,
                    p.CreatedAt))
                .ToListAsync();
            return Ok(puzzles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APuzzle>> Get(int id)
        {
            var puzzle = await _context.Puzzles
                .Include(p => p.Difficulty)
                .Include(p => p.Method)
                .Where(p => p.Id == id && !p.IsDeleted)
                .Select(p => new APuzzle(
                    p.Id,
                    p.Title,
                    p.Content,
                    p.Answer,
                    p.MaxScore,
                    p.DifficultyId,
                    p.Difficulty.DifficultyName,
                    p.MethodId,
                    p.Method != null ? p.Method.Name : null,
                    p.IsTraining,
                    p.TutorialOrder,
                    p.CreatedByAdminId,
                    p.CreatedAt))
                .FirstOrDefaultAsync();
            if (puzzle == null) return NotFound();
            return Ok(puzzle);
        }

        [HttpPost]
        public async Task<ActionResult<APuzzle>> Create([FromBody] APuzzleCreate dto)
        {
            var puzzle = new Puzzle
            {
                Title = dto.Title,
                Content = dto.Content,
                Answer = dto.Answer,
                MaxScore = dto.MaxScore,
                DifficultyId = dto.DifficultyId,
                MethodId = dto.MethodId,
                IsTraining = dto.IsTraining,
                TutorialOrder = dto.TutorialOrder,
                CreatedAt = DateTime.UtcNow
                // CreatedByAdminId можно установить позже, если нужно
            };
            _context.Puzzles.Add(puzzle);
            await _context.SaveChangesAsync();

            // Загружаем связанные данные для ответа
            await _context.Entry(puzzle).Reference(p => p.Difficulty).LoadAsync();
            await _context.Entry(puzzle).Reference(p => p.Method).LoadAsync();

            var result = new APuzzle(
                puzzle.Id,
                puzzle.Title,
                puzzle.Content,
                puzzle.Answer,
                puzzle.MaxScore,
                puzzle.DifficultyId,
                puzzle.Difficulty.DifficultyName,
                puzzle.MethodId,
                puzzle.Method?.Name,
                puzzle.IsTraining,
                puzzle.TutorialOrder,
                puzzle.CreatedByAdminId,
                puzzle.CreatedAt);

            return CreatedAtAction(nameof(Get), new { id = puzzle.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] APuzzleUpdate dto)
        {
            if (id != dto.Id) return BadRequest();
            var puzzle = await _context.Puzzles
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefaultAsync();
            if (puzzle == null) return NotFound();

            puzzle.Title = dto.Title;
            puzzle.Content = dto.Content;
            puzzle.Answer = dto.Answer;
            puzzle.MaxScore = dto.MaxScore;
            puzzle.DifficultyId = dto.DifficultyId;
            puzzle.MethodId = dto.MethodId;
            puzzle.IsTraining = dto.IsTraining;
            puzzle.TutorialOrder = dto.TutorialOrder;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var puzzle = await _context.Puzzles.FindAsync(id);
            if (puzzle == null || puzzle.IsDeleted) return NotFound();

            puzzle.IsDeleted = true;
            puzzle.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}