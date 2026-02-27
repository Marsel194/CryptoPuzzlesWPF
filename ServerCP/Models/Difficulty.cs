namespace CryptoPuzzles.Server.Models
{
    public class Difficulty
    {
        public int Id { get; set; }
        public string DifficultyName { get; set; } = string.Empty;

        public virtual ICollection<Puzzle> Puzzles { get; set; } = [];
    }
}