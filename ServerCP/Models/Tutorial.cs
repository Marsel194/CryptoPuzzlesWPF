using CryptoPuzzles.Shared;
using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class Tutorial : IEntityWithId, ISoftDelete, IHasCreatedAt
    {
        public Tutorial() { }

        public int Id { get; set; }

        public int MethodId { get; set; }

        [MaxLength(150)]
        public string TheoryTitle { get; set; } = string.Empty;

        public string TheoryContent { get; set; } = string.Empty;

        public int SortOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual EncryptionMethod Method { get; set; } = null!;
    }
}