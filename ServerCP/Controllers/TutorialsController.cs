using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorialsController : BaseController<Tutorial, ATutorial, ATutorialCreate, ATutorialUpdate>
    {
        public TutorialsController(AppDbContext context) : base(context) { }

        protected override ATutorial MapToDto(Tutorial entity)
        {
            return new ATutorial(
                entity.Id,
                entity.MethodId,
                entity.Method?.Name ?? string.Empty,
                entity.TheoryTitle,
                entity.TheoryContent,
                entity.SortOrder,
                entity.CreatedAt,
                entity.IsDeleted,
                entity.DeletedAt
            );
        }

        protected override Tutorial MapToEntity(ATutorialCreate dto)
        {
            return new Tutorial
            {
                MethodId = dto.MethodId,
                TheoryTitle = dto.TheoryTitle,
                TheoryContent = dto.TheoryContent,
                SortOrder = dto.SortOrder,
                CreatedAt = DateTime.UtcNow
            };
        }

        protected override void UpdateEntity(Tutorial entity, ATutorialUpdate dto)
        {
            entity.MethodId = dto.MethodId;
            entity.TheoryTitle = dto.TheoryTitle;
            entity.TheoryContent = dto.TheoryContent;
            entity.SortOrder = dto.SortOrder;
        }

        [HttpGet]
        public override async Task<ActionResult<IEnumerable<ATutorial>>> GetAll()
        {
            var tutorials = await _context.Tutorials
                .Include(t => t.Method)
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Id)
                .Select(t => new ATutorial(
                    t.Id,
                    t.MethodId,
                    t.Method.Name,
                    t.TheoryTitle,
                    t.TheoryContent,
                    t.SortOrder,
                    t.CreatedAt,
                    t.IsDeleted,
                    t.DeletedAt
                ))
                .ToListAsync();
            return Ok(tutorials);
        }

        [HttpGet("{id}")]
        public override async Task<ActionResult<ATutorial>> Get(int id)
        {
            var tutorial = await _context.Tutorials
                .Include(t => t.Method)
                .Where(t => t.Id == id && !t.IsDeleted)
                .Select(t => new ATutorial(
                    t.Id,
                    t.MethodId,
                    t.Method.Name,
                    t.TheoryTitle,
                    t.TheoryContent,
                    t.SortOrder,
                    t.CreatedAt,
                    t.IsDeleted,
                    t.DeletedAt
                ))
                .FirstOrDefaultAsync();
            if (tutorial == null) return NotFound();
            return Ok(tutorial);
        }

        [HttpPost]
        public override async Task<ActionResult<ATutorial>> Create(ATutorialCreate dto)
        {
            var entity = MapToEntity(dto);
            _context.Tutorials.Add(entity);
            await _context.SaveChangesAsync();
            await _context.Entry(entity).Reference(t => t.Method).LoadAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, MapToDto(entity));
        }
    }
}