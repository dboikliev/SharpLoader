using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Management;
using System.Windows.Media.Imaging;
using SharpLoader.DependencyInjection;
using SharpLoader.Helpers;
using SharpLoader.Models.Video;
using SharpLoader.Services.Contracts;
using System.IO;

namespace SharpLoader.Services.Implementations
{
    public class YouTubeVideoInfoService : IVideoInfoService
    {
        private readonly IHtmlDownloader htmlDownloader;

        public YouTubeVideoInfoService()
        {
            htmlDownloader = DependencyResolver.Instance.Resolve<IHtmlDownloader>();
        }

        public async Task<VideoInfo> GetVideoInfo(string videoUrl)
        {
            var videoId = ExtractVideoId(videoUrl);
            var metadata = await GetVideoMetadata(videoId);
            var urls = ExtractDownloadUrls(metadata);
            var duration = ExtractDurationInSeconds(metadata);
            var thumbnailUrl = ExtractThumbnailUrl(metadata);
            var thumbnail = GetThumbnail(thumbnailUrl);
            var title = ExtractVideoTitle(metadata);

            var url = urls.First();
            var size = GetVideoSizeInBytes(url);
            var videoInfo = new VideoInfo
            {
                VideoUrl = videoUrl,
                FileSize = size,
                DownloadUrl = url,
                DurationInSeconds = duration,
                Thumbnail = thumbnail,
                Title = title
            };
            return videoInfo;
        }

        private string ExtractVideoTitle(string metadata)
        {
            var title = HttpUtility.ParseQueryString(metadata)["title"];
            return title;
        }

        private string ExtractThumbnailUrl(string metadata)
        {
            var thumbnailUrl = HttpUtility.ParseQueryString(metadata)["thumbnail_url"];
            return thumbnailUrl;
        }

        private BitmapImage GetThumbnail(string thumbnailUrl)
        {
            using (var webClient = new WebClient())
            {
                var data = webClient.DownloadData(thumbnailUrl);
                var bitmap = new BitmapImage();
                using (var dataStream = new MemoryStream(data))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = dataStream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                bitmap.Freeze();
                return bitmap;
            }

        }

        private int ExtractDurationInSeconds(string metadata)
        {
            var duration = int.Parse(HttpUtility.ParseQueryString(metadata)["length_seconds"]);
            return duration;
        }

        private async Task<string> GetVideoMetadata(string videoId)
        {
            var metadataUrl = string.Format("http://www.youtube.com/get_video_info?video_id={0}&el=detailpage", videoId);
            using (var webClient = new WebClient())
            {
                var metadata = await webClient.DownloadStringTaskAsync(metadataUrl);
                return metadata;
            }
        }

        private IEnumerable<string> ExtractDownloadUrls(string metadata)
        {
            var urlEncodedStreamMap = HttpUtility.ParseQueryString(metadata)["url_encoded_fmt_stream_map"];
            var urls = ParseUrlEncodedStreamMap(urlEncodedStreamMap);
            return urls;
        }

        private IEnumerable<string> ParseUrlEncodedStreamMap(string urlEncodedStreamMap)
        {
            var urlsMetadata = GetUrlsMetadata(urlEncodedStreamMap);
            var parsedUrls = urlsMetadata.Select(ParseUrlMetadata);
            return parsedUrls;
        }

        private IEnumerable<string> GetUrlsMetadata(string urlEncodedStreamMap)
        {
            var urlsMetadata = urlEncodedStreamMap.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return urlsMetadata;
        }

        private string ParseUrlMetadata(string urlMetadata)
        {
            var queryParams = HttpUtility.ParseQueryString(urlMetadata);
            var downloadUrlParams = HttpUtility.ParseQueryString(queryParams["url"]);

            if (!string.IsNullOrEmpty(queryParams["sig"]) || !string.IsNullOrEmpty(queryParams["s"]))
                downloadUrlParams["signature"] = queryParams["sig"] ?? DecipherSignature(queryParams["s"]);
            
            var decodedUrl = HttpUtility.UrlDecode(HttpUtility.UrlDecode(downloadUrlParams.ToString()));
            return decodedUrl;
        }

        private string DecipherSignature(string signature)
        {
            var signatureChars = signature.ToCharArray();
            signatureChars = SwapSigunatureCharacters(signatureChars, 13);
            signatureChars = Slice(signatureChars, 1);
            signatureChars = SwapSigunatureCharacters(signatureChars, 4);
            signatureChars = Slice(signatureChars, 2);
            Array.Reverse(signatureChars);
            signatureChars = Slice(signatureChars, 2);
            signatureChars = SwapSigunatureCharacters(signatureChars, 25);
            var decipheredSignature = string.Concat(signatureChars);
            return decipheredSignature;
        }

        private char[] Slice(char[] characters, int position)
        {
            return characters.Skip(position).Take(characters.Length).ToArray();
        }

        private char[] SwapSigunatureCharacters(char[] signatureCharacters, int position)
        {
            var charToMove = signatureCharacters[0];
            var newValue = signatureCharacters[position % signatureCharacters.Length];
            signatureCharacters[0] = newValue;
            signatureCharacters[position] = charToMove;
            return signatureCharacters;
        }

        private long GetVideoSizeInBytes(string downloadUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(downloadUrl);
            var response = (HttpWebResponse)request.GetResponse();
            var contentLength = response.ContentLength;
            return contentLength;
        }

        private string ExtractVideoId(string videoUrl)
        {
            const string pattern = @"https?://(www\.)?youtube.com/watch\?v=(?<videoId>[\w\-]+)";
            var videoIdMatch = Regex.Match(videoUrl, pattern);
            var videoId = videoIdMatch.Groups["videoId"].Captures[0].Value;
            return videoId;
        }
    }
}