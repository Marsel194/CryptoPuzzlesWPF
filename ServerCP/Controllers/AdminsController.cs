using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.Server.Helpers;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // READ: Получить всех (кроме удаленных)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AAdmin>>> GetAll()
        {
            var admins = await _context.Admins
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.Id)
                .Select(a => new AAdmin(a.Id, a.Login, a.FirstName, a.LastName, a.MiddleName ?? "", a.CreatedAt))
                .ToListAsync();
            return Ok(admins);
        }

        [HttpPost]
        public async Task<ActionResult<AAdmin>> Create([FromBody] AAdminCreate dto)
        {
            var admin = new Admin
            {
                Login = dto.Login,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MiddleName = dto.MiddleName,
                PasswordHash = Argon2PasswordActions.HashPassword(dto.Password),
                CreatedAt = DateTime.Now
            };
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = admin.Id },
                new AAdmin(admin.Id, admin.Login, admin.FirstName, admin.LastName, admin.MiddleName ?? "", admin.CreatedAt));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AAdminUpdate dto)
        {
            if (id != dto.Id) return BadRequest();
            var existing = await _context.Admins
                .Where(a => a.Id == id && !a.IsDeleted)
                .FirstOrDefaultAsync();
            if (existing == null) return NotFound();

            existing.Login = dto.Login;
            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.MiddleName = dto.MiddleName;
            if (!string.IsNullOrWhiteSpace(dto.Password))
                existing.PasswordHash = Argon2PasswordActions.HashPassword(dto.Password);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AAdmin>> Get(int id)
        {
            var admin = await _context.Admins
                .Where(a => a.Id == id && !a.IsDeleted)
                .Select(a => new AAdmin(a.Id, a.Login, a.FirstName, a.LastName, a.MiddleName ?? "", a.CreatedAt))
                .FirstOrDefaultAsync();
            if (admin == null) return NotFound();
            return Ok(admin);
        }

        // DELETE: Мягкое удаление (флаг is_deleted)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();

            admin.IsDeleted = true;
            admin.DeletedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}