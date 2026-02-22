using Hairulin_02_01.ViewModels.Base;

namespace Hairulin_02_01.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

        public AdminViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}