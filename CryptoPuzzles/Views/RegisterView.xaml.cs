using Hairulin_02_01.Services;
using System.Windows;
using System.Windows.Controls;

namespace Hairulin_02_01
{
    public partial class RegisterView : UserControl
    {
        private readonly ApiService _apiService;
        public RegisterView()
        {
            InitializeComponent();
            _apiService = new ApiService(); 
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }
            else if (string.IsNullOrEmpty(txtUserName.Text))
            {
                MessageBox.Show("Введите имя пользователя");
                return;
            }
            else if (password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }

            btnRegister.IsEnabled = false;

            try
            {
                var newUser = new User
                {
                    Login = login,
                    PasswordHash = password,
                    Username = txtUserName.Text,
                    Email = txtEmail.Text
                };

                var registeredUser = await _apiService.RegisterAsync(newUser);

                if (registeredUser != null)
                {
                    MessageBox.Show($"Регистрация успешна! Добро пожаловать, {registeredUser.Username}",
               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
                btnRegister.IsEnabled = true;
            }
        }
    }
}
