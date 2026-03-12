using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class SessionsViewModel : EntityViewModelBase<AGameSession, AGameSessionCreate, AGameSessionUpdate>
    {
        public SessionsViewModel(GameSessionApiService apiService) : base(apiService) { }

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
                CurrentTutorialIndex: null
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
                   x.SessionStart == y.SessionStart;
        }

        protected override bool FilterPredicate(AGameSession item)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) return true;
            var f = FilterText.ToLower();
            return (!string.IsNullOrEmpty(item.UserLogin) && item.UserLogin.ToLower().Contains(f)) ||
                   (!string.IsNullOrEmpty(item.Username) && item.Username.ToLower().Contains(f));
        }
    }
}