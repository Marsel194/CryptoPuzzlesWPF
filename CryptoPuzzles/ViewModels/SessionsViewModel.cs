using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.Shared;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class SessionsViewModel : EntityViewModelBase<AGameSession, AGameSessionCreate, AGameSessionUpdate>
    {
        public SessionsViewModel(GameSessionApiService apiService) : base(apiService) { }

        protected override AGameSession CreateNewItem()
        {
            return new AGameSession
            {
                Id = 0,
                UserId = 0,
                UserLogin = "",
                Score = 0,
                SessionStartTime = DateTime.Now,
                CurrentPuzzleId = null,
                CurrentPuzzleTitle = null,
                TrainingCompleted = false,
                HintsUsed = 0,
                CompletedAt = null
            };
        }

        protected override AGameSessionCreate MapToCreateDto(AGameSession item)
        {
            return new AGameSessionCreate(
                UserId: item.UserId,
                Score: item.Score,
                CurrentPuzzleId: item.CurrentPuzzleId,
                TrainingCompleted: item.TrainingCompleted,
                HintsUsed: item.HintsUsed,
                CompletedAt: item.CompletedAt
            );
        }

        protected override AGameSessionUpdate MapToUpdateDto(AGameSession item)
        {
            return new AGameSessionUpdate(
                Id: item.Id,
                Score: item.Score,
                CurrentPuzzleId: item.CurrentPuzzleId,
                TrainingCompleted: item.TrainingCompleted,
                HintsUsed: item.HintsUsed,
                CompletedAt: item.CompletedAt
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
                   x.Score == y.Score &&
                   x.CurrentPuzzleId == y.CurrentPuzzleId &&
                   x.TrainingCompleted == y.TrainingCompleted &&
                   x.HintsUsed == y.HintsUsed &&
                   x.CompletedAt == y.CompletedAt &&
                   x.UserId == y.UserId &&
                   x.UserLogin == y.UserLogin &&
                   x.SessionStartTime == y.SessionStartTime;    
        }

        protected override bool FilterPredicate(AGameSession item)
        {
            if (string.IsNullOrWhiteSpace(FilterText)) return true;
            var f = FilterText.ToLower();
            return (!string.IsNullOrEmpty(item.UserLogin) && item.UserLogin.ToLower().Contains(f)) ||
                   (!string.IsNullOrEmpty(item.CurrentPuzzleTitle) && item.CurrentPuzzleTitle.ToLower().Contains(f));
        }
    }
}