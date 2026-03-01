using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Server.Repositories;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

using CryptoPuzzles.Server.Helpers;

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

        [HttpPost("register")]
        public async Task<ActionResult<UARegisterResponse>> RegisterAsync([FromBody] UARegisterRequest request)
        {
            {
                if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Username) ||
                    string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new UAErrorResponse("Все поля обязательны", null));
                }

                if (!request.Email.Contains('@'))
                    return BadRequest(new UAErrorResponse("Некорректный email", null));

                var existingUser = await _userRepository.GetByLoginAsync(request.Login);
                if (existingUser != null)
                    return BadRequest(new UAErrorResponse("Логин уже занят", null));

                var user = new User
                {
                    Login = request.Login,
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = Argon2PasswordActions.HashPassword(request.Password)
                };

                var createdUser = await _userRepository.CreateAsync(user);

                return Ok(new UARegisterResponse(
                    createdUser.Id,
                    createdUser.Login,
                    createdUser.Username,
                    createdUser.Email));
            }
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UUser>>> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UUser>> Get(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

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
                user.PasswordHash = Argon2PasswordActions.HashPassword(user.PasswordHash);
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