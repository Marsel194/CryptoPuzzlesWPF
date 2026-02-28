using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using CryptoPuzzles.ViewModels;

namespace CryptoPuzzles.Views
{
    public partial class AdminsView : UserControl
    {
        public AdminsView()
        {
            InitializeComponent();
            //DataContext = App.Services.GetRequiredService<AdminsViewModel>();
        }
    }
}
