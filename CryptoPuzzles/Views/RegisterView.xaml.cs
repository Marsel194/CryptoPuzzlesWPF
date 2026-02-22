using Hairulin_02_01.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Hairulin_02_01.Views
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
            //DataContext = App.Services.GetService<RegisterViewModel>();
        }
    }
}
