using System;
using System.Windows.Media.Imaging;
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

        private long _progress;
        private BitmapImage _thumbnail;
        private string _speed;
        private string _title;
        private string _duration;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public long Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        public BitmapImage Thumbnail
        {
            get
            {
                return _thumbnail;
            }
            set
            {
                _thumbnail = value;
                OnPropertyChanged(nameof(Thumbnail));
            }
        }

        public string Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        public string Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = string.Format("{0} MB/s", value);
                OnPropertyChanged(nameof(Speed));
            }
        }

        private readonly IVideoInfoService _videoInfoService;
        private readonly IDownloaderService _downloaderService;

        public DownloadViewModel()
        {
            _videoInfoService = DependencyResolver.Instance.Resolve<IVideoInfoService>();
            _downloaderService = DependencyResolver.Instance.Resolve<IDownloaderService>();

            _downloaderService.ProgressUpdated += OnDownloaderProgressUpdated;
            _downloaderService.SpeedUpdated += OnSpeedUpdated;
            _downloaderService.DownloadFinished += RaiseDownloadFinished;
             
        }

        private void RaiseDownloadFinished(object sender, DownloadFinishedEventArgs e)
        {
            EventUtils.RaiseEvent(this, e, ref DownloadFinished);
        }

        public VideoInfo Initialize(string videoUrl)
        {
            var videoInfo = _videoInfoService.GetVideoInfo(videoUrl);
            Thumbnail = videoInfo.Thumbnail;
            Title = videoInfo.Title;
            Duration = TimeSpan.FromSeconds(videoInfo.DurationInSeconds).ToString();
            return videoInfo;
        }

        public void Download(VideoInfo videoInfo, string downloadLocation)
        {
            _downloaderService.BeginDownload(videoInfo, downloadLocation);
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
