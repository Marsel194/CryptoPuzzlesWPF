using CryptoPuzzles.ViewModels;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoPuzzles.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            //DataContext = App.Services.GetRequiredService<LoginViewModel>();
        }
    }
}