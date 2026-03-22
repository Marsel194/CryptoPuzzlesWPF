using Microsoft.AspNetCore.Mvc;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DifficultiesController : BaseController<Difficulty, ADifficulty, ADifficultyCreate, ADifficultyUpdate>
    {
        public DifficultiesController(AppDbContext context) : base(context) { }

        protected override ADifficulty MapToDto(Difficulty entity)
        {
            return new ADifficulty(entity.Id, entity.DifficultyName);
        }

        protected override Difficulty MapToEntity(ADifficultyCreate dto)
        {
            return new Difficulty { DifficultyName = dto.DifficultyName };
        }
    }
}