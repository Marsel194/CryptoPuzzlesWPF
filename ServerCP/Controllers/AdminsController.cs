using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Models;

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

        [HttpGet("get")]
        public async Task<ActionResult<Admin>> GetAdmins()
        {
            try
            {
                var admins = await _context.Admins.ToListAsync();

                if (admins != null)
                {
                    return Ok(admins);
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest(new ErrorResponse
                {
                    message = "Внутренняя ошибка сервера"
                });
            }
        }
    }
}
