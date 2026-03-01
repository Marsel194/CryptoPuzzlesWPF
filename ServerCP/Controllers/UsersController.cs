using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Helpers;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.SharedDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AUser>>> GetAll()
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new AUser(u.Id, u.Login, u.Username, u.Email, u.CreatedAt))
                .ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AUser>> Get(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id && !u.IsDeleted)
                .Select(u => new AUser(u.Id, u.Login, u.Username, u.Email, u.CreatedAt))
                .FirstOrDefaultAsync();
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<AUser>> Create([FromBody] AUserCreate dto)
        {
            var user = new User
            {
                Login = dto.Login,
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = Argon2PasswordActions.HashPassword(dto.Password), // используйте ваш метод хеширования
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.Id },
                new AUser(user.Id, user.Login, user.Username, user.Email, user.CreatedAt));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AUserUpdate dto)
        {
            if (id != dto.Id) return BadRequest();
            var user = await _context.Users
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstOrDefaultAsync();
            if (user == null) return NotFound();

            user.Login = dto.Login;
            user.Username = dto.Username;
            user.Email = dto.Email;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted) return NotFound();

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}