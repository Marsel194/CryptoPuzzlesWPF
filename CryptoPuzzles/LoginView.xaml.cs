using Hairulin_02_01.Services;
using System.Windows;
using System.Windows.Controls;

namespace Hairulin_02_01
{
    public partial class LoginView : UserControl
    {
        private readonly ApiService _apiService;

        public LoginView()
        {
            InitializeComponent();
            Loaded += LoginView_Loaded;
            _apiService = new ApiService();
        }

        private async void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            await IsServerAliveAsync();
        }

        private async Task<bool> IsServerAliveAsync(bool showMessage = true)
        {
            bool isAlive = await _apiService.IsServerAliveAsync();

            if (!isAlive && showMessage)
                MessageBox.Show("Сервер не отвечает. Проверьте интернет или попробуйте позже");

            return isAlive;
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                btnLogin.IsEnabled = false;
                btnLogin.Content = "Вход...";

                if (!await _apiService.IsServerAliveAsync()) return;

                var response = await _apiService.LoginAsync(
                    login,
                    password
                );

                MessageBox.Show($"Добро пожаловать, {response.Login}!");

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    if (mainWindow.DataContext is MainViewModel viewModel)
                    {
                        viewModel.ShowMainApp();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                btnLogin.IsEnabled = true;
                btnLogin.Content = "Войти";
            }
        }
    }
}