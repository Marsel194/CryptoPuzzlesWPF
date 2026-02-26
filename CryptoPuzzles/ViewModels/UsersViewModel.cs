using CryptoPuzzles.SharedDTO;
using CryptoPuzzles.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CryptoPuzzles.ViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        private ObservableCollection<UUser> _users = [];
        public ObservableCollection<UUser> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        private UUser _selectedUser = new("", "", "");
        public UUser SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public ICommand LoadUsersCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand SaveUserCommand { get; }

        public UsersViewModel()
        {
            LoadUsersCommand = new RelayCommand(_ => LoadUsers());
            AddUserCommand = new RelayCommand(_ => AddUser());
            EditUserCommand = new RelayCommand(_ => EditUser(), _ => SelectedUser != null);
            DeleteUserCommand = new RelayCommand(_ => DeleteUser(), _ => SelectedUser != null);
            SaveUserCommand = new RelayCommand(_ => SaveUser());

            LoadUsers();
        }

        private void LoadUsers()
        {
            
        }

        private void AddUser()
        {
        }

        private void EditUser()
        {
        }

        private void DeleteUser()
        {

        }

        private void SaveUser()
        {

        }
    }
}