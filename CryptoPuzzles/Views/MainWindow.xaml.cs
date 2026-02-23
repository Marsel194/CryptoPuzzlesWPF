using Hairulin_02_01.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Hairulin_02_01.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = App.Services.GetService<MainViewModel>();
        }
    }
}
