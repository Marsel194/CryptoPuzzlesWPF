using Hairulin_02_01;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;

    public HealthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("check-database")]
    public async Task<IActionResult> CheckDatabase()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();

            return Ok(new
            {
                canConnect,
                message = canConnect ? "Подключение к БД успешно" : "Не удалось подключиться к БД"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                canConnect = false,
                error = ex.Message
            });
        }
    }
}