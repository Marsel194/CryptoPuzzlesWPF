using System.Text.Json.Serialization;

namespace CryptoPuzzles.Server.Models
{
    public class UserStatistic
    {
        public int UserId { get; set; }

        public int TotalSessions { get; set; } = 0;

        public int TotalPuzzlesSolved { get; set; } = 0;

        public int TotalScore { get; set; } = 0;

        public int TotalHintsUsed { get; set; } = 0;

        public decimal AvgScorePerSession { get; set; } = 0;

        public DateTime? LastActive { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; } = null!;
    }
}