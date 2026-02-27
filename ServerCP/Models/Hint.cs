namespace CryptoPuzzles.Server.Models
{
    public class Hint
    {
        public int Id { get; set; }
        public int PuzzleId { get; set; }
        public string HintText { get; set; } = string.Empty;
        public int HintOrder { get; set; } = 1;
        public DateTime CreatedAt { get; set; }

        public virtual Puzzle Puzzle { get; set; } = null!;
    }
}