using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Server.Models
{
    public class Hint : IEntityWithId, ISoftDelete, IHasCreatedAt
    {
        public Hint() { }

        public int Id { get; set; }

        public int PuzzleId { get; set; }

        public string HintText { get; set; } = string.Empty;

        public int HintOrder { get; set; } = 1;

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Puzzle Puzzle { get; set; } = null!;
    }
}