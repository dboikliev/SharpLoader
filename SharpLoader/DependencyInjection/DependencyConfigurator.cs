using FFMPEG.Implementations;
using FFMPEG.Interfaces;
using SharpLoader.Helpers;
using SharpLoader.Services.Contracts;
using SharpLoader.Services.Implementations;

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
            DependencyResolver.Instance.RegisterType<IHtmlDownloader, HtmlDownloader>();
            DependencyResolver.Instance.RegisterType<IHtmlParser, HtmlParser>();
            DependencyResolver.Instance.RegisterType<IFfmpegEncoder, FfmpegEncoder>();
        }
    }
}
