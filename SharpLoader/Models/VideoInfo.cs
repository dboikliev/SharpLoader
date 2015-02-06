using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using VboxLoader.Models.Exceptions;
using System.Windows.Media.Imaging;

namespace SharpLoader.Models
{
    abstract class VideoInfo
    {
        //private static readonly long bytesPerKilobyte = 1024;
        //private static readonly long bytesPerMegabyte = 1024 * bytesPerKilobyte;
        //private static readonly int segmentCount = 10;
        //private static readonly long bytesPerSegment = bytesPerMegabyte;

        public IEnumerable<FileSegment[]> Segments { get; protected set; }
        public string DownloadUrl { get; protected set; }
        public string Url { get; protected set; }
        public long FileSize { get; protected set; }
        public BitmapImage Thumbnail { get; protected set; }


        protected abstract string GetVideoDownloadUrl();
        protected abstract BitmapImage GetVideoThumbnail();

        protected VideoInfo(string videoUrl)
        {
            this.Url = videoUrl;
        }

        public static VideoInfo LoadInfo(string videoUrl)
        {
            VideoInfo videoInfo = CreateVideoInfo(videoUrl);
            return videoInfo;
        }

        private static VideoInfo CreateVideoInfo(string videoUrl)
        {
            VideoInfo videoInfo = null;
            string domain = GetUrlDomain(videoUrl);
            switch (domain)
            {
                case "vbox7.com":
                    videoInfo = new Vbox7VideoInfo(videoUrl);
                    break;
                default:
                    throw new DomainNotSupportedException(domain);
            }
            return videoInfo;
        }

        private static string GetUrlDomain(string videoUrl)
        {
            string pattern = @"http://(www\.)?(?<domain>\w+\.\w{2,4})(/[\w;amp]*)*/?";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(videoUrl);
            Group group = match.Groups["domain"];
            string domain = string.Empty;
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(downloadUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            long contentLength = response.ContentLength;
            return contentLength;
        }
    }
}
