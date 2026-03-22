using Microsoft.AspNetCore.Mvc;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;
using CryptoPuzzles.Server.Helpers;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : BaseController<Admin, AAdmin, AAdminCreate, AAdminUpdate>
    {
        public AdminsController(AppDbContext context) : base(context) { }

        protected override AAdmin MapToDto(Admin entity)
        {
            return new AAdmin(
                entity.Id,
                entity.Login,
                entity.FirstName,
                entity.LastName,
                entity.MiddleName ?? "",
                entity.CreatedAt,
                entity.IsDeleted,
                entity.DeletedAt
            );
        }

        protected override Admin MapToEntity(AAdminCreate dto)
        {
            return new Admin
            {
                Login = dto.Login,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MiddleName = dto.MiddleName,
                PasswordHash = Argon2PasswordActions.HashPassword(dto.Password),
                CreatedAt = DateTime.Now
            };
        }

        protected override void UpdateEntity(Admin entity, AAdminUpdate dto)
        {
            // Copy common properties (IsDeleted/DeletedAt etc.) from DTO to entity
            base.UpdateEntity(entity, dto);

            // Apply admin-specific fields
            entity.Login = dto.Login;
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.MiddleName = dto.MiddleName;
            if (!string.IsNullOrWhiteSpace(dto.Password))
                entity.PasswordHash = Argon2PasswordActions.HashPassword(dto.Password);
        }
    }
}