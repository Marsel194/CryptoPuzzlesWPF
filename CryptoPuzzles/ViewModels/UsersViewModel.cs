using Hairulin_02_01.Models;
using Hairulin_02_01.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hairulin_02_01.ViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        private User _selectedUser;
        public User SelectedUser
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
            // Загрузка из БД
            Users = new ObservableCollection<User>
            {
                new User { Id = 1, Login = "user1", Email = "user1@test.com", CreatedAt = DateTime.Now },
                new User { Id = 2, Login = "user2", Email = "user2@test.com", CreatedAt = DateTime.Now }
            };
        }

        private void AddUser()
        {
            SelectedUser = new User();
            // Открыть диалог добавления
        }

        private void EditUser()
        {
            // Открыть диалог редактирования
        }

        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                Users.Remove(SelectedUser);
                // Удаление из БД
            }
        }

        private void SaveUser()
        {
            // Сохранение в БД
        }
    }
}