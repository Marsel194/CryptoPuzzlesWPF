using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;

namespace CryptoPuzzles.ViewModels
{
    public class AdminsViewModel : EntityViewModelBase<AAdmin, AAdminCreate, AAdminUpdate>
    {
        public AdminsViewModel(AdminApiService apiService) : base(apiService)
        {
        }

        protected override AAdmin CreateNewItem()
        {
            return new AAdmin(0, "", "", "", null, DateTime.Now);
        }

        protected override AAdminCreate MapToCreateDto(AAdmin item)
        {
            return new AAdminCreate(item.Login, "", item.FirstName, item.LastName, item.MiddleName);
        }

        protected override AAdminUpdate MapToUpdateDto(AAdmin item)
        {
            return new AAdminUpdate(item.Id, item.Login, item.FirstName, item.LastName, item.MiddleName, null);
        }

        protected override int GetId(AAdmin item) => item.Id;
    }
}