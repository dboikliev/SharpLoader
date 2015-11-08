using System.Windows.Input;
using SharpLoader.Commands;

namespace SharpLoader.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public AllDownloadsViewModel AllDownloads { get; } = new AllDownloadsViewModel();
        public AllConvertingViewModel AllConverting { get; } = new AllConvertingViewModel();

        public ICommand BeginDownload { get; private set; }

        public MainViewModel()
        {
            BeginDownload = new CommandWithParameter<string>(InitializeDownload, (_) => true);
        }

        private void InitializeDownload(string videoUrl)
        {
            AllDownloads.DownloadFinished += InitialzieConvert;
            AllDownloads.BeginDownload.Execute(videoUrl);
        }

        private void InitialzieConvert(object sender, Models.Downloader.DownloadFinishedEventArgs e)
        {
            //App.Current.Dispatcher.Invoke(delegate
            //{
            //    var downaloadViewModel = (DownloadViewModel)sender;
            //    AllConverting.BeginConverting(downaloadViewModel.Title, downaloadViewModel.Thumbnail,
            //        e.DownloadFileName, e.DownloadFileName, "avi");
            //});
        }
    }
}
