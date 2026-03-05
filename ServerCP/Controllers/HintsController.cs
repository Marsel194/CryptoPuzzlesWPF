using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.SharedDTO;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HintsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HintsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AHint>>> GetAll()
        {
            var hints = await _context.Hints
                .Include(h => h.Puzzle)
                .Where(h => !h.IsDeleted)
                .OrderBy(h => h.Id)
                .Select(h => new AHint(
                    h.Id,
                    h.PuzzleId,
                    h.Puzzle.Title,
                    h.HintText,
                    h.HintOrder,
                    h.CreatedAt))
                .ToListAsync();
            return Ok(hints);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AHint>> Get(int id)
        {
            var hint = await _context.Hints
                .Include(h => h.Puzzle)
                .Where(h => h.Id == id && !h.IsDeleted)
                .Select(h => new AHint(
                    h.Id,
                    h.PuzzleId,
                    h.Puzzle.Title,
                    h.HintText,
                    h.HintOrder,
                    h.CreatedAt))
                .FirstOrDefaultAsync();
            if (hint == null) return NotFound();
            return Ok(hint);
        }

        [HttpPost]
        public async Task<ActionResult<AHint>> Create([FromBody] AHintCreate dto)
        {
            var hint = new Hint
            {
                PuzzleId = dto.PuzzleId,
                HintText = dto.HintText,
                HintOrder = dto.HintOrder,
                CreatedAt = DateTime.UtcNow
            };
            _context.Hints.Add(hint);
            await _context.SaveChangesAsync();

            await _context.Entry(hint).Reference(h => h.Puzzle).LoadAsync();

            var result = new AHint(
                hint.Id,
                hint.PuzzleId,
                hint.Puzzle.Title,
                hint.HintText,
                hint.HintOrder,
                hint.CreatedAt);

            return CreatedAtAction(nameof(Get), new { id = hint.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AHintUpdate dto)
        {
            if (id != dto.Id) return BadRequest();
            var hint = await _context.Hints
                .Where(h => h.Id == id && !h.IsDeleted)
                .FirstOrDefaultAsync();
            if (hint == null) return NotFound();

            hint.PuzzleId = dto.PuzzleId;
            hint.HintText = dto.HintText;
            hint.HintOrder = dto.HintOrder;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var hint = await _context.Hints.FindAsync(id);
            if (hint == null || hint.IsDeleted) return NotFound();

            hint.IsDeleted = true;
            hint.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}