using DownloaderWPF.Commands;
using DownloaderWPF.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VboxLoader;

namespace DownloaderWPF.ViewModels
{
    class ViewModelDownloader : ViewModelBase
    {
        public ICommand DownloadCommand { get; private set; }

        private long progress;
        private BitmapSource thumbnail;

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

        public BitmapSource Thumbnail
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

        

        public ViewModelDownloader()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            this.DownloadCommand = new RelayCommandWithParameter<string>(Download, null);
            Downloader.ProgressUpdated += OnDownloaderProgressUpdated;
        }

        private void Download(string videoUrl)
        {
            string downloadLocation = this.GetDownloadLocation();
            //if (!string.IsNullOrEmpty(downloadLocation))
            //{
                (new Task(() =>
                {
                    VideoInfo video = VideoInfo.LoadInfo(videoUrl);
                    Downloader.Download(video, "test.flv");
                })).Start();
            //}
        }

        public string GetDownloadLocation()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                return fbd.SelectedPath;
            }
            return string.Empty;
        }

        void OnDownloaderProgressUpdated(object sender, ProgressUpdatedEventArgs e)
        {
            this.Progress = e.Progress;
        }
    }
}
