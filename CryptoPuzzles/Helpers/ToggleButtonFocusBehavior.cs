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
            AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
            AssociatedObject.Checked += OnCheckedChanged;
            AssociatedObject.Unchecked += OnCheckedChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseDown -= OnPreviewMouseDown;
            AssociatedObject.Checked -= OnCheckedChanged;
            AssociatedObject.Unchecked -= OnCheckedChanged;
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _lastFocusedElement = Keyboard.FocusedElement;
        }

        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (_lastFocusedElement == null) return;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Keyboard.Focus(_lastFocusedElement);
            }), System.Windows.Threading.DispatcherPriority.Input);
        }
    }
}