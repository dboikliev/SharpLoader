using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SharpLoader.Commands;
using SharpLoader.Models;
using SharpLoader.Models.VideoInfo;

namespace SharpLoader.ViewModels
{
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
                return progress;
            }
            set
            {
                progress = value;
                OnPropertyChanged("Progress");
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
                OnPropertyChanged("Thumbnail");
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
                OnPropertyChanged("Speed");
            }
        }


        public ViewModelDownloader()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            DownloadCommand = new RelayCommandWithParameter<string>(Download, CanDownload);
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
            var downloadLocation = GetDownloadLocation();
            if (!string.IsNullOrEmpty(downloadLocation))
            {
                if (task != null && task.Status == TaskStatus.Running)
                {
                    cancellationSource.Cancel();
                }
                cancellationSource = new CancellationTokenSource();
                var token = cancellationSource.Token;
                var downloader = new Downloader();
                downloader.ProgressUpdated += OnDownloaderProgressUpdated;
                downloader.SpeedUpdated += OnSpeedUpdated;
                task = new Task(() =>
                                   {
                                       var video = VideoInfoBase.LoadInfo(videoUrl);
                                       Thumbnail = video.Thumbnail;
                                       downloader.DownloadFile(video, downloadLocation, token);
                                   }, token);
                task.Start();
            }
        }

        public string GetDownloadLocation()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Video File |*.flv";
            dialog.AddExtension = true;
            dialog.DefaultExt = ".flv";
            dialog.ShowDialog();
            return dialog.FileName;
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
