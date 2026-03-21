using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Helpers;
using CryptoPuzzles.Shared;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private string GenerateJwtToken(int id, string login, bool isAdmin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, login),
                new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UALoginRequest request)
        {
            try
            {
                // Проверка пользователя
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login && !u.IsDeleted);
                if (user != null)
                {
                    if (Argon2PasswordActions.VerifyPassword(request.Password, user.PasswordHash))
                    {
                        var token = GenerateJwtToken(user.Id, user.Login, false);
                        var response = new UALoginResponse(
                            user.Id,
                            user.Login,
                            user.Email,
                            user.Username,
                            false,
                            token  // <-- добавляем токен
                        );
                        return Ok(response);
                    }
                }

                // Проверка администратора
                var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Login == request.Login && !a.IsDeleted);
                if (admin != null)
                {
                    if (Argon2PasswordActions.VerifyPassword(request.Password, admin.PasswordHash))
                    {
                        // build short display name safely
                        var firstInitial = !string.IsNullOrWhiteSpace(admin.FirstName) ? admin.FirstName[0].ToString() + "." : string.Empty;
                        var middleInitial = !string.IsNullOrWhiteSpace(admin.MiddleName) ? " " + admin.MiddleName[0] + "." : string.Empty;
                        string username = string.IsNullOrWhiteSpace(admin.LastName)
                            ? (firstInitial + middleInitial).Trim()
                            : ($"{admin.LastName} {firstInitial}" + middleInitial).Trim();

                        var token = GenerateJwtToken(admin.Id, admin.Login, true);
                        var response = new UALoginResponse(
                            admin.Id,
                            admin.Login,
                            "",
                            username,
                            true,
                            token  // <-- добавляем токен
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