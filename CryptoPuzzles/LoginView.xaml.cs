using Isopoh.Cryptography.Argon2;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hairulin_02_01
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            this.Loaded += LoginView_Loaded;
        }

        private async void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckDatabaseConnectionAsync();
        }

        private async System.Threading.Tasks.Task CheckDatabaseConnectionAsync()
        {
            /*try
            {
                using var db = new AppDbContext();
                await db.Database.EnsureCreatedAsync();

                if (!await db.Database.CanConnectAsync())
                {
                    var result = MessageBox.Show(
                        "Не удалось подключиться к базе данных. Хотите создать новую базу данных?",
                        "Ошибка подключения",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        await db.Database.EnsureCreatedAsync();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка при подключении к базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var db = new AppDbContext();

                var user = db.Users.FirstOrDefault(u => u.Login == login);

                if (user == null || !Argon2.Verify(user.PasswordHash, password))
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show($"Добро пожаловать, {user.Username}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    if (mainWindow.DataContext is MainViewModel viewModel)
                    {
                        viewModel.ShowMainApp();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }
    }
}