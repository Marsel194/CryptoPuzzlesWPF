using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Server.Repositories;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 1;
            argon2.MemorySize = 65536;
            argon2.Iterations = 3;

            byte[] hash = argon2.GetBytes(32);

            byte[] hashBytes = new byte[16 + 32];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }

        // Пример: регистрация с проверкой логина через репозиторий
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user)
        {
            // 1. Проверяем, существует ли пользователь с таким логином
            var existingUser = await _userRepository.GetByLoginAsync(user.Login);
            if (existingUser != null)
                return BadRequest(new { message = "Логин уже занят" });

            // 2. Хэшируем пароль
            user.PasswordHash = HashPassword(user.PasswordHash);

            // 3. Создаём пользователя через репозиторий
            var createdUser = await _userRepository.CreateAsync(user);

            // 4. Возвращаем пользователя без пароля (лучше через DTO, но для примера так)
            createdUser.PasswordHash = string.Empty;
            return Ok(createdUser);
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            // Убираем пароль перед отправкой (в реальном проекте используй DTO)
            foreach (var u in users)
                u.PasswordHash = string.Empty;
            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.PasswordHash = string.Empty;
            return Ok(user);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            if (id != user.Id)
                return BadRequest();

            if (!await _userRepository.ExistsAsync(id))
                return NotFound();

            // Если пароль пришёл, возможно, его нужно захэшировать заново
            // Лучше принимать DTO, но пока упростим
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = HashPassword(user.PasswordHash);
            }
            else
            {
                // Если пароль не передан, оставляем старый — нужно получить текущего пользователя
                var existing = await _userRepository.GetByIdAsync(id);
                if (existing != null)
                    user.PasswordHash = existing.PasswordHash;
            }

            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _userRepository.ExistsAsync(id))
                return NotFound();

            await _userRepository.DeleteAsync(id);
            return NoContent();
        }

        // Дополнительный метод: поиск по логину (если понадобится)
        [HttpGet("by-login/{login}")]
        public async Task<ActionResult<User>> GetByLogin(string login)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null)
                return NotFound();

            user.PasswordHash = string.Empty;
            return Ok(user);
        }
    }
}