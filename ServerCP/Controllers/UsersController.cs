using Hairulin_02_01;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Konscious.Security.Cryptography;
using System.Text;
using System.Security.Cryptography;

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

    private bool VerifyPassword(string password, string storedHash)
    {
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        byte[] hash = new byte[32];
        Array.Copy(hashBytes, 16, hash, 0, 32);

        using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
        {
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 1;
            argon2.MemorySize = 65536;
            argon2.Iterations = 3;

            byte[] newHash = argon2.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (hash[i] != newHash[i])
                    return false;
            }
            return true;
        }
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

    [HttpGet("login")]
    public async Task<IActionResult> Login(string login, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Login == login);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return BadRequest(new { message = "Логин или пароль неверные" });

        user.PasswordHash = null;
        return Ok(user);
    }
}