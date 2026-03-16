using CryptoPuzzles.Server.Controllers;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PuzzlesController : BaseController<Puzzle, APuzzle, APuzzleCreate, APuzzleUpdate>
{
    public PuzzlesController(AppDbContext context) : base(context) { }

    protected override APuzzle MapToDto(Puzzle entity)
    {
        return new APuzzle(
            entity.Id,
            entity.Title,
            entity.Content,
            entity.Answer,
            entity.MaxScore,
            entity.DifficultyId,
            entity.Difficulty?.DifficultyName ?? "",
            entity.MethodId,
            entity.Method?.Name,
            entity.IsTraining,
            entity.TutorialOrder,
            entity.CreatedByAdminId,
            entity.CreatedByAdmin != null ? $"{entity.CreatedByAdmin.FirstName} {entity.CreatedByAdmin.LastName}" : null,
            entity.CreatedAt,
            entity.IsDeleted,
            entity.DeletedAt
        );
    }

    protected override Puzzle MapToEntity(APuzzleCreate dto)
    {
        return new Puzzle
        {
            Title = dto.Title,
            Content = dto.Content,
            Answer = dto.Answer,
            MaxScore = dto.MaxScore,
            DifficultyId = dto.DifficultyId,
            MethodId = dto.MethodId,
            IsTraining = dto.IsTraining,
            TutorialOrder = dto.TutorialOrder,
            CreatedAt = DateTime.UtcNow
        };
    }
}