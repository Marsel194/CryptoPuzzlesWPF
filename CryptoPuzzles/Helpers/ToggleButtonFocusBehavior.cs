using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace CryptoPuzzles.Helpers
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class ToggleButtonFocusBehavior : Behavior<ToggleButton>
    {
        private IInputElement? _lastFocusedElement;

        protected override void OnAttached()
        {
            base.OnAttached();
#pragma warning disable CA1416 // Предупреждения о платформе
            AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
            AssociatedObject.Checked += OnCheckedChanged;
            AssociatedObject.Unchecked += OnCheckedChanged;
#pragma warning restore CA1416
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
#pragma warning disable CA1416
            AssociatedObject.PreviewMouseDown -= OnPreviewMouseDown;
            AssociatedObject.Checked -= OnCheckedChanged;
            AssociatedObject.Unchecked -= OnCheckedChanged;
#pragma warning restore CA1416
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Сохраняем элемент, который имел фокус до клика
            _lastFocusedElement = Keyboard.FocusedElement;
        }

        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            // Возвращаем фокус на сохраненный элемент
            if (_lastFocusedElement != null)
            {
                // Используем Dispatcher для отложенного возврата фокуса
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
#pragma warning disable CA1416
                    Keyboard.Focus(_lastFocusedElement);
#pragma warning restore CA1416
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }
    }
}