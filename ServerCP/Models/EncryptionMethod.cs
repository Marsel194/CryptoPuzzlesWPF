namespace CryptoPuzzles.Server.Models
{
    public class EncryptionMethod
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Puzzle> Puzzles { get; set; } = [];
        public virtual ICollection<Tutorial> Tutorials { get; set; } = [];
    }
}