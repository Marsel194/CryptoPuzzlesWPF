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
            LoadUsersCommand = new AsyncRelayCommand(_ => LoadUsers());
            AddUserCommand = new AsyncRelayCommand(_ => AddUser());
            EditUserCommand = new AsyncRelayCommand(_ => EditUser(), _ => SelectedUser != null);
            DeleteUserCommand = new AsyncRelayCommand(_ => DeleteUser(), _ => SelectedUser != null);
            SaveUserCommand = new AsyncRelayCommand(_ => SaveUser());

            _ = LoadUsers();
        }

        private async Task LoadUsers()
        {
            await Task.CompletedTask;
        }

        private async Task AddUser()
        {
            await Task.CompletedTask;
        }

        private async Task EditUser()
        {
            await Task.CompletedTask;
        }

        private async Task DeleteUser()
        {
            await Task.CompletedTask;
        }

        private async Task SaveUser()
        {
            await Task.CompletedTask;
        }
    }
}