using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DifficultiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DifficultiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ADifficulty>>> GetAll()
        {
            var difficulties = await _context.Difficulties
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Id)
                .Select(d => new ADifficulty(d.Id, d.DifficultyName))
                .ToListAsync();
            return Ok(difficulties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ADifficulty>> Get(int id)
        {
            var difficulty = await _context.Difficulties
                .Where(d => d.Id == id && !d.IsDeleted)
                .Select(d => new ADifficulty(d.Id, d.DifficultyName))
                .FirstOrDefaultAsync();
            if (difficulty == null) return NotFound();
            return Ok(difficulty);
        }

        [HttpPost]
        public async Task<ActionResult<ADifficulty>> Create([FromBody] ADifficultyCreate dto)
        {
            var difficulty = new Difficulty
            {
                DifficultyName = dto.DifficultyName
            };
            _context.Difficulties.Add(difficulty);
            await _context.SaveChangesAsync();

            var result = new ADifficulty(difficulty.Id, difficulty.DifficultyName);
            return CreatedAtAction(nameof(Get), new { id = difficulty.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ADifficultyUpdate dto)
        {
            if (id != dto.Id) return BadRequest();
            var difficulty = await _context.Difficulties
                .Where(d => d.Id == id && !d.IsDeleted)
                .FirstOrDefaultAsync();
            if (difficulty == null) return NotFound();

            difficulty.DifficultyName = dto.DifficultyName;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var difficulty = await _context.Difficulties.FindAsync(id);
            if (difficulty == null || difficulty.IsDeleted) return NotFound();

            difficulty.IsDeleted = true;
            difficulty.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}