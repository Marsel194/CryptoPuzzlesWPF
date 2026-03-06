using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class SessionsViewModel : EntityViewModelBase<AGameSession, AGameSessionUpdate, AGameSessionUpdate>
    {
        public SessionsViewModel(GameSessionApiService apiService) : base(apiService) { }

        protected override AGameSession CreateNewItem()
        {
            return new AGameSession(0, 0, "", 0, DateTime.Now, null, null, false, 0, null);
        }

        protected override AGameSessionUpdate MapToCreateDto(AGameSession item)
        {
            return new AGameSessionUpdate(item.Id, item.Score, item.CurrentPuzzleId, item.TrainingCompleted, item.HintsUsed, item.CompletedAt);
        }

        protected override AGameSessionUpdate MapToUpdateDto(AGameSession item) => MapToCreateDto(item);

        protected override int GetId(AGameSession item) => item.Id;

        protected override async Task AddAsync()
        {
            await DialogService.ShowMessage("Создание сессий вручную не поддерживается.");
            await Task.CompletedTask;
        }

        protected override async Task SaveAsync()
        {
            await DialogService.ShowMessage("Редактирование сессий не поддерживается. Можно только удалять.");
        }

        protected override bool IsEqual(AGameSession x, AGameSession y)
        {
            return true;
        }

        protected override bool FilterPredicate(AGameSession item)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) return true;
            var f = FilterText.ToLower();
            return item.UserLogin.ToLower().Contains(f) ||
                   (item.CurrentPuzzleTitle?.ToLower().Contains(f) ?? false);
        }
    }
}