using CryptoPuzzles.Services;
using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;

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
            // Для сессий, вероятно, не нужно добавлять вручную
            DialogService.ShowMessage("Создание сессий вручную не поддерживается.");
            await Task.CompletedTask;
        }

        protected override async Task SaveAsync()
        {
            // Для сессий разрешим только удаление (изменять не нужно)
            DialogService.ShowMessage("Редактирование сессий не поддерживается. Можно только удалять.");
            // Тем не менее, можно разрешить удаление, но не сохранение изменений.
            // Вызов base.SaveAsync приведёт к попытке обновления, поэтому лучше не вызывать.
            // Альтернатива: переопределить SaveCommand, чтобы он ничего не делал.
        }

        protected override bool IsEqual(AGameSession x, AGameSession y)
        {
            // Сессии обычно не редактируются, поэтому можно всегда возвращать true (нет изменений)
            // Или сравнивать только ключевые поля.
            return x.Id == y.Id &&
                   x.Score == y.Score &&
                   x.CurrentPuzzleId == y.CurrentPuzzleId &&
                   x.TrainingCompleted == y.TrainingCompleted &&
                   x.HintsUsed == y.HintsUsed &&
                   x.CompletedAt == y.CompletedAt;
        }
    }
}