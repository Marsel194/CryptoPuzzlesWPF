using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using System.Collections.ObjectModel;

namespace CryptoPuzzles.ViewModels
{
    public class PuzzlesViewModel : EntityViewModelBase<APuzzle, APuzzleCreate, APuzzleUpdate>
    {
        private readonly DifficultyApiService _difficultyApi;
        private readonly EncryptionMethodApiService _methodApi;
        private ObservableCollection<ADifficulty> _difficulties;
        private ObservableCollection<AEncryptionMethod> _methods;

        public PuzzlesViewModel(PuzzleApiService puzzleApi, DifficultyApiService difficultyApi, EncryptionMethodApiService methodApi)
            : base(puzzleApi)
        {
            _difficultyApi = difficultyApi;
            _methodApi = methodApi;
            LoadLookupsAsync();
        }

        public ObservableCollection<ADifficulty> Difficulties { get => _difficulties; set => SetProperty(ref _difficulties, value); }
        public ObservableCollection<AEncryptionMethod> Methods { get => _methods; set => SetProperty(ref _methods, value); }

        private async Task LoadLookupsAsync()
        {
            Difficulties = new ObservableCollection<ADifficulty>(await _difficultyApi.GetAllAsync());
            Methods = new ObservableCollection<AEncryptionMethod>(await _methodApi.GetAllAsync());
        }

        protected override APuzzle CreateNewItem()
        {
            return new APuzzle(0, "", "", "", 50, 0, "", null, null, false, null, null, DateTime.Now);
        }

        protected override APuzzleCreate MapToCreateDto(APuzzle item)
        {
            return new APuzzleCreate(item.Title, item.Content, item.Answer, item.MaxScore,
                item.DifficultyId, item.MethodId, item.IsTraining, item.TutorialOrder);
        }

        protected override APuzzleUpdate MapToUpdateDto(APuzzle item)
        {
            return new APuzzleUpdate(item.Id, item.Title, item.Content, item.Answer, item.MaxScore,
                item.DifficultyId, item.MethodId, item.IsTraining, item.TutorialOrder);
        }

        protected override int GetId(APuzzle item) => item.Id;
    }
}