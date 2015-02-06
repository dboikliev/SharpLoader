using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using VboxLoader.Models.Exceptions;

namespace SharpLoader.Models.VideoInfo
{
    abstract class VideoInfoBase
    {
        public IEnumerable<VideoFileSegment[]> Segments { get; protected set; }
        public string DownloadUrl { get; protected set; }
        public string Url { get; protected set; }
        public long VideoSize { get; protected set; }
        public BitmapImage Thumbnail { get; protected set; }


        protected abstract string GetVideoDownloadUrl();
        protected abstract BitmapImage GetVideoThumbnail();

        protected VideoInfoBase(string videoUrl)
        {
            Url = videoUrl;
        }

        public static VideoInfoBase LoadInfo(string videoUrl)
        {
            var videoInfoBase = CreateVideoInfo(videoUrl);
            return videoInfoBase;
        }

        private static VideoInfoBase CreateVideoInfo(string videoUrl)
        {
            VideoInfoBase videoInfoBase = null;
            var domain = GetUrlDomain(videoUrl);
            switch (domain)
            {
                case "vbox7.com":
                    videoInfoBase = new Vbox7VideoInfo(videoUrl);
                    break;
                default:
                    throw new DomainNotSupportedException(domain);
            }
            return videoInfoBase;
        }

        private static string GetUrlDomain(string videoUrl)
        {
            var pattern = @"http://(www\.)?(?<domain>\w+\.\w{2,4})(/[\w;amp]*)*/?";
            var regex = new Regex(pattern);
            var match = regex.Match(videoUrl);
            var group = match.Groups["domain"];
            var domain = string.Empty;
            if (group.Success)
            {
                domain = group.Captures[0].Value;
            }
            else
            {
                throw new InvalidUrlException();
            }
            return domain;
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
