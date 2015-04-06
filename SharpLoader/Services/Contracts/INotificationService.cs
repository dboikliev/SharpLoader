namespace SharpLoader.Services.Contracts
{
    public interface INotificationService
    {
        void ShowErrorNotification(string message);
        void ShowInfoNotification(string message);
    }
}
