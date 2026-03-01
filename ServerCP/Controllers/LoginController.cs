using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Helpers;
using CryptoPuzzles.SharedDTO;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UALoginRequest request)
        {
            try
            {
                // Проверяем в Users
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
                if (user != null)
                {
                    if (Argon2PasswordActions.VerifyPassword(request.Password, user.PasswordHash))
                    {
                        var response = new UALoginResponse(user.Login, user.Email, user.Username, false);
                        return Ok(response);
                    }
                }

                // Проверяем в Admins
                var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Login == request.Login);
                if (admin != null)
                {
                    if (Argon2PasswordActions.VerifyPassword(request.Password, admin.PasswordHash))
                    {
                        // Формируем имя для админа
                        string username = $"{admin.LastName} {admin.FirstName[0]}.";
                        if (!string.IsNullOrEmpty(admin.MiddleName))
                        {
                            username += $" {admin.MiddleName[0]}.";
                        }

                        var response = new UALoginResponse(admin.Login, "", username, true);
                        return Ok(response);
                    }
                }

                // Неудачная попытка входа
                return Unauthorized(new UAErrorResponse("Неверный логин или пароль", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UAErrorResponse("Внутренняя ошибка сервера", ex.Message));
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UARegisterRequest request)  // используем общий DTO
        {
            try
            {
                // Проверяем, не занят ли логин
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
                if (existingUser != null)
                {
                    return Conflict(new UAErrorResponse("Пользователь с таким логином уже существует", null));
                }

                // Проверяем, не занят ли email
                var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingEmail != null)
                {
                    return Conflict(new UAErrorResponse("Пользователь с таким email уже существует", null));
                }

                // Создаём нового пользователя (нужно будет добавить хеширование пароля)
                var newUser = new User
                {
                    Login = request.Login,
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = Argon2PasswordActions.HashPassword(request.Password), // если такой метод есть
                    CreatedAt = DateTime.UtcNow
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