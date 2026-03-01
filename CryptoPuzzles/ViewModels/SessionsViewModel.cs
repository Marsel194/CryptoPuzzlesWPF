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
    }
}