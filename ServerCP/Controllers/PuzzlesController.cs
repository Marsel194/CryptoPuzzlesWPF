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

    protected override void UpdateEntity(Puzzle entity, APuzzleUpdate dto)
    {
        entity.Title = dto.Title;
        entity.Content = dto.Content;
        entity.Answer = dto.Answer;
        entity.MaxScore = dto.MaxScore;
        entity.DifficultyId = dto.DifficultyId;
        entity.MethodId = dto.MethodId;
        entity.IsTraining = dto.IsTraining;
        entity.TutorialOrder = dto.TutorialOrder;
    }

    [HttpGet]
    public override async Task<ActionResult<IEnumerable<APuzzle>>> GetAll()
    {
        var puzzles = await _context.Puzzles
            .Include(p => p.Difficulty)
            .Include(p => p.Method)
            .Include(p => p.CreatedByAdmin)
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Id)
            .Select(p => new APuzzle(
                p.Id,
                p.Title,
                p.Content,
                p.Answer,
                p.MaxScore,
                p.DifficultyId,
                p.Difficulty.DifficultyName,
                p.MethodId,
                p.Method != null ? p.Method.Name : null,
                p.IsTraining,
                p.TutorialOrder,
                p.CreatedByAdminId,
                p.CreatedByAdmin != null ? $"{p.CreatedByAdmin.FirstName} {p.CreatedByAdmin.LastName}" : null,
                p.CreatedAt,
                p.IsDeleted,
                p.DeletedAt
            ))
            .ToListAsync();
        return Ok(puzzles);
    }

    [HttpGet("{id}")]
    public override async Task<ActionResult<APuzzle>> Get(int id)
    {
        var puzzle = await _context.Puzzles
            .Include(p => p.Difficulty)
            .Include(p => p.Method)
            .Include(p => p.CreatedByAdmin)
            .Where(p => p.Id == id && !p.IsDeleted)
            .Select(p => new APuzzle(
                p.Id,
                p.Title,
                p.Content,
                p.Answer,
                p.MaxScore,
                p.DifficultyId,
                p.Difficulty.DifficultyName,
                p.MethodId,
                p.Method != null ? p.Method.Name : null,
                p.IsTraining,
                p.TutorialOrder,
                p.CreatedByAdminId,
                p.CreatedByAdmin != null ? $"{p.CreatedByAdmin.FirstName} {p.CreatedByAdmin.LastName}" : null,
                p.CreatedAt,
                p.IsDeleted,
                p.DeletedAt
            ))
            .FirstOrDefaultAsync();
        if (puzzle == null) return NotFound();
        return Ok(puzzle);
    }
}