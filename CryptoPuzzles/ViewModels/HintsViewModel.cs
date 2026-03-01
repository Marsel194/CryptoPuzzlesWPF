using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using System.Collections.ObjectModel;

namespace CryptoPuzzles.ViewModels
{
    public class HintsViewModel : EntityViewModelBase<AHint, AHintCreate, AHintUpdate>
    {
        private readonly PuzzleApiService _puzzleApi;
        private ObservableCollection<APuzzle> _puzzles;

        public HintsViewModel(HintApiService hintApi, PuzzleApiService puzzleApi) : base(hintApi)
        {
            _puzzleApi = puzzleApi;
            LoadPuzzlesAsync();
        }

        public ObservableCollection<APuzzle> Puzzles { get => _puzzles; set => SetProperty(ref _puzzles, value); }

        private async Task LoadPuzzlesAsync()
        {
            Puzzles = new ObservableCollection<APuzzle>(await _puzzleApi.GetAllAsync());
        }

        protected override AHint CreateNewItem()
        {
            return new AHint(0, 0, "", "", 1, DateTime.Now);
        }

        protected override AHintCreate MapToCreateDto(AHint item)
        {
            return new AHintCreate(item.PuzzleId, item.HintText, item.HintOrder);
        }

        protected override AHintUpdate MapToUpdateDto(AHint item)
        {
            return new AHintUpdate(item.Id, item.PuzzleId, item.HintText, item.HintOrder);
        }

        protected override int GetId(AHint item) => item.Id;
    }
}