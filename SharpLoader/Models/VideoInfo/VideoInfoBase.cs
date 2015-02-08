using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using VboxLoader.Models.Exceptions;

namespace SharpLoader.Models.VideoInfo
{
    public abstract class VideoInfoBase
    {
        public string DownloadUrl { get; protected set; }
        public string VideoUrl { get; protected set; }
        public long VideoSize { get; protected set; }
        public BitmapImage Thumbnail { get; protected set; }
        public string Title { get; protected set; }

        protected abstract string GetVideoDownloadUrl();

        protected BitmapImage GetVideoThumbnail(string thumbnailUrl)
        {
            using (var wc = new WebClient())
            {
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

        protected VideoInfoBase(string videoUrl)
        {
            VideoUrl = videoUrl;
        }

        public static VideoInfoBase LoadInfo(string videoUrl)
        {
            var videoInfoBase = CreateVideoInfo(videoUrl);
            return videoInfoBase;
        }

        private static VideoInfoBase CreateVideoInfo(string videoUrl)
        {
            var domain = GetUrlDomain(videoUrl);
            switch (domain)
            {
                case "vbox7.com":
                    return new Vbox7VideoInfo(videoUrl);
                default:
                    throw new DomainNotSupportedException(domain);
            }
        }

        private static string GetUrlDomain(string videoUrl)
        {
            const string pattern = @"http://(www\.)?(?<domain>\w+\.\w{2,4})(/[\w;amp]*)*/?";
            var regex = new Regex(pattern);
            var match = regex.Match(videoUrl);
            var group = match.Groups["domain"];
            if (group.Success)
            {
                string domain = group.Captures[0].Value;
                return domain;
            }
            throw new InvalidUrlException();
        }


        protected static long GetVideoLengthInBytes(string downloadUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(downloadUrl);
            var response = (HttpWebResponse)request.GetResponse();
            var contentLength = response.ContentLength;
            return contentLength;
        }
    }
}
