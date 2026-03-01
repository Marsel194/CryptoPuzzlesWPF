using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;

namespace CryptoPuzzles.ViewModels
{
    public class MethodsViewModel : EntityViewModelBase<AEncryptionMethod, AEncryptionMethodCreate, AEncryptionMethodUpdate>
    {
        public MethodsViewModel(EncryptionMethodApiService apiService) : base(apiService) { }

        protected override AEncryptionMethod CreateNewItem()
        {
            return new AEncryptionMethod(0, "");
        }

        protected override AEncryptionMethodCreate MapToCreateDto(AEncryptionMethod item)
        {
            return new AEncryptionMethodCreate(item.Name);
        }

        protected override AEncryptionMethodUpdate MapToUpdateDto(AEncryptionMethod item)
        {
            return new AEncryptionMethodUpdate(item.Id, item.Name);
        }

        protected override int GetId(AEncryptionMethod item) => item.Id;
    }
}