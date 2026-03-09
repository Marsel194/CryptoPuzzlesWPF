using CryptoPuzzles.Shared;
using System.Text.Json.Serialization;

namespace CryptoPuzzles.Server.Models
{
    public class UserStatistic
    {
        [ExportName("ID пользователя")]
        public int UserId { get; set; }

        [ExportName("Всего сессий")]
        public int TotalSessions { get; set; } = 0;

        [ExportName("Всего решено головоломок")]
        public int TotalPuzzlesSolved { get; set; } = 0;

        [ExportName("Всего очков")]
        public int TotalScore { get; set; } = 0;

        [ExportName("Всего использовано подсказок")]
        public int TotalHintsUsed { get; set; } = 0;

        [ExportName("Средний балл за сессию")]
        public decimal AvgScorePerSession { get; set; } = 0;

        [ExportName("Последняя активность")]
        public DateTime? LastActive { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; } = null!;
    }
}