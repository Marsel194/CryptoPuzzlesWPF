using Isopoh.Cryptography.Argon2;
using System.Windows;
using System.Windows.Controls;

namespace Hairulin_02_01
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
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

            using var _db = new AppDbContext();

            if (_db.Users.Any(s => s.Login == login))
            {
                MessageBox.Show("Логин уже занят");
                return;
            }
            using var db = new AppDbContext();

            var hash = Argon2.Hash(password);

            User user = new()
            {
                Login = txtLogin.Text,
                PasswordHash = hash.ToString(),
                Username = txtUserName.Text,
                Email = txtEmail.Text,
            };

            _db.Users.Add(user);
            _db.SaveChanges();
        }
    }
}
