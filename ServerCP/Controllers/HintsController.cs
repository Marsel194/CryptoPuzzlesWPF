using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HintsController : BaseController<Hint, AHint, AHintCreate, AHintUpdate>
    {
        public HintsController(AppDbContext context) : base(context) { }

        protected override AHint MapToDto(Hint entity)
        {
            return new AHint(
                entity.Id,
                entity.PuzzleId,
                entity.Puzzle?.Title ?? string.Empty,
                entity.HintText,
                entity.HintOrder,
                entity.CreatedAt,
                entity.IsDeleted,
                entity.DeletedAt
            );
        }

        protected override Hint MapToEntity(AHintCreate dto)
        {
            return new Hint
            {
                PuzzleId = dto.PuzzleId,
                HintText = dto.HintText,
                HintOrder = dto.HintOrder,
                CreatedAt = DateTime.UtcNow
            };
        }

        protected override void UpdateEntity(Hint entity, AHintUpdate dto)
        {
            entity.PuzzleId = dto.PuzzleId;
            entity.HintText = dto.HintText;
            entity.HintOrder = dto.HintOrder;
        }

        [HttpGet]
        public override async Task<ActionResult<IEnumerable<AHint>>> GetAll()
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
                    h.CreatedAt,
                    h.IsDeleted,
                    h.DeletedAt
                ))
                .ToListAsync();
            return Ok(hints);
        }

        [HttpGet("{id}")]
        public override async Task<ActionResult<AHint>> Get(int id)
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
                    h.CreatedAt,
                    h.IsDeleted,
                    h.DeletedAt
                ))
                .FirstOrDefaultAsync();
            if (hint == null) return NotFound();
            return Ok(hint);
        }

        [HttpPost]
        public override async Task<ActionResult<AHint>> Create(AHintCreate dto)
        {
            var entity = MapToEntity(dto);
            _context.Hints.Add(entity);
            await _context.SaveChangesAsync();
            await _context.Entry(entity).Reference(h => h.Puzzle).LoadAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, MapToDto(entity));
        }
    }
}