using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Newtonsoft.Json;
using SharpLoader.Commands;
using SharpLoader.DependencyInjection;
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
        public ObservableCollection<DownloadViewModel> Downloads { get; private set; }

        public event EventHandler<DownloadFinishedEventArgs> DownloadFinished;

        public ICommand BeginDownload { get; private set; }

        private readonly IDialogService dialogService;
        private readonly IUrlService urlService;
        private readonly INotificationService notificationService;

        public AllDownloadsViewModel()
        {
            Downloads = new ObservableCollection<DownloadViewModel>();

            BeginDownload = new CommandWithParameter<string>(InitializeDownload, CanInitializeDownload);

            dialogService = DependencyResolver.Instance.Resolve<IDialogService>();
            urlService = DependencyResolver.Instance.Resolve<IUrlService>();
            notificationService = DependencyResolver.Instance.Resolve<INotificationService>();
        }

        private void InitializeDownload(string url)
        {
            if (IsPlayList(url))
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (sender, args) =>
                {

                    var videoUrl = args.Argument.ToString();
                    DownloadPlaylist(videoUrl);

                };
                worker.RunWorkerAsync(url);
                return;
            }
            DownloadSingleVideo(url);
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
            var downloadLocation = dialogService.ShowSaveFileDialog(videoInfo.FileName, videoInfo.FileExtension,
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
            using (var client = new WebClient())
            {
                Playlist playlist;
                var nextPageToken = string.Empty;
                do
                {
                    var listId = Regex.Match(playlistUrl, @"\w+&list=(?<listId>.+)[&.]*").Groups["listId"].Value;
                    var url =
                        $"https://content.googleapis.com/youtube/v3/playlistItems?part=contentDetails&maxResults=50&pageToken={nextPageToken}&playlistId={listId}&key=AIzaSyCFj15TpkchL4OUhLD1Q2zgxQnMb7v3XaM";
                    client.Headers["X-Origin"] = "https://developers.google.com";
                    var json = client.DownloadString(url);
                    playlist = JsonConvert.DeserializeObject<Playlist>(json);
                    nextPageToken = playlist.NextPageToken;
                    var directory = dialogService.ShowChooseDirectoryDialog();
                    Parallel.ForEach(playlist.Items, (item) =>
                    {
                        var videoUrl = $"https://www.youtube.com/watch?v={item.ContentDetails.VideoId}";
                        if (!ValidateUrl(videoUrl))
                        {
                            return;
                        }


                        var downloader1 = new DownloadViewModel();
                        downloader1.DownloadFinished += RaiseDownloadFinished;
                        var videoInfo1 = downloader1.Initialize(videoUrl);

                        bool? result1 = false;

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Downloads.Add(downloader1);
                        });
                        downloader1.Download(videoInfo1, Path.Combine(directory, videoInfo1.FileName));
                    });
                } while (!string.IsNullOrEmpty(playlist.NextPageToken));
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
            if (!urlService.IsValidUrl(videoUrl))
            {
                notificationService.ShowErrorNotification(string.Format("{0} is not a valid url.", videoUrl));
                return false;
            }
            return true;
        }

        private bool ValidateDownloadLocation(string downloadLocation)
        {
            if (string.IsNullOrEmpty(downloadLocation))
            {
                notificationService.ShowErrorNotification("Invalid download location.");
                return false;
            }
            return true;
        }
    }
}
