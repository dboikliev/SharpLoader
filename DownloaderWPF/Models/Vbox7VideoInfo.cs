namespace DownloaderWPF.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Policy;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Media.Imaging;
    
    sealed class Vbox7VideoInfo : VideoInfo
    {
        private readonly string videoInfoUrl;
        private readonly string videoInfo;
        private readonly string videoId;

        public Vbox7VideoInfo(string videoUrl)
            : base(videoUrl)
        {
            this.videoId = this.GetVideoId();
            this.videoInfoUrl = this.GetVideoInfoUrl();
            this.videoInfo = this.GetVideoInfo();

            base.DownloadUrl = this.GetVideoDownloadUrl();
            base.FileSize = base.GetVideoLengthInBytes();
            base.Segments = FileSegment.SplitLengthIntoSegmentGroups(base.FileSize);

            base.Thumbnail = GetVideoThumbnail();
        }

        private string GetVideoId()
        {
            string pattern = @"http://(www\.)?vbox7.com/play:(?<videoId>\w+)";
            Match videoIdMatch = Regex.Match(base.Url, pattern);
            string videoId = videoIdMatch.Groups["videoId"].Captures[0].Value;
            return videoId;
        }

        private string GetVideoInfoUrl()
        {
            string url = string.Format("http://www.vbox7.com/etc/ext.do?key={0}", videoId);
            return url;
        }

        private string GetVideoInfo()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.videoInfoUrl);
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.3) Gecko/20100406 Firefox/3.5.3 (.NET CLR 4.0)";
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

        protected override string GetVideoDownloadUrl()
        {
            string downloadUrlPattern = @"media\d+.vbox7.com/s/\w+/(?<filename>\w+\.\w{3})";
            Match match = Regex.Match(this.videoInfo, downloadUrlPattern);
            string downloadUrl = string.Format("http://{0}", match.Value);
            return downloadUrl;
        }

        private string GetThumbnailDownloadUrl()
        {
            string thumbnailUrlPattern = @"i\d+.vbox7.com(/\w+)*(?<image_name>\w+\.jpg)";
            Match match = Regex.Match(this.videoInfo, thumbnailUrlPattern);
            string imageUrl = string.Format("http://{0}", match.Value);
            return imageUrl;
        }

        protected override BitmapSource GetVideoThumbnail()
        {
            string thumbnailUrl = this.GetThumbnailDownloadUrl();
            WebClient wc = new WebClient();
            byte[] bytes = wc.DownloadData(thumbnailUrl);
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            Bitmap bitmap = new Bitmap(ms);
            //WebClient wc = new WebClient();
            //byte[] imageBytes = wc.DownloadData(thumbnailUrl);
            //BitmapImage image = new BitmapImage();
            //using (MemoryStream ms = new MemoryStream(imageBytes))
            //{
            //    image.BeginInit();
            //    image.StreamSource = ms;
            //    image.EndInit();
            //}

            var bitmapSource =
               System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                 bitmap.GetHbitmap(Color.Black),
                 IntPtr.Zero,
                 new System.Windows.Int32Rect(0, 0, bitmap.Width, bitmap.Height),
                 null);
            return bitmapSource;
        }
    }
}
