using CryptoPuzzles.Services;
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
        private bool? _solvedFilter;
        private int? _minHintsUsed;
        private int? _maxHintsUsed;
        private int? _minScoreEarned;
        private int? _maxScoreEarned;
        private DateTime? _minStartedAt;
        private DateTime? _maxStartedAt;
        private DateTime? _minSolvedAt;
        private DateTime? _maxSolvedAt;

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

        public bool? SolvedFilter
        {
            get => _solvedFilter;
            set
            {
                if (SetProperty(ref _solvedFilter, value))
                    ApplyFilter();
            }
        }

        public int? MinHintsUsed
        {
            get => _minHintsUsed;
            set
            {
                if (SetProperty(ref _minHintsUsed, value))
                    ApplyFilter();
            }
        }

        public int? MaxHintsUsed
        {
            get => _maxHintsUsed;
            set
            {
                if (SetProperty(ref _maxHintsUsed, value))
                    ApplyFilter();
            }
        }

        public int? MinScoreEarned
        {
            get => _minScoreEarned;
            set
            {
                if (SetProperty(ref _minScoreEarned, value))
                    ApplyFilter();
            }
        }

        public int? MaxScoreEarned
        {
            get => _maxScoreEarned;
            set
            {
                if (SetProperty(ref _maxScoreEarned, value))
                    ApplyFilter();
            }
        }

        public DateTime? MinStartedAt
        {
            get => _minStartedAt;
            set
            {
                if (SetProperty(ref _minStartedAt, value))
                    ApplyFilter();
            }
        }

        public DateTime? MaxStartedAt
        {
            get => _maxStartedAt;
            set
            {
                if (SetProperty(ref _maxStartedAt, value))
                    ApplyFilter();
            }
        }

        public DateTime? MinSolvedAt
        {
            get => _minSolvedAt;
            set
            {
                if (SetProperty(ref _minSolvedAt, value))
                    ApplyFilter();
            }
        }

        public DateTime? MaxSolvedAt
        {
            get => _maxSolvedAt;
            set
            {
                if (SetProperty(ref _maxSolvedAt, value))
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

        protected override bool HasAdditionalFilters() =>
            !ShowDeleted || SessionIdFilter.HasValue || !string.IsNullOrWhiteSpace(UserFilter) ||
            !string.IsNullOrWhiteSpace(PuzzleFilter) || SolvedFilter.HasValue ||
            MinHintsUsed.HasValue || MaxHintsUsed.HasValue || MinScoreEarned.HasValue ||
            MaxScoreEarned.HasValue || MinStartedAt.HasValue || MaxStartedAt.HasValue ||
            MinSolvedAt.HasValue || MaxSolvedAt.HasValue;

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

            bool solvedMatch = !SolvedFilter.HasValue || item.Solved == SolvedFilter.Value;

            bool hintsMatch = true;
            if (MinHintsUsed.HasValue)
                hintsMatch = item.HintsUsed >= MinHintsUsed.Value;
            if (hintsMatch && MaxHintsUsed.HasValue)
                hintsMatch = item.HintsUsed <= MaxHintsUsed.Value;

            bool scoreMatch = true;
            if (MinScoreEarned.HasValue)
                scoreMatch = item.ScoreEarned >= MinScoreEarned.Value;
            if (scoreMatch && MaxScoreEarned.HasValue)
                scoreMatch = item.ScoreEarned <= MaxScoreEarned.Value;

            bool startedMatch = true;
            if (MinStartedAt.HasValue)
                startedMatch = item.StartedAt >= MinStartedAt.Value;
            if (startedMatch && MaxStartedAt.HasValue)
                startedMatch = item.StartedAt <= MaxStartedAt.Value;

            bool solvedAtMatch = true;
            if (MinSolvedAt.HasValue)
                solvedAtMatch = item.SolvedAt >= MinSolvedAt.Value;
            if (solvedAtMatch && MaxSolvedAt.HasValue)
                solvedAtMatch = item.SolvedAt <= MaxSolvedAt.Value;

            return userMatch && puzzleMatch && solvedMatch && hintsMatch && scoreMatch && startedMatch && solvedAtMatch;
        }
    }
}