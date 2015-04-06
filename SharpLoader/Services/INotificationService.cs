namespace SharpLoader.Services
{
    public interface INotificationService
    {
        void ShowErrorNotification(string message);
        void ShowInfoNotification(string message);
    }
}
