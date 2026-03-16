using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class UserStatisticsViewModel : EntityViewModelBase<AUserStatistic, object, object>
    {
        private bool _showDeleted = true;
        private string _loginFilter = string.Empty;
        private string _nameFilter = string.Empty;

        public UserStatisticsViewModel(UserStatisticsApiService apiService) : base(apiService) { }

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

        protected override bool HasAdditionalFilters() => !ShowDeleted || !string.IsNullOrWhiteSpace(LoginFilter) || !string.IsNullOrWhiteSpace(NameFilter);

        protected override bool FilterPredicate(AUserStatistic item)
        {
            if (!ShowDeleted && item.IsDeleted)
                return false;

            bool loginMatch = string.IsNullOrWhiteSpace(LoginFilter) ||
                              (item.UserLogin?.Contains(LoginFilter, StringComparison.OrdinalIgnoreCase) == true);

            bool nameMatch = string.IsNullOrWhiteSpace(NameFilter) ||
                             (item.Username?.Contains(NameFilter, StringComparison.OrdinalIgnoreCase) == true);

            return loginMatch && nameMatch;
        }
    }
}