using CryptoPuzzles.Client.Services;
using CryptoPuzzles.Client.Services.ApiService;
using CryptoPuzzles.Client.ViewModels.Base;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Client.ViewModels
{
    public class UserStatisticsViewModel(UserStatisticsApiService apiService) : EntityViewModelBase<AUserStatistic, object, object>(apiService)
    {
        private bool _showDeleted = true;
        private string _loginFilter = string.Empty;
        private string _nameFilter = string.Empty;
        private int? _minTotalSessions;
        private int? _maxTotalSessions;
        private int? _minTotalPuzzlesSolved;
        private int? _maxTotalPuzzlesSolved;
        private int? _minTotalScore;
        private int? _maxTotalScore;
        private int? _minTotalHintsUsed;
        private int? _maxTotalHintsUsed;
        private decimal? _minAvgScorePerSession;
        private decimal? _maxAvgScorePerSession;
        private DateTime? _minLastActive;
        private DateTime? _maxLastActive;

        public bool ShowDeleted
        {
            get => _showDeleted;
            set
            {
                if (SetProperty(ref _showDeleted, value))
                    ApplyFilter();
            }
        }

        public string LoginFilter
        {
            get => _loginFilter;
            set
            {
                if (SetProperty(ref _loginFilter, value))
                    ApplyFilter();
            }
        }

        public string NameFilter
        {
            get => _nameFilter;
            set
            {
                if (SetProperty(ref _nameFilter, value))
                    ApplyFilter();
            }
        }

        public int? MinTotalSessions
        {
            get => _minTotalSessions;
            set
            {
                if (SetProperty(ref _minTotalSessions, value))
                    ApplyFilter();
            }
        }

        public int? MaxTotalSessions
        {
            get => _maxTotalSessions;
            set
            {
                if (SetProperty(ref _maxTotalSessions, value))
                    ApplyFilter();
            }
        }

        public int? MinTotalPuzzlesSolved
        {
            get => _minTotalPuzzlesSolved;
            set
            {
                if (SetProperty(ref _minTotalPuzzlesSolved, value))
                    ApplyFilter();
            }
        }

        public int? MaxTotalPuzzlesSolved
        {
            get => _maxTotalPuzzlesSolved;
            set
            {
                if (SetProperty(ref _maxTotalPuzzlesSolved, value))
                    ApplyFilter();
            }
        }

        public int? MinTotalScore
        {
            get => _minTotalScore;
            set
            {
                if (SetProperty(ref _minTotalScore, value))
                    ApplyFilter();
            }
        }

        public int? MaxTotalScore
        {
            get => _maxTotalScore;
            set
            {
                if (SetProperty(ref _maxTotalScore, value))
                    ApplyFilter();
            }
        }

        public int? MinTotalHintsUsed
        {
            get => _minTotalHintsUsed;
            set
            {
                if (SetProperty(ref _minTotalHintsUsed, value))
                    ApplyFilter();
            }
        }

        public int? MaxTotalHintsUsed
        {
            get => _maxTotalHintsUsed;
            set
            {
                if (SetProperty(ref _maxTotalHintsUsed, value))
                    ApplyFilter();
            }
        }

        public decimal? MinAvgScorePerSession
        {
            get => _minAvgScorePerSession;
            set
            {
                if (SetProperty(ref _minAvgScorePerSession, value))
                    ApplyFilter();
            }
        }

        public decimal? MaxAvgScorePerSession
        {
            get => _maxAvgScorePerSession;
            set
            {
                if (SetProperty(ref _maxAvgScorePerSession, value))
                    ApplyFilter();
            }
        }

        public DateTime? MinLastActive
        {
            get => _minLastActive;
            set
            {
                if (SetProperty(ref _minLastActive, value))
                    ApplyFilter();
            }
        }

        public DateTime? MaxLastActive
        {
            get => _maxLastActive;
            set
            {
                if (SetProperty(ref _maxLastActive, value))
                    ApplyFilter();
            }
        }

        protected override AUserStatistic CreateNewItem() => new();

        protected override object MapToCreateDto(AUserStatistic item) =>
            throw new NotSupportedException();

        protected override object MapToUpdateDto(AUserStatistic item) =>
            throw new NotSupportedException();

        protected override int GetId(AUserStatistic item) => item.UserId;

        protected override async Task AddAsync()
        {
            await DialogService.ShowMessage("Статистика формируется автоматически и не может быть добавлена вручную.");
        }

        protected override async Task SaveAsync()
        {
            await DialogService.ShowMessage("Статистика только для чтения.");
        }

        protected override bool IsEqual(AUserStatistic x, AUserStatistic y) =>
            x.UserId == y.UserId &&
            x.TotalSessions == y.TotalSessions &&
            x.TotalPuzzlesSolved == y.TotalPuzzlesSolved &&
            x.TotalScore == y.TotalScore &&
            x.TotalHintsUsed == y.TotalHintsUsed &&
            x.AvgScorePerSession == y.AvgScorePerSession &&
            x.LastActive == y.LastActive &&
            x.IsDeleted == y.IsDeleted &&
            x.DeletedAt == y.DeletedAt;

        protected override bool HasAdditionalFilters() =>
            !ShowDeleted || !string.IsNullOrWhiteSpace(LoginFilter) || !string.IsNullOrWhiteSpace(NameFilter) ||
            MinTotalSessions.HasValue || MaxTotalSessions.HasValue || MinTotalPuzzlesSolved.HasValue ||
            MaxTotalPuzzlesSolved.HasValue || MinTotalScore.HasValue || MaxTotalScore.HasValue ||
            MinTotalHintsUsed.HasValue || MaxTotalHintsUsed.HasValue || MinAvgScorePerSession.HasValue ||
            MaxAvgScorePerSession.HasValue || MinLastActive.HasValue || MaxLastActive.HasValue;

        protected override bool FilterPredicate(AUserStatistic item)
        {
            if (!ShowDeleted && item.IsDeleted)
                return false;

            bool loginMatch = string.IsNullOrWhiteSpace(LoginFilter) ||
                              (item.UserLogin?.Contains(LoginFilter, StringComparison.OrdinalIgnoreCase) == true);

            bool nameMatch = string.IsNullOrWhiteSpace(NameFilter) ||
                             (item.Username?.Contains(NameFilter, StringComparison.OrdinalIgnoreCase) == true);

            bool sessionsMatch = true;
            if (MinTotalSessions.HasValue)
                sessionsMatch = item.TotalSessions >= MinTotalSessions.Value;
            if (sessionsMatch && MaxTotalSessions.HasValue)
                sessionsMatch = item.TotalSessions <= MaxTotalSessions.Value;

            bool solvedMatch = true;
            if (MinTotalPuzzlesSolved.HasValue)
                solvedMatch = item.TotalPuzzlesSolved >= MinTotalPuzzlesSolved.Value;
            if (solvedMatch && MaxTotalPuzzlesSolved.HasValue)
                solvedMatch = item.TotalPuzzlesSolved <= MaxTotalPuzzlesSolved.Value;

            bool scoreMatch = true;
            if (MinTotalScore.HasValue)
                scoreMatch = item.TotalScore >= MinTotalScore.Value;
            if (scoreMatch && MaxTotalScore.HasValue)
                scoreMatch = item.TotalScore <= MaxTotalScore.Value;

            bool hintsMatch = true;
            if (MinTotalHintsUsed.HasValue)
                hintsMatch = item.TotalHintsUsed >= MinTotalHintsUsed.Value;
            if (hintsMatch && MaxTotalHintsUsed.HasValue)
                hintsMatch = item.TotalHintsUsed <= MaxTotalHintsUsed.Value;

            bool avgMatch = true;
            if (MinAvgScorePerSession.HasValue)
                avgMatch = item.AvgScorePerSession >= MinAvgScorePerSession.Value;
            if (avgMatch && MaxAvgScorePerSession.HasValue)
                avgMatch = item.AvgScorePerSession <= MaxAvgScorePerSession.Value;

            bool lastActiveMatch = true;
            if (MinLastActive.HasValue)
                lastActiveMatch = item.LastActive >= MinLastActive.Value;
            if (lastActiveMatch && MaxLastActive.HasValue)
                lastActiveMatch = item.LastActive <= MaxLastActive.Value;

            return loginMatch && nameMatch && sessionsMatch && solvedMatch && scoreMatch && hintsMatch && avgMatch && lastActiveMatch;
        }
    }
}