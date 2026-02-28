using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.SharedDTO;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<AAdminDto>>> GetAdmins()
        {
            try
            {
                var admins = await _context.Admins.ToListAsync();

                if (admins != null && admins.Any())
                {
                    var adminDtos = admins.Select(a => new AAdminDto(
                        Id: a.Id,
                        Login: a.Login,
                        FirstName: a.FirstName,
                        LastName: a.LastName,
                        MiddleName: a.MiddleName ?? string.Empty,
                        CreatedAt: a.CreatedAt
                    ));

                    return Ok(adminDtos);
                }

                return NotFound(new UAErrorResponse("Администраторы не найдены", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UAErrorResponse("Внутренняя ошибка сервера", ex.Message));
            }
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<AAdminDto>> GetAdmin(int id)
        {
            try
            {
                var admin = await _context.Admins.FindAsync(id);

                if (admin == null)
                {
                    return NotFound(new UAErrorResponse($"Администратор с ID {id} не найден", null));
                }

                var adminDto = new AAdminDto(
                    Id: admin.Id,
                    Login: admin.Login,
                    FirstName: admin.FirstName,
                    LastName: admin.LastName,
                    MiddleName: admin.MiddleName ?? string.Empty,
                    CreatedAt: admin.CreatedAt
                );

                return Ok(adminDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UAErrorResponse("Внутренняя ошибка сервера", ex.Message));
            }
        }
    }
}