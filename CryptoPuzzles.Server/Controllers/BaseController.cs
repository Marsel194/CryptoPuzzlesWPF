using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CryptoPuzzles.Server.Controllers
{
    public abstract class BaseController<TEntity, TDto, TCreateDto, TUpdateDto> : ControllerBase
        where TEntity : class, IEntityWithId, ISoftDelete, new()
        where TUpdateDto : class, IHasId
    {
        protected readonly AppDbContext _context;
        private readonly Dictionary<string, PropertyInfo> _entityProps;
        private readonly Dictionary<string, PropertyInfo> _dtoProps;

        protected BaseController(AppDbContext context)
        {
            _context = context;

            _entityProps = typeof(TEntity).GetProperties()
                .Where(p => p.CanWrite)
                .ToDictionary(p => p.Name, p => p);

            _dtoProps = typeof(TUpdateDto).GetProperties()
                .Where(p => p.Name != nameof(IHasId.Id))
                .ToDictionary(p => p.Name, p => p);
        }

        protected abstract TDto MapToDto(TEntity entity);
        protected abstract TEntity MapToEntity(TCreateDto dto);

        protected virtual void UpdateEntity(TEntity entity, TUpdateDto dto)
        {
            foreach (var dtoProp in _dtoProps)
            {
                if (_entityProps.TryGetValue(dtoProp.Key, out var entityProp))
                {
                    var value = dtoProp.Value.GetValue(dto);
                    entityProp.SetValue(entity, value);
                }
            }
        }

        protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query)
        {
            return query;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll([FromQuery] bool includeDeleted = false)
        {
            var query = _context.Set<TEntity>().AsQueryable();
            query = ApplyIncludes(query);
            if (!includeDeleted)
                query = query.Where(e => !e.IsDeleted);

            var entities = await query
                .OrderBy(e => e.Id)
                .ToListAsync();
            return Ok(entities.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TDto>> Get(int id, [FromQuery] bool includeDeleted = false)
        {
            var query = _context.Set<TEntity>().AsQueryable();
            query = ApplyIncludes(query);
            var entity = await query
                .FirstOrDefaultAsync(e => e.Id == id && (includeDeleted || !e.IsDeleted));
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

            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
                return NotFound();

            UpdateEntity(entity, dto);

            if (entity.IsDeleted && entity.DeletedAt == null)
                entity.DeletedAt = DateTime.UtcNow;
            if (!entity.IsDeleted)
                entity.DeletedAt = null;

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