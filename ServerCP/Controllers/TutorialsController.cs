using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.SharedDTO;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorialsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TutorialsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ATutorial>>> GetAll()
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
                    t.CreatedAt))
                .ToListAsync();
            return Ok(tutorials);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ATutorial>> Get(int id)
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
                    t.CreatedAt))
                .FirstOrDefaultAsync();
            if (tutorial == null) return NotFound();
            return Ok(tutorial);
        }

        [HttpPost]
        public async Task<ActionResult<ATutorial>> Create([FromBody] ATutorialCreate dto)
        {
            var tutorial = new Tutorial
            {
                MethodId = dto.MethodId,
                TheoryTitle = dto.TheoryTitle,
                TheoryContent = dto.TheoryContent,
                SortOrder = dto.SortOrder,
                CreatedAt = DateTime.UtcNow
            };
            _context.Tutorials.Add(tutorial);
            await _context.SaveChangesAsync();

            await _context.Entry(tutorial).Reference(t => t.Method).LoadAsync();

            var result = new ATutorial(
                tutorial.Id,
                tutorial.MethodId,
                tutorial.Method.Name,
                tutorial.TheoryTitle,
                tutorial.TheoryContent,
                tutorial.SortOrder,
                tutorial.CreatedAt);

            return CreatedAtAction(nameof(Get), new { id = tutorial.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ATutorialUpdate dto)
        {
            if (id != dto.Id) return BadRequest();
            var tutorial = await _context.Tutorials
                .Where(t => t.Id == id && !t.IsDeleted)
                .FirstOrDefaultAsync();
            if (tutorial == null) return NotFound();

            tutorial.MethodId = dto.MethodId;
            tutorial.TheoryTitle = dto.TheoryTitle;
            tutorial.TheoryContent = dto.TheoryContent;
            tutorial.SortOrder = dto.SortOrder;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tutorial = await _context.Tutorials.FindAsync(id);
            if (tutorial == null || tutorial.IsDeleted) return NotFound();

            tutorial.IsDeleted = true;
            tutorial.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}