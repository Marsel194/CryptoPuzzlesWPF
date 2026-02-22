using Hairulin_02_01.Services;
using Hairulin_02_01.ViewModels;
using Hairulin_02_01.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Hairulin_02_01.Views
{
    public partial class MainWindow : Window
    {
        private readonly NavigationService _navigationService;
        public MainWindow()
        {
            InitializeComponent();

            DataContext = App.Services.GetService<MainViewModel>();

            _navigationService = App.Services.GetService<NavigationService>();

            if (_navigationService != null)
            {
                _navigationService.OnViewChanged += OnViewChanged;
            }

            DataContext = App.Services.GetService<LoginViewModel>();
        }

        private void OnViewChanged(ViewModelBase viewModel)
        {
            DataContext = viewModel;
        }
    }
}
