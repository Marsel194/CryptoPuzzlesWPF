using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.SharedDTO;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionMethodsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EncryptionMethodsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AEncryptionMethod>>> GetAll()
        {
            var methods = await _context.EncryptionMethods
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.Id)
                .Select(m => new AEncryptionMethod(m.Id, m.Name))
                .ToListAsync();
            return Ok(methods);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AEncryptionMethod>> Get(int id)
        {
            var method = await _context.EncryptionMethods
                .Where(m => m.Id == id && !m.IsDeleted)
                .Select(m => new AEncryptionMethod(m.Id, m.Name))
                .FirstOrDefaultAsync();
            if (method == null) return NotFound();
            return Ok(method);
        }

        [HttpPost]
        public async Task<ActionResult<AEncryptionMethod>> Create([FromBody] AEncryptionMethodCreate dto)
        {
            var method = new EncryptionMethod
            {
                Name = dto.Name
            };
            _context.EncryptionMethods.Add(method);
            await _context.SaveChangesAsync();

            var result = new AEncryptionMethod(method.Id, method.Name);
            return CreatedAtAction(nameof(Get), new { id = method.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AEncryptionMethodUpdate dto)
        {
            if (id != dto.Id) return BadRequest();
            var method = await _context.EncryptionMethods
                .Where(m => m.Id == id && !m.IsDeleted)
                .FirstOrDefaultAsync();
            if (method == null) return NotFound();

            method.Name = dto.Name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var method = await _context.EncryptionMethods.FindAsync(id);
            if (method == null || method.IsDeleted) return NotFound();

            method.IsDeleted = true;
            method.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}