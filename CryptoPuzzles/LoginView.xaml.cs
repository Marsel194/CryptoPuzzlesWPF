using Hairulin_02_01.Services;
using System.Windows;
using System.Windows.Controls;

namespace Hairulin_02_01
{
    public partial class LoginView : UserControl
    {
        private readonly ApiService apiService;

        public LoginView()
        {
            InitializeComponent();
            Loaded += LoginView_Loaded;
            apiService = new ApiService();
        }

        private async void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            await IsServerAliveAsync();
        }

        private async Task IsServerAliveAsync()
        {
            if (!await apiService.IsServerAliveAsync())
                MessageBox.Show("Сервер не отвечает. Проверьте интернет или попробуйте позже");
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