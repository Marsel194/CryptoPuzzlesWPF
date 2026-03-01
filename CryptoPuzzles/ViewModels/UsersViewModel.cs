using CryptoPuzzles.Services.ApiService;
using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.ViewModels.Base;

namespace CryptoPuzzles.ViewModels
{
    public class UsersViewModel : EntityViewModelBase<AUser, AUserUpdate, AUserUpdate>
    {
        public UsersViewModel(UserApiService apiService) : base(apiService) { }

        protected override AUser CreateNewItem()
        {
            return new AUser(0, "", "", "", DateTime.Now);
        }

        protected override AUserUpdate MapToCreateDto(AUser item)
        {
            return new AUserUpdate(item.Id, item.Login, item.Username, item.Email);
        }

        protected override AUserUpdate MapToUpdateDto(AUser item) => MapToCreateDto(item);
        protected override int GetId(AUser item) => item.Id;

        protected override bool IsEqual(AUser x, AUser y)
        {
            return x.Id == y.Id &&
                   x.Login == y.Login &&
                   x.Username == y.Username &&
                   x.Email == y.Email;
        }
    }
}