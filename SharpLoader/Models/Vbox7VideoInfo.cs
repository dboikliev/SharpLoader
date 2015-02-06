namespace SharpLoader.Models
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows.Media.Imaging;
    
    /// <summary>
    /// Holds the information about a video from Vbox7.
    /// </summary>
    sealed class Vbox7VideoInfo : VideoInfo
    {
        private readonly string videoInfoUrl;
        private readonly string videoInfo;
        private readonly string videoId;

        public Vbox7VideoInfo(string videoUrl)
            : base(videoUrl)
        {
            this.videoId = GetVideoId(videoUrl);
            this.videoInfoUrl = GetVideoInfoUrl(this.videoId);
            this.videoInfo = GetVideoInfo(this.videoInfoUrl);

            base.DownloadUrl = this.GetVideoDownloadUrl();
            base.FileSize = GetVideoLengthInBytes(base.DownloadUrl);
            base.Segments = FileSegment.SplitLengthIntoSegmentGroups(base.FileSize);
            base.Thumbnail = this.GetVideoThumbnail();
        }

        private static string GetVideoId(string videoUrl)
        {
            string pattern = @"http://(www\.)?vbox7.com/play:(?<videoId>\w+)";
            Match videoIdMatch = Regex.Match(videoUrl, pattern);
            string videoId = videoIdMatch.Groups["videoId"].Captures[0].Value;
            return videoId;
        }

        private static string GetVideoInfoUrl(string videoId)
        {
            string url = string.Format("http://www.vbox7.com/etc/ext.do?key={0}", videoId);
            return url;
        }


        /// <summary>
        /// Downloads the information containing the links for the video file, the thumbnails etc.
        /// </summary>
        /// <param name="videoInfoUrl">The url where the video information is located.</param>
        /// <returns>The text containing information about the video.</returns>
        private static string GetVideoInfo(string videoInfoUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(videoInfoUrl);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            string responseContent = string.Empty;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseContent = reader.ReadToEnd();
                }
            }
            response.Close();
            return responseContent;
        }

        private static string GetThumbnailDownloadUrl(string videoInfo)
        {
            string thumbnailUrlPattern = @"i\d+.vbox7.com(/\w+)*(?<image_name>\w+\.jpg)";
            Match match = Regex.Match(videoInfo, thumbnailUrlPattern);
            string imageUrl = string.Format("http://{0}", match.Value);
            return imageUrl;
        }
        
        protected override string GetVideoDownloadUrl()
        {
            string downloadUrlPattern = @"media\d+.vbox7.com/s/\w+/(?<filename>\w+\.\w{3})";
            Match match = Regex.Match(this.videoInfo, downloadUrlPattern);
            string downloadUrl = string.Format("http://{0}", match.Value);
            return downloadUrl;
        }

        protected override BitmapImage GetVideoThumbnail()
        {
            string thumbnailUrl = GetThumbnailDownloadUrl(this.videoInfo);
            WebClient wc = new WebClient();
            byte[] imageBytes = wc.DownloadData(thumbnailUrl);
            BitmapImage image = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(imageBytes))
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
