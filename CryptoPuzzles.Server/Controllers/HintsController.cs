using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HintsController(AppDbContext context) : BaseController<Hint, AHint, AHintCreate, AHintUpdate>(context)
    {
        protected override IQueryable<Hint> ApplyIncludes(IQueryable<Hint> query)
        {
            return query.Include(h => h.Puzzle);
        }

        protected override AHint MapToDto(Hint entity)
        {
            return new AHint(
                entity.Id,
                entity.PuzzleId,
                entity.Puzzle?.Title ?? string.Empty,
                entity.HintText,
                entity.HintOrder,
                entity.CreatedAt,
                entity.IsDeleted,
                entity.DeletedAt
            );
        }

        protected override Hint MapToEntity(AHintCreate dto)
        {
            return new Hint
            {
                PuzzleId = dto.PuzzleId,
                HintText = dto.HintText,
                HintOrder = dto.HintOrder,
                CreatedAt = DateTime.UtcNow
            };
        }

        protected override void UpdateEntity(Hint entity, AHintUpdate dto)
        {
            base.UpdateEntity(entity, dto);

            entity.PuzzleId = dto.PuzzleId;
            entity.HintText = dto.HintText;
            entity.HintOrder = dto.HintOrder;
        }
    }
}