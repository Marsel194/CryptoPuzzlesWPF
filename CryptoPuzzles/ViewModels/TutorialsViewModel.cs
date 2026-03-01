using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using System.Collections.ObjectModel;

namespace CryptoPuzzles.ViewModels
{
    public class TutorialsViewModel : EntityViewModelBase<ATutorial, ATutorialCreate, ATutorialUpdate>
    {
        private readonly EncryptionMethodApiService _methodApi;
        private ObservableCollection<AEncryptionMethod> _methods;

        public TutorialsViewModel(TutorialApiService tutorialApi, EncryptionMethodApiService methodApi) : base(tutorialApi)
        {
            _methodApi = methodApi;
            LoadMethodsAsync();
        }

        public ObservableCollection<AEncryptionMethod> Methods { get => _methods; set => SetProperty(ref _methods, value); }

        private async Task LoadMethodsAsync()
        {
            Methods = new ObservableCollection<AEncryptionMethod>(await _methodApi.GetAllAsync());
        }

        protected override ATutorial CreateNewItem()
        {
            return new ATutorial(0, 0, "", "", "", 0, true, DateTime.Now, DateTime.Now);
        }

        protected override ATutorialCreate MapToCreateDto(ATutorial item)
        {
            return new ATutorialCreate(item.MethodId, item.TheoryTitle, item.TheoryContent, item.SortOrder, item.IsActive);
        }

        protected override ATutorialUpdate MapToUpdateDto(ATutorial item)
        {
            return new ATutorialUpdate(item.Id, item.MethodId, item.TheoryTitle, item.TheoryContent, item.SortOrder, item.IsActive);
        }

        protected override int GetId(ATutorial item) => item.Id;
    }
}