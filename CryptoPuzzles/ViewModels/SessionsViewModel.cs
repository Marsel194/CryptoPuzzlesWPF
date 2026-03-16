using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class SessionsViewModel : EntityViewModelBase<AGameSession, AGameSessionCreate, AGameSessionUpdate>
    {
        private bool _showDeleted = true;
        private string _userFilter = string.Empty;
        private string _typeFilter = string.Empty;

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

        protected override bool HasAdditionalFilters() => !ShowDeleted || !string.IsNullOrWhiteSpace(UserFilter) || !string.IsNullOrWhiteSpace(TypeFilter);

        protected override bool FilterPredicate(AGameSession item)
        {
            if (!ShowDeleted && item.IsDeleted)
                return false;

            bool userMatch = string.IsNullOrWhiteSpace(UserFilter) ||
                             (!string.IsNullOrEmpty(item.UserLogin) && item.UserLogin.Contains(UserFilter, StringComparison.OrdinalIgnoreCase)) ||
                             (!string.IsNullOrEmpty(item.Username) && item.Username.Contains(UserFilter, StringComparison.OrdinalIgnoreCase));

            bool typeMatch = string.IsNullOrWhiteSpace(TypeFilter) ||
                             item.SessionType.Contains(TypeFilter, StringComparison.OrdinalIgnoreCase);

            return userMatch && typeMatch;
        }
    }
}