using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using FFMPEG.Interfaces;
using SharpLoader.Models.Downloader;
using SharpLoader.DependencyInjection;
using SharpLoader.Models.Video;
using SharpLoader.Services.Contracts;
using SharpLoader.Utils;

namespace SharpLoader.ViewModels
{
    public class DownloadViewModel : ViewModelBase
    {
        public event EventHandler<DownloadFinishedEventArgs> DownloadFinished;

        private long progress;
        private BitmapImage thumbnail;
        private string speed;
        private string title;
        private string duration;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public long Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        public BitmapImage Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                thumbnail = value;
                OnPropertyChanged(nameof(Thumbnail));
            }
        }

        public string Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        public string Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = string.Format("{0} MB/s", value);
                OnPropertyChanged(nameof(Speed));
            }
        }

        private readonly IVideoInfoService videoInfoService;
        private readonly IDownloaderService downloaderService;

        public DownloadViewModel()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            videoInfoService = DependencyResolver.Instance.Resolve<IVideoInfoService>();
            downloaderService = DependencyResolver.Instance.Resolve<IDownloaderService>();

            downloaderService.ProgressUpdated += OnDownloaderProgressUpdated;
            downloaderService.SpeedUpdated += OnSpeedUpdated;
            downloaderService.DownloadFinished += RaiseDownloadFinished;
             
        }

        private void RaiseDownloadFinished(object sender, DownloadFinishedEventArgs e)
        {
            EventUtils.RaiseEvent(this, e, ref DownloadFinished);
            //ffmpegEncoder.EncodeToAvi(e.DownloadFileName, e.DownloadFileName);
        }

        public VideoInfo Initialize(string videoUrl)
        {
            var videoInfo = videoInfoService.GetVideoInfo(videoUrl);
            Thumbnail = videoInfo.Thumbnail;
            Title = videoInfo.Title;
            Duration = TimeSpan.FromSeconds(videoInfo.DurationInSeconds).ToString();
            return videoInfo;
        }

        public void Download(VideoInfo videoInfo, string downloadLocation)
        {
            downloaderService.BeginDownload(videoInfo, downloadLocation);
        }

        void OnDownloaderProgressUpdated(object sender, ProgressUpdatedEventArgs e)
        {
            Progress = e.Progress;
        }

        private void OnSpeedUpdated(object sender, SpeedUpdatedEventArgs e)
        {
            Speed = e.Speed.ToString("0.00");
        }
    }
}
