using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Helpers;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(AppDbContext context) : BaseController<User, AUser, AUserCreate, AUserUpdate>(context)
    {
        protected override AUser MapToDto(User entity)
        {
            return new AUser(
                entity.Id,
                entity.Login,
                entity.Username,
                entity.Email,
                entity.CreatedAt,
                entity.IsDeleted,
                entity.DeletedAt
            );
        }

        protected override User MapToEntity(AUserCreate dto)
        {
            return new User
            {
                Login = dto.Login,
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = Argon2PasswordActions.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };
        }

        protected override void UpdateEntity(User entity, AUserUpdate dto)
        {
            base.UpdateEntity(entity, dto);

            entity.Login = dto.Login;
            entity.Username = dto.Username;
            entity.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Password))
                entity.PasswordHash = Argon2PasswordActions.HashPassword(dto.Password);
        }

        [HttpPost]
        public override async Task<ActionResult<AUser>> Create(AUserCreate dto)
        {
            if (await _context.Users.AnyAsync(u => u.Login == dto.Login && !u.IsDeleted))
                return Conflict(new UAErrorResponse("Пользователь с таким логином уже существует", null));

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && !u.IsDeleted))
                return Conflict(new UAErrorResponse("Пользователь с таким email уже существует", null));

            var entity = MapToEntity(dto);
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = entity.Id }, MapToDto(entity));
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(int id, AUserUpdate dto)
        {
            if (id != dto.Id)
                return BadRequest("ID в маршруте не совпадает с ID сущности");

            var entity = await _context.Users
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstOrDefaultAsync();
            if (entity == null)
                return NotFound();

            if (entity.Login != dto.Login)
            {
                if (await _context.Users.AnyAsync(u => u.Login == dto.Login && u.Id != id && !u.IsDeleted))
                    return Conflict(new UAErrorResponse("Пользователь с таким логином уже существует", null));
            }

            if (entity.Email != dto.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id && !u.IsDeleted))
                    return Conflict(new UAErrorResponse("Пользователь с таким email уже существует", null));
            }

            UpdateEntity(entity, dto);

            if (entity.IsDeleted && entity.DeletedAt == null)
                entity.DeletedAt = DateTime.UtcNow;
            if (!entity.IsDeleted)
                entity.DeletedAt = null;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}