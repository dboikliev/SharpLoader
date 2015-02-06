using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace SharpLoader.Models.VideoInfo
{
    /// <summary>
    /// Holds the information about a video from Vbox7.
    /// </summary>
    sealed class Vbox7VideoInfo : VideoInfoBase
    {
        private readonly string videoInfoUrl;
        private readonly string videoInfo;
        private readonly string videoId;

        public Vbox7VideoInfo(string videoUrl)
            : base(videoUrl)
        {
            videoId = GetVideoId(videoUrl);
            videoInfoUrl = GetVideoInfoUrl(videoId);
            videoInfo = GetVideoInfo(videoInfoUrl);

            DownloadUrl = GetVideoDownloadUrl();
            VideoSize = GetVideoLengthInBytes(DownloadUrl);
            Segments = VideoFileSegment.SplitLengthIntoSegmentGroups(VideoSize);
            Thumbnail = GetVideoThumbnail();
        }

        private static string GetVideoId(string videoUrl)
        {
            const string pattern = @"http://(www\.)?vbox7.com/play:(?<videoId>\w+)";
            var videoIdMatch = Regex.Match(videoUrl, pattern);
            var videoId = videoIdMatch.Groups["videoId"].Captures[0].Value;
            return videoId;
        }

        private static string GetVideoInfoUrl(string videoId)
        {
            var url = string.Format("http://www.vbox7.com/etc/ext.do?key={0}", videoId);
            return url;
        }

        /// <summary>
        /// Downloads the information containing the links for the video file, the thumbnails etc.
        /// </summary>
        /// <param name="videoInfoUrl">The url where the video information is located.</param>
        /// <returns>The text containing information about the video.</returns>
        private static string GetVideoInfo(string videoInfoUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(videoInfoUrl);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            var responseContent = string.Empty;
            var response = (HttpWebResponse)request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    responseContent = reader.ReadToEnd();
                }
            }
            response.Close();
            return responseContent;
        }

        private static string GetThumbnailDownloadUrl(string videoInfo)
        {
            var thumbnailUrlPattern = @"i\d+.vbox7.com(/\w+)*(?<image_name>\w+\.jpg)";
            var match = Regex.Match(videoInfo, thumbnailUrlPattern);
            var imageUrl = string.Format("http://{0}", match.Value);
            return imageUrl;
        }
        
        protected override string GetVideoDownloadUrl()
        {
            var downloadUrlPattern = @"media\d+.vbox7.com/s/\w+/(?<filename>\w+\.\w{3})";
            var match = Regex.Match(videoInfo, downloadUrlPattern);
            var downloadUrl = string.Format("http://{0}", match.Value);
            return downloadUrl;
        }

        protected override BitmapImage GetVideoThumbnail()
        {
            var thumbnailUrl = GetThumbnailDownloadUrl(videoInfo);
            var wc = new WebClient();
            var imageBytes = wc.DownloadData(thumbnailUrl);
            var image = new BitmapImage();
            using (var ms = new MemoryStream(imageBytes))
            {
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
