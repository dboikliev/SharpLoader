using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace SharpLoader.Models.Video
{
    public class VideoInfo
    {
        public string DownloadUrl { get; set; }
        public string VideoUrl { get; set; }
        public long FileSize { get; set; }

        public string FileName => Regex.Replace(Title, "[<>:\"\\/|?*]", string.Empty) + FileExtension;

        public string FileExtension { get; set; }
        public BitmapImage Thumbnail { get; set; }
        public string Title { get; set; }
        public int DurationInSeconds { get; set; }

    }
}
