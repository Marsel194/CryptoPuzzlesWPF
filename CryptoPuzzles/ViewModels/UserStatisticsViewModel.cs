using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class UserStatisticsViewModel : EntityViewModelBase<AUserStatistic, object, object>
    {
        public UserStatisticsViewModel(UserStatisticsApiService apiService) : base(apiService) { }

        protected override AUserStatistic CreateNewItem() =>
            throw new NotSupportedException("Статистика только для чтения");

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
            x.LastActive == y.LastActive;

        protected override bool FilterPredicate(AUserStatistic item)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) return true;
            var f = FilterText.ToLower();
            return item.UserLogin?.ToLower().Contains(f) == true ||
                   item.Username?.ToLower().Contains(f) == true ||
                   item.Email?.ToLower().Contains(f) == true;
        }
    }
}