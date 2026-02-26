using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Models;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login);

                if (user != null && VerifyPasswordArgon.VerifyPassword(request.Password, user.PasswordHash))
                {
                    var response = new LoginResponse
                    {
                        Id = user.Id,
                        Login = user.Login,
                        Email = user.Email,
                        Username = user.Username,
                        IsAdmin = false
                    };
                    return Ok(response);
                }

                var admin = await _context.Admins.FirstOrDefaultAsync(u => u.Login == request.Login);

                if (admin != null && VerifyPasswordArgon.VerifyPassword(request.Password, admin.PasswordHash))
                {
                    var response = new LoginResponse
                    {
                        Id = admin.Id,
                        Login = admin.Login,
                        Username = admin.LastName + " " + admin.FirstName[0] + ". " +
                            (admin.MiddleName != null ? admin.MiddleName[0] + "." : ""),
                        IsAdmin = true
                    };
                    return Ok(response);
                }

                return Unauthorized(new ErrorResponse
                {
                    Message = "Неверный логин или пароль"
                });
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Внутренняя ошибка сервера"
                });
            }
        }
    }
}