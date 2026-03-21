using CryptoPuzzles.Client.Services;
using CryptoPuzzles.Client.Services.ApiService;
using CryptoPuzzles.Client.ViewModels.Base;
using CryptoPuzzles.Shared;

namespace CryptoPuzzles.Client.ViewModels
{
    public class SessionsViewModel : EntityViewModelBase<AGameSession, AGameSessionCreate, AGameSessionUpdate>
    {
        private bool _showDeleted = true;
        private string _userFilter = string.Empty;
        private string _typeFilter = string.Empty;
        private bool? _isCompletedFilter;
        private DateTime? _minSessionStart;
        private DateTime? _maxSessionStart;
        private DateTime? _minCompletedAt;
        private DateTime? _maxCompletedAt;
        private int? _minTotalScore;
        private int? _maxTotalScore;

        public SessionsViewModel(GameSessionApiService apiService) : base(apiService) { }

        public bool ShowDeleted
        {
            get => _showDeleted;
            set
            {
                if (SetProperty(ref _showDeleted, value))
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

        public string TypeFilter
        {
            get => _typeFilter;
            set
            {
                if (SetProperty(ref _typeFilter, value))
                    ApplyFilter();
            }
        }

        public bool? IsCompletedFilter
        {
            get => _isCompletedFilter;
            set
            {
                if (SetProperty(ref _isCompletedFilter, value))
                    ApplyFilter();
            }
        }

        public DateTime? MinSessionStart
        {
            get => _minSessionStart;
            set
            {
                if (SetProperty(ref _minSessionStart, value))
                    ApplyFilter();
            }
        }

        public DateTime? MaxSessionStart
        {
            get => _maxSessionStart;
            set
            {
                if (SetProperty(ref _maxSessionStart, value))
                    ApplyFilter();
            }
        }

        public DateTime? MinCompletedAt
        {
            get => _minCompletedAt;
            set
            {
                if (SetProperty(ref _minCompletedAt, value))
                    ApplyFilter();
            }
        }

        public DateTime? MaxCompletedAt
        {
            get => _maxCompletedAt;
            set
            {
                if (SetProperty(ref _maxCompletedAt, value))
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

        protected override AGameSession CreateNewItem() => new();

        protected override AGameSessionCreate MapToCreateDto(AGameSession item)
        {
            return new AGameSessionCreate(
                UserId: item.UserId,
                SessionType: item.SessionType,
                TotalScore: item.TotalScore
            );
        }

        protected override AGameSessionUpdate MapToUpdateDto(AGameSession item)
        {
            return new AGameSessionUpdate(
                Id: item.Id,
                TotalScore: item.TotalScore,
                IsCompleted: item.IsCompleted,
                CompletedAt: item.CompletedAt,
                CurrentTutorialIndex: null,
                item.IsDeleted,
                item.DeletedAt
            );
        }

        protected override int GetId(AGameSession item) => item.Id;

        protected override async Task AddAsync()
        {
            await DialogService.ShowMessage("Создание сессий вручную не поддерживается.");
        }

        protected override async Task SaveAsync()
        {
            await DialogService.ShowMessage("Редактирование сессий не поддерживается. Можно только удалять.");
        }

        protected override bool IsEqual(AGameSession x, AGameSession y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.Id == y.Id &&
                   x.TotalScore == y.TotalScore &&
                   x.IsCompleted == y.IsCompleted &&
                   x.CompletedAt == y.CompletedAt &&
                   x.UserId == y.UserId &&
                   x.UserLogin == y.UserLogin &&
                   x.SessionStart == y.SessionStart &&
                   x.IsDeleted == y.IsDeleted &&
                   x.DeletedAt == y.DeletedAt;
        }

        protected override bool HasAdditionalFilters() =>
            !ShowDeleted || !string.IsNullOrWhiteSpace(UserFilter) || !string.IsNullOrWhiteSpace(TypeFilter) ||
            IsCompletedFilter.HasValue || MinSessionStart.HasValue || MaxSessionStart.HasValue ||
            MinCompletedAt.HasValue || MaxCompletedAt.HasValue || MinTotalScore.HasValue || MaxTotalScore.HasValue;

        protected override bool FilterPredicate(AGameSession item)
        {
            if (!ShowDeleted && item.IsDeleted)
                return false;

            bool userMatch = string.IsNullOrWhiteSpace(UserFilter) ||
                             (!string.IsNullOrEmpty(item.UserLogin) && item.UserLogin.Contains(UserFilter, StringComparison.OrdinalIgnoreCase)) ||
                             (!string.IsNullOrEmpty(item.Username) && item.Username.Contains(UserFilter, StringComparison.OrdinalIgnoreCase));

            bool typeMatch = string.IsNullOrWhiteSpace(TypeFilter) ||
                             item.SessionType.Contains(TypeFilter, StringComparison.OrdinalIgnoreCase);

            bool completedMatch = !IsCompletedFilter.HasValue || item.IsCompleted == IsCompletedFilter.Value;

            bool startMatch = true;
            if (MinSessionStart.HasValue)
                startMatch = item.SessionStart >= MinSessionStart.Value;
            if (startMatch && MaxSessionStart.HasValue)
                startMatch = item.SessionStart <= MaxSessionStart.Value;

            bool completedDateMatch = true;
            if (MinCompletedAt.HasValue)
                completedDateMatch = item.CompletedAt >= MinCompletedAt.Value;
            if (completedDateMatch && MaxCompletedAt.HasValue)
                completedDateMatch = item.CompletedAt <= MaxCompletedAt.Value;

            bool scoreMatch = true;
            if (MinTotalScore.HasValue)
                scoreMatch = item.TotalScore >= MinTotalScore.Value;
            if (scoreMatch && MaxTotalScore.HasValue)
                scoreMatch = item.TotalScore <= MaxTotalScore.Value;

            return userMatch && typeMatch && completedMatch && startMatch && completedDateMatch && scoreMatch;
        }
    }
}