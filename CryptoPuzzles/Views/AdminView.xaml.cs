using CryptoPuzzles.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CryptoPuzzles.Views
{
    public partial class AdminView : UserControl
    {
        public AdminView()
        {
            InitializeComponent();
            //DataContext = App.Services.GetRequiredService<AdminViewModel>();
        }
    }
}
