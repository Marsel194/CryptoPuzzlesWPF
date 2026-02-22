using Hairulin_02_01.ViewModels.Base;

namespace Hairulin_02_01.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

        public RegisterViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}