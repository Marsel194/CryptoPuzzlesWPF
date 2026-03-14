using Microsoft.AspNetCore.Mvc;
using CryptoPuzzles.Server.Data;
using CryptoPuzzles.Server.Models;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionMethodsController : BaseController<EncryptionMethod, AEncryptionMethod, AEncryptionMethodCreate, AEncryptionMethodUpdate>
    {
        public EncryptionMethodsController(AppDbContext context) : base(context) { }

        protected override AEncryptionMethod MapToDto(EncryptionMethod entity)
        {
            return new AEncryptionMethod(entity.Id, entity.Name);
        }

        protected override EncryptionMethod MapToEntity(AEncryptionMethodCreate dto)
        {
            return new EncryptionMethod { Name = dto.Name };
        }

        protected override void UpdateEntity(EncryptionMethod entity, AEncryptionMethodUpdate dto)
        {
            entity.Name = dto.Name;
        }
    }
}