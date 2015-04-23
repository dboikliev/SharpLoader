using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using SharpLoader.DependencyInjection;
using SharpLoader.Helpers;
using SharpLoader.Models.Video;
using SharpLoader.Services.Contracts;

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
            var urls = await GetDownloadUrls(videoId);
            var url = urls.First();
            var size = GetVideoSizeInBytes(url);
            var videoInfo = new VideoInfo
            {
                VideoUrl = videoUrl,
                FileSize = size,
                DownloadUrl = url
            };
            return videoInfo;
        }

        private async Task<IEnumerable<string>> GetDownloadUrls(string videoId)
        {
            var metadataUrl = string.Format("http://www.youtube.com/get_video_info?video_id={0}&el=detailpage", videoId);
            var html = await new WebClient().DownloadStringTaskAsync(metadataUrl);
            var urlEncodedStreamMap = HttpUtility.ParseQueryString(html)["url_encoded_fmt_stream_map"];
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
            Array.Reverse(signatureChars);
            signatureChars = SwapSigunatureCharacters(signatureChars, 41);
            signatureChars = SwapSigunatureCharacters(signatureChars, 65);
            signatureChars = SwapSigunatureCharacters(signatureChars, 11);
            Array.Reverse(signatureChars);
            var decipheredSignature = string.Concat(signatureChars);
            return decipheredSignature;
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