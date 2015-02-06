using SharpLoader.Commands;
using SharpLoader.Models;

namespace SharpLoader.ViewModels
{
    using SharpLoader.Commands;
    using SharpLoader.Models;
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    class ViewModelDownloader : ViewModelBase
    {
        public ICommand DownloadCommand { get; private set; }
        
        private Task task;
        private CancellationTokenSource cancellationSource;
        
        private long progress;
        private BitmapImage thumbnail;
        private string speed;

        public long Progress
        {
            get
            {
                return this.progress;
            }
            set
            {
                this.progress = value;
                base.OnPropertyChanged("Progress");
            }
        }

        public BitmapImage Thumbnail
        {
            get
            {
                return this.thumbnail;
            }
            set
            {
                this.thumbnail = value;
                base.OnPropertyChanged("Thumbnail");
            }
        }

        public string Speed
        {
            get
            {
                return this.speed;
            }
            set
            {
                this.speed = string.Format("{0} MB/s", value);
                base.OnPropertyChanged("Speed");
            }
        }


        public ViewModelDownloader()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            this.DownloadCommand = new RelayCommandWithParameter<string>(Download, CanDownload);
            Downloader.ProgressUpdated += OnDownloaderProgressUpdated;
            Downloader.SpeedUpdated += OnSpeedUpdated;
        }

        private bool CanDownload(string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return false;
            }
            return true;
        }

        private void Download(string videoUrl)
        {   
            string downloadLocation = GetDownloadLocation();
            if (!string.IsNullOrEmpty(downloadLocation))
            {
                if (task != null && task.Status == TaskStatus.Running)
                {
                    cancellationSource.Cancel();
                }
                cancellationSource = new CancellationTokenSource();
                CancellationToken token = cancellationSource.Token;
                task = new Task(() =>
                                   {
                                       VideoInfo video = VideoInfo.LoadInfo(videoUrl);
                                       this.Thumbnail = video.Thumbnail;
                                       Downloader.Download(video, downloadLocation, token);
                                   }, token);
                task.Start();
            }
        }

        public string GetDownloadLocation()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Video File |*.flv";
            dialog.AddExtension = true;
            dialog.DefaultExt = ".flv";
            dialog.ShowDialog();
            return dialog.FileName;
        }

        void OnDownloaderProgressUpdated(object sender, ProgressUpdatedEventArgs e)
        {
            this.Progress = e.Progress;
        }

        private void OnSpeedUpdated(object sender, SpeedUpdatedEventArgs e)
        {
            this.Speed = e.Speed.ToString("0.00");
        }
    }
}
