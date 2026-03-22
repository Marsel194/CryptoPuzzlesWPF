using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorialsController : BaseController<Tutorial, ATutorial, ATutorialCreate, ATutorialUpdate>
    {
        public TutorialsController(AppDbContext context) : base(context) { }

        protected override IQueryable<Tutorial> ApplyIncludes(IQueryable<Tutorial> query)
        {
            return query.Include(t => t.Method);
        }

        protected override ATutorial MapToDto(Tutorial entity)
        {
            return new ATutorial(
                entity.Id,
                entity.MethodId,
                entity.Method?.Name ?? string.Empty,
                entity.TheoryTitle,
                entity.TheoryContent,
                entity.SortOrder,
                entity.CreatedAt,
                entity.IsDeleted,
                entity.DeletedAt
            );
        }

        protected override Tutorial MapToEntity(ATutorialCreate dto)
        {
            return new Tutorial
            {
                MethodId = dto.MethodId,
                TheoryTitle = dto.TheoryTitle,
                TheoryContent = dto.TheoryContent,
                SortOrder = dto.SortOrder,
                CreatedAt = DateTime.UtcNow
            };
        }

        protected override void UpdateEntity(Tutorial entity, ATutorialUpdate dto)
        {
            base.UpdateEntity(entity, dto);

            entity.MethodId = dto.MethodId;
            entity.TheoryTitle = dto.TheoryTitle;
            entity.TheoryContent = dto.TheoryContent;
            entity.SortOrder = dto.SortOrder;
        }
    }
}