using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Helpers;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UALoginRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login && !u.IsDeleted);
                if (user != null)
                {
                    if (Argon2PasswordActions.VerifyPassword(request.Password, user.PasswordHash))
                    {
                        var response = new UALoginResponse(
                            user.Id,
                            user.Login,
                            user.Email,
                            user.Username,
                            false
                        );
                        return Ok(response);
                    }
                }

                var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Login == request.Login && !a.IsDeleted);
                if (admin != null)
                {
                    if (Argon2PasswordActions.VerifyPassword(request.Password, admin.PasswordHash))
                    {
                        string username = $"{admin.LastName} {admin.FirstName[0]}.";
                        if (!string.IsNullOrEmpty(admin.MiddleName))
                            username += $" {admin.MiddleName[0]}.";

                        var response = new UALoginResponse(
                            admin.Id,
                            admin.Login,
                            "",
                            username,
                            true
                        );
                        return Ok(response);
                    }
                }

                return Unauthorized(new UAErrorResponse("Неверный логин или пароль", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UAErrorResponse("Внутренняя ошибка сервера", ex.Message));
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UARegisterRequest request)
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Login == request.Login && !u.IsDeleted);
                if (existingUser != null)
                    return Conflict(new UAErrorResponse("Пользователь с таким логином уже существует", null));

                var existingEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);
                if (existingEmail != null)
                    return Conflict(new UAErrorResponse("Пользователь с таким email уже существует", null));

                var newUser = new User
                {
                    Login = request.Login,
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = Argon2PasswordActions.HashPassword(request.Password),
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var response = new UARegisterResponse(
                    Id: newUser.Id,
                    Login: newUser.Login,
                    Username: newUser.Username,
                    Email: newUser.Email
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UAErrorResponse("Внутренняя ошибка сервера при регистрации", ex.Message));
            }
        }
    }
}