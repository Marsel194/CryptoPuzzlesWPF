namespace CryptoPuzzles.Server.Models
{
    public class Hint
    {
        public int Id { get; set; }

        public int PuzzleId { get; set; }

        public required string HintText { get; set; }

        public int HintOrder { get; set; } = 1;

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Puzzle Puzzle { get; set; } = null!;
    }
}