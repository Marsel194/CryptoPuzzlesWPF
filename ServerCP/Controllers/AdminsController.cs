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
                .Select(a => new AAdmin(a.Id, a.Login, a.FirstName, a.LastName, a.MiddleName ?? "", a.CreatedAt))
                .ToListAsync();
            return Ok(admins);
        }

        // CREATE: Добавить нового
        [HttpPost]
        public async Task<ActionResult<AAdmin>> Create([FromBody] Admin admin)
        {
            // Хешируем пароль перед сохранением
            admin.PasswordHash = Argon2PasswordActions.HashPassword(admin.PasswordHash);
            admin.CreatedAt = DateTime.Now;

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = admin.Id },
                new AAdmin(admin.Id, admin.Login, admin.FirstName, admin.LastName, admin.MiddleName ?? "", admin.CreatedAt));
        }

        // UPDATE: Редактировать данные
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Admin updatedAdmin)
        {
            var existing = await _context.Admins.FindAsync(id);
            if (existing == null) return NotFound();

            existing.FirstName = updatedAdmin.FirstName;
            existing.LastName = updatedAdmin.LastName;
            existing.MiddleName = updatedAdmin.MiddleName;
            existing.Login = updatedAdmin.Login;

            // Если пришел новый пароль — обновляем его
            if (!string.IsNullOrWhiteSpace(updatedAdmin.PasswordHash))
                existing.PasswordHash = Argon2PasswordActions.HashPassword(updatedAdmin.PasswordHash);

            await _context.SaveChangesAsync();
            return NoContent();
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