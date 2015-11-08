using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using FFMPEG.Interfaces;
using SharpLoader.DependencyInjection;

namespace SharpLoader.ViewModels
{
    public class ConvertViewModel : ViewModelBase
    {

        private readonly IFfmpegEncoder ffmpegEncoder;



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

        public ConvertViewModel()
        {
            ffmpegEncoder = DependencyResolver.Instance.Resolve<IFfmpegEncoder>();
        }

        public async void Convert(string fileName, string resultFileName)
        {
            ffmpegEncoder.ProgressUpdated += OnProgressUpdated;
            ffmpegEncoder.EncodeToAvi(fileName, resultFileName);
        }

        private void OnProgressUpdated(object sender, FFMPEG.Implementations.VideoEncodingEventArgs e)
        {
            Progress = (int)e.ProgressInPercent;
        }
    }
}
