using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Server.Data;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

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
                    Message = "Внутренняя ошибка сервера"
                });
            }
        }
    }
}
