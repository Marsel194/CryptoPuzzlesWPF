using CryptoPuzzles.Shared;
using System.ComponentModel.DataAnnotations;

namespace CryptoPuzzles.Server.Models
{
    public class Tutorial
    {
        [ExportName("ID")]
        public int Id { get; set; }

        [ExportName("ID метода")]
        public int MethodId { get; set; }

        [ExportName("Название")]
        [MaxLength(150)]
        public required string TheoryTitle { get; set; }

        [ExportName("Содержимое")]
        public required string TheoryContent { get; set; }

        [ExportName("Какой по счету")]
        public int SortOrder { get; set; } = 0;

        [ExportName("Дата создания")]
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual EncryptionMethod Method { get; set; } = null!;
    }
}