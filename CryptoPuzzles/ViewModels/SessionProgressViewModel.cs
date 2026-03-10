using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class SessionProgressViewModel : EntityViewModelBase<ASessionProgress, ASessionProgressCreate, ASessionProgressUpdate>
    {
        public SessionProgressViewModel(SessionProgressApiService apiService) : base(apiService) { }

        protected override ASessionProgress CreateNewItem() => new()
        {
            Id = 0,
            SessionId = 0,
            PuzzleId = 0,
            PuzzleOrder = 1,
            Solved = false,
            HintsUsed = 0,
            ScoreEarned = 0,
            StartedAt = DateTime.Now,
            SolvedAt = null
        };

        protected override ASessionProgressCreate MapToCreateDto(ASessionProgress item) =>
            new(item.SessionId, item.PuzzleId, item.PuzzleOrder, item.HintsUsed, item.ScoreEarned);

        protected override ASessionProgressUpdate MapToUpdateDto(ASessionProgress item) =>
            new(item.Id, item.HintsUsed, item.ScoreEarned, item.Solved, item.SolvedAt);

        protected override int GetId(ASessionProgress item) => item.Id;

        protected override async Task AddAsync()
        {
            if (NewItem.SessionId <= 0 || NewItem.PuzzleId <= 0)
            {
                await DialogService.ShowError("ID сессии и головоломки должны быть положительными!");
                return;
            }

            var itemToAdd = new ASessionProgress
            {
                SessionId = NewItem.SessionId,
                PuzzleId = NewItem.PuzzleId,
                PuzzleOrder = NewItem.PuzzleOrder,
                StartedAt = DateTime.Now
            };
            Items.Add(itemToAdd);
            _addedItems.Add(itemToAdd);
            NewItem = CreateNewItem();
        }

        protected override bool IsEqual(ASessionProgress x, ASessionProgress y) =>
            x.Id == y.Id &&
            x.SessionId == y.SessionId &&
            x.PuzzleId == y.PuzzleId &&
            x.PuzzleOrder == y.PuzzleOrder &&
            x.Solved == y.Solved &&
            x.HintsUsed == y.HintsUsed &&
            x.ScoreEarned == y.ScoreEarned &&
            x.SolvedAt == y.SolvedAt;

        protected override bool FilterPredicate(ASessionProgress item)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) return true;
            var f = FilterText.ToLower();
            return item.UserLogin?.ToLower().Contains(f) == true ||
                   item.Username?.ToLower().Contains(f) == true ||
                   item.PuzzleTitle?.ToLower().Contains(f) == true ||
                   item.SessionId.ToString().Contains(f);
        }
    }
}