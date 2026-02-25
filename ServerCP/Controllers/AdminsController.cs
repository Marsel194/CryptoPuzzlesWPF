using Hairulin_02_01;
using Hairulin_02_01.Models;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using CryptoPuzzles.Server;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Admins
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
                    IsAdmin = true
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
}
