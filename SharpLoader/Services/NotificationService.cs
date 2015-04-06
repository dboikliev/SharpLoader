using System.Windows;

namespace SharpLoader.Services
{
    public class NotificationService : INotificationService
    {
        public void ShowErrorNotification(string message)
        {
            MessageBox.Show(message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInfoNotification(string message)
        {
            MessageBox.Show(message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
