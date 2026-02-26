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
    }
}
