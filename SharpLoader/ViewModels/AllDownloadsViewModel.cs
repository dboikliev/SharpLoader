using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using SharpLoader.Commands;
using SharpLoader.Models.Downloader;
using SharpLoader.Services.Contracts;
using SharpLoader.Utils;

namespace SharpLoader.ViewModels
{
    class Playlist
    {
        [JsonProperty("items")]
        public Item[] Items { get; set; }
        [JsonProperty("nextPageToken")]
        public string NextPageToken { get; set; }
    }

    class Item
    {
        [JsonProperty("contentDetails")]
        public ContentDetails ContentDetails { get; set; }
    }

    class ContentDetails
    {
        [JsonProperty("videoId")]
        public string VideoId { get; set; }
    }


    public class AllDownloadsViewModel : ViewModelBase
    {
        public ObservableCollection<DownloadViewModel> Downloads { get; }

        public event EventHandler<DownloadFinishedEventArgs> DownloadFinished;

        public ICommand BeginDownload { get; private set; }

        private readonly IDialogService _dialogService;
        private readonly IUrlService _urlService;
        private readonly INotificationService _notificationService;
        private readonly IYoutubePlaylistLinksService _playlistLinksService;

        public AllDownloadsViewModel(IDialogService dialogService,
            IUrlService urlService,
            INotificationService notificationService,
            IYoutubePlaylistLinksService playlistLinksService)
        {
            _dialogService = dialogService;
            _urlService = urlService;
            _notificationService = notificationService;
            _playlistLinksService = playlistLinksService;
            Downloads = new ObservableCollection<DownloadViewModel>();

            BeginDownload = new CommandWithParameter<string>(InitializeDownload, CanInitializeDownload);

            //_dialogService = DependencyResolver.Instance.Resolve<IDialogService>();
            //_urlService = DependencyResolver.Instance.Resolve<IUrlService>();
            //_notificationService = DependencyResolver.Instance.Resolve<INotificationService>();
        }

        private void InitializeDownload(string url)
        {
            if (IsPlayList(url))
            {
                DownloadPlaylist(url);
            }
            else
            {
                DownloadSingleVideo(url);
            }
        }


        private void DownloadSingleVideo(string videoUrl)
        {
            if (!ValidateUrl(videoUrl))
            {
                return;
            }


            var downloader = new DownloadViewModel();
            downloader.DownloadFinished += RaiseDownloadFinished;
            var videoInfo = downloader.Initialize(videoUrl);

            bool? result = false;
            var downloadLocation = _dialogService.ShowSaveFileDialog(videoInfo.FileName, videoInfo.FileExtension,
                out result);
            if (!result.GetValueOrDefault() || !ValidateDownloadLocation(downloadLocation))
            {
                return;
            }

            Downloads.Add(downloader);
            downloader.Download(videoInfo, downloadLocation);
        }

        private void DownloadPlaylist(string playlistUrl)
        {
            var playlistUrls = _playlistLinksService.ExtractPlaylistLinks(playlistUrl);
            var downloadLocation = _dialogService.ShowChooseDirectoryDialog();
            foreach (var url in playlistUrls)
            {
                Task.Run(() =>
                {
                    var downloader = new DownloadViewModel();
                    Application.Current.Dispatcher.Invoke(() => Downloads.Add(downloader));
                    var videoInfo = downloader.Initialize(url);
                    downloader.Download(videoInfo, downloadLocation + "\\" + videoInfo.FileName);
                });
            }
        }

        private bool IsPlayList(string videoUrl)
        {
            return videoUrl.Contains("list");
        }

        private void RaiseDownloadFinished(object sender, DownloadFinishedEventArgs e)
        {
            EventUtils.RaiseEvent(sender, e, ref DownloadFinished);
        }

        private bool CanInitializeDownload(string videoUrl)
        {
            return true;
        }

        private bool ValidateUrl(string videoUrl)
        {
            if (!_urlService.IsValidUrl(videoUrl))
            {
                _notificationService.ShowErrorNotification(string.Format("{0} is not a valid url.", videoUrl));
                return false;
            }
            return true;
        }

        private bool ValidateDownloadLocation(string downloadLocation)
        {
            if (string.IsNullOrEmpty(downloadLocation))
            {
                _notificationService.ShowErrorNotification("Invalid download location.");
                return false;
            }
            return true;
        }
    }
}
