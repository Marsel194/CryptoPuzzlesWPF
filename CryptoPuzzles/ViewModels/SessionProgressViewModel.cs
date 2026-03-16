using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class SessionProgressViewModel : EntityViewModelBase<ASessionProgress, ASessionProgressCreate, ASessionProgressUpdate>
    {
        private bool _showDeleted = true;
        private int? _sessionIdFilter;
        private string _userFilter = string.Empty;
        private string _puzzleFilter = string.Empty;

        public SessionProgressViewModel(SessionProgressApiService apiService) : base(apiService) { }

        public bool ShowDeleted
        {
            get => _showDeleted;
            set
            {
                if (SetProperty(ref _showDeleted, value))
                    ApplyFilter();
            }
        }

        public int? SessionIdFilter
        {
            get => _sessionIdFilter;
            set
            {
                if (SetProperty(ref _sessionIdFilter, value))
                    ApplyFilter();
            }
        }

        public string UserFilter
        {
            get => _userFilter;
            set
            {
                if (SetProperty(ref _userFilter, value))
                    ApplyFilter();
            }
        }

        public string PuzzleFilter
        {
            get => _puzzleFilter;
            set
            {
                if (SetProperty(ref _puzzleFilter, value))
                    ApplyFilter();
            }
        }

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
            new(item.Id, item.HintsUsed, item.ScoreEarned, item.Solved, item.SolvedAt, item.IsDeleted, item.DeletedAt);

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
            x.SolvedAt == y.SolvedAt &&
            x.IsDeleted == y.IsDeleted &&
            x.DeletedAt == y.DeletedAt;

        protected override bool HasAdditionalFilters() => !ShowDeleted || SessionIdFilter.HasValue || !string.IsNullOrWhiteSpace(UserFilter) || !string.IsNullOrWhiteSpace(PuzzleFilter);

        protected override bool FilterPredicate(ASessionProgress item)
        {
            if (!ShowDeleted && (item.IsDeleted ?? false))
                return false;

            if (SessionIdFilter.HasValue && item.SessionId != SessionIdFilter.Value)
                return false;

            bool userMatch = string.IsNullOrWhiteSpace(UserFilter) ||
                             (item.UserLogin?.Contains(UserFilter, StringComparison.OrdinalIgnoreCase) == true) ||
                             (item.Username?.Contains(UserFilter, StringComparison.OrdinalIgnoreCase) == true);

            bool puzzleMatch = string.IsNullOrWhiteSpace(PuzzleFilter) ||
                               (item.PuzzleTitle?.Contains(PuzzleFilter, StringComparison.OrdinalIgnoreCase) == true);

            return userMatch && puzzleMatch;
        }
    }
}