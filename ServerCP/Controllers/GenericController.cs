using Microsoft.AspNetCore.Mvc;
using CryptoPuzzles.Server.Repositories;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T> : ControllerBase where T : class
    {
        protected readonly IRepository<T> _repository;

        public GenericController(IRepository<T> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll() => Ok(await _repository.GetAllAsync());

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get(int id) => Ok(await _repository.GetByIdAsync(id));

        [HttpPost]
        public virtual async Task<IActionResult> Create(T entity) => Ok(await _repository.CreateAsync(entity));

        [HttpPut("{id}")]
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(int id, T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                return BadRequest("Сущность не содержит поля Id");

            var entityId = idProperty.GetValue(entity);
            if (entityId == null || (int)entityId != id)
                return BadRequest("ID в маршруте не совпадает с ID сущности");

            if (!await _repository.ExistsAsync(id))
                return NotFound();

            await _repository.UpdateAsync(entity);
            return Ok();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return Ok();
        }
    }
}