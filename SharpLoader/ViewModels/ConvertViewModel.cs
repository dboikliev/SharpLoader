using System.Windows.Media.Imaging;
using FFMPEG.Interfaces;
using SharpLoader.DependencyInjection;

namespace SharpLoader.ViewModels
{
    public class ConvertViewModel : ViewModelBase
    {

        private readonly IFfmpegEncoder _ffmpegEncoder;



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

        public ConvertViewModel()
        {
            _ffmpegEncoder = DependencyResolver.Instance.Resolve<IFfmpegEncoder>();
        }

        public async void Convert(string fileName, string resultFileName)
        {
            _ffmpegEncoder.ProgressUpdated += OnProgressUpdated;
            _ffmpegEncoder.EncodeToAvi(fileName, resultFileName);
        }

        private void OnProgressUpdated(object sender, FFMPEG.Implementations.VideoEncodingEventArgs e)
        {
            Progress = (int)e.ProgressInPercent;
        }
    }
}
