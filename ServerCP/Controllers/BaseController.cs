using CryptoPuzzles.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    public abstract class BaseController<TEntity, TDto, TCreateDto, TUpdateDto> : ControllerBase
        where TEntity : class, IEntityWithId, ISoftDelete, new()
        where TUpdateDto : class, IHasId
    {
        protected readonly AppDbContext _context;

        protected BaseController(AppDbContext context)
        {
            _context = context;
        }

        protected abstract TDto MapToDto(TEntity entity);
        protected abstract TEntity MapToEntity(TCreateDto dto);
        protected abstract void UpdateEntity(TEntity entity, TUpdateDto dto);

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            var entities = await _context.Set<TEntity>()
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.Id)
                .ToListAsync();
            return Ok(entities.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TDto>> Get(int id)
        {
            var entity = await _context.Set<TEntity>()
                .Where(e => e.Id == id && !e.IsDeleted)
                .FirstOrDefaultAsync();
            if (entity == null)
                return NotFound();
            return Ok(MapToDto(entity));
        }

        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Create(TCreateDto dto)
        {
            var entity = MapToEntity(dto);
            if (entity is IHasCreatedAt withCreated)
                withCreated.CreatedAt = DateTime.UtcNow;
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, MapToDto(entity));
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(int id, TUpdateDto dto)
        {
            if (dto.Id != id)
                return BadRequest("ID в маршруте не совпадает с ID сущности");

            var entity = await _context.Set<TEntity>()
                .Where(e => e.Id == id && !e.IsDeleted)
                .FirstOrDefaultAsync();
            if (entity == null)
                return NotFound();

            UpdateEntity(entity, dto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null || entity.IsDeleted)
                return NotFound();

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}