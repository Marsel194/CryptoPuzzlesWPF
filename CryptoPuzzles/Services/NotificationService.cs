using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace CryptoPuzzles.Services
{
    public static class NotificationService
    {
        private static readonly Queue<string> _recentMessages = new();
        private static readonly TimeSpan _duplicateWindow = TimeSpan.FromSeconds(2);
        private static DateTime _lastMessageTime = DateTime.MinValue;

        private static Snackbar FindSnackbar()
        {
            if (Application.Current.MainWindow is not Window mainWindow)
                return null!;
            return FindVisualChild<Snackbar>(mainWindow);
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                    return tChild;
                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null!;
        }

        public static void ShowInfo(string message)
        {
            ShowMessage(message, "MaterialDesignSuccessBrush");
        }

        public static void ShowWarning(string message)
        {
            ShowMessage(message, "MaterialDesignWarningBrush");
        }

        public static void ShowError(string message)
        {
            ShowMessage(message, "MaterialDesignErrorBrush");
        }

        private static void ShowMessage(string message, string brushKey)
        {
            var now = DateTime.Now;
            if (_recentMessages.Contains(message) && (now - _lastMessageTime) < _duplicateWindow)
            {
                ShowGroupedMessage(message, brushKey);
                return;
            }

            while (_recentMessages.Count > 5)
                _recentMessages.Dequeue();

            _recentMessages.Enqueue(message);
            _lastMessageTime = now;

            ShowSnackbar(message, brushKey);
        }

        private static void ShowGroupedMessage(string message, string brushKey)
        {
            var groupedMessage = $"⚠️ Повтор: {message}";
            ShowSnackbar(groupedMessage, brushKey);
        }

        private static void ShowSnackbar(string message, string brushKey)
        {
            var snackbar = FindSnackbar();
            if (snackbar == null)
                return;

            try
            {
                var background = (SolidColorBrush)Application.Current.FindResource(brushKey);
                var foreground = (SolidColorBrush)Application.Current.FindResource("MaterialDesignBody");

                var border = new Border
                {
                    Background = background,
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16, 10, 16, 10),
                    Child = new TextBlock
                    {
                        Text = message,
                        Foreground = foreground,
                        FontWeight = FontWeights.SemiBold,
                        FontSize = 14
                    }
                };

                snackbar.MessageQueue?.Clear();
                snackbar.MessageQueue?.Enqueue(border, null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            catch
            {
                var fallbackColor = brushKey.Contains("Error") ? Colors.Red :
                                   brushKey.Contains("Warning") ? Colors.Orange : Colors.Green;

                var border = new Border
                {
                    Background = new SolidColorBrush(fallbackColor),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16, 10, 16, 10),
                    Child = new TextBlock
                    {
                        Text = message,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.SemiBold,
                        FontSize = 14
                    }
                };

                snackbar.MessageQueue?.Clear();
                snackbar.MessageQueue?.Enqueue(border, null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        public static void ClearQueue()
        {
            var snackbar = FindSnackbar();
            snackbar?.MessageQueue?.Clear();
            _recentMessages.Clear();
        }
    }
}