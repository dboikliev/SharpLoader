using SharpLoader.Services;
using SharpLoader.DependencyInjection;

namespace SharpLoader.DependencyInjection
{
    public sealed class DependencyConfigurator : IDependencyConfigurator
    {
        public static DependencyConfigurator Instance { get; private set; }

        static DependencyConfigurator()
        {
            Instance = new DependencyConfigurator();
        }

        private DependencyConfigurator() { }

        public void InitializeContainer()
        {
            DependencyResolver.Instance.RegisterType<IUrlService, UrlService>();
            DependencyResolver.Instance.RegisterType<IVideoInfoService, VideoInfoService>();
            DependencyResolver.Instance.RegisterType<IDialogService, DialogService>();
            DependencyResolver.Instance.RegisterType<IDownloaderService, DownloaderService>();
            DependencyResolver.Instance.RegisterType<INotificationService, NotificationService>();
        }
    }
}
