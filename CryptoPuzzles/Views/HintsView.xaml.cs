using CryptoPuzzles.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CryptoPuzzles.Views
{
    public partial class HintsView : UserControl
    {
        public HintsView()
        {
            InitializeComponent();
            //DataContext = App.Services.GetRequiredService<HintsViewModel>();
        }
    }
}
