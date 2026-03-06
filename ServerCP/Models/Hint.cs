using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Models
{
    public class Hint
    {
        [ExportName("ID")]
        public int Id { get; set; }

        [ExportName("ID пазла ")]
        public int PuzzleId { get; set; }

        [ExportName("Текст подсказки")]
        public required string HintText { get; set; }

        [ExportName("Порядок подсказки")]
        public int HintOrder { get; set; } = 1;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Puzzle Puzzle { get; set; } = null!;
    }
}