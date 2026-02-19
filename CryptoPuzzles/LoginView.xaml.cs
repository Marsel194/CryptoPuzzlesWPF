using Hairulin_02_01.Services;
using System.Windows;
using System.Windows.Controls;

namespace Hairulin_02_01
{
    public partial class LoginView : UserControl
    {
        ApiService apiService;
        public LoginView()
        {
            InitializeComponent();
            Loaded += LoginView_Loaded;
            apiService = new ApiService();
        }

        private async void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckServerAndDatabaseAsync();
        }

        private async Task CheckServerAndDatabaseAsync()
        {
            try
            {
                var (canConnect, message) = await apiService.CheckDatabaseConnectionAsync();

                if (!canConnect)
                {
                    var result = MessageBox.Show(
                        $"Сервер: {message}\n\nХотите попробовать снова?",
                        "Ошибка подключения",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        await CheckServerAndDatabaseAsync();
                        return;
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении к серверу: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
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
                var user = await apiService.LoginAsync(login, password);
                MessageBox.Show($"Добро пожаловать, {user.Login}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

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
                MessageBox.Show(ex.Message, "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}