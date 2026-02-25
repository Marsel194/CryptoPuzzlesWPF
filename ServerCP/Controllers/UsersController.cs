using CryptoPuzzles.Server;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using CryptoPuzzles.Server.Models;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    private string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = 1;
        argon2.MemorySize = 65536;
        argon2.Iterations = 3;

        byte[] hash = argon2.GetBytes(32);

        byte[] hashBytes = new byte[16 + 32];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(User user)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Login == user.Login);

        if (existingUser != null)
            return BadRequest(new { message = "Логин уже занят" });

        user.PasswordHash = HashPassword(user.PasswordHash);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.PasswordHash = null;
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == request.Login);

            if (user == null || !VerifyPasswordArgon.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new ErrorResponse
                {
                    message = "Неверный логин или пароль"
                });
            }

            var response = new LoginResponse
            {
                Id = user.Id,
                Login = user.Login,
                Email = user.Email
            };

            return Ok(response);
        }
        catch (Exception)
        {
            return StatusCode(500, new ErrorResponse
            {
                message = "Внутренняя ошибка сервера"
            });
        }
    }
}