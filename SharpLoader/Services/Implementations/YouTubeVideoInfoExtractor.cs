using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Media.Imaging;
using SharpLoader.DependencyInjection;
using SharpLoader.Helpers;
using SharpLoader.Models.Video;
using SharpLoader.Services.Contracts;
using System.IO;
using SharpLoader.Constants;

namespace SharpLoader.Services.Implementations
{
    public class YouTubeVideoInfoService : IVideoInfoService
    {
        private readonly IHtmlDownloader _htmlDownloader;

        public YouTubeVideoInfoService()
        {
            _htmlDownloader = DependencyResolver.Instance.Resolve<IHtmlDownloader>();
        }

        public VideoInfo GetVideoInfo(string videoUrl)
        {
            var videoId = ExtractVideoId(videoUrl);
            var metadata = GetVideoMetadata(videoId);
            var urls = ExtractDownloadUrls(metadata);
            var duration = ExtractDurationInSeconds(metadata);
            var thumbnailUrl = ExtractThumbnailUrl(metadata);
            var thumbnail = GetThumbnail(thumbnailUrl);
            var title = ExtractVideoTitle(metadata);
            var fileExtension = string.Empty;

            var url = urls.Where(u =>
            {
                try
                {
                    var response = (HttpWebResponse)WebRequest.CreateHttp(u).GetResponse();
                    fileExtension = MimeTypes.Instance[response.ContentType];
                    return response.StatusCode != HttpStatusCode.Forbidden;
                }
                catch (Exception)
                {
                    return false;
                }
            }).First();

            var size = GetVideoSizeInBytes(url);
            var videoInfo = new VideoInfo
            {
                VideoUrl = videoUrl,
                FileSize = size,
                DownloadUrl = url,
                DurationInSeconds = duration,
                Thumbnail = thumbnail,
                Title = title,
                FileExtension = fileExtension
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

        private string GetVideoMetadata(string videoId)
        {
            using (var webClient = new WebClient())
            {

                var metadataUrl = $"http://www.youtube.com/get_video_info?video_id={videoId}&el=detailpage";

                //webClient.Headers["Cookie"] =
                //    "GEUP=e518689d4c480e82d730e6c78c768a7faQwAAAA=; _ga=GA1.2.171862194.1434138653; gmOGQ.resume=FMoZKfx1dcw:346,GBIZfnaVc5M:1333,9D1M7QeteKU:266,55uTkKr7Cxc:1171; VISITOR_INFO1_LIVE=pOCPy7RkY6s; s_gl=1d69aac621b2f9c0a25dade722d6e24bcwIAAABVUw==; YSC=GuEy-_U0sWQ; SID=DQAAABEBAACuGaalsZyc6ufDVHFAl9Dp9guxEjpJWSLdHh0jZv5PNniuZowvfjYni68EuBtEtAG7yAs0rCcCBkZ7gE6t7ppnNWmjrPi_RayofDNqv265xjJAxQYVxoPQ4Xzt10AEQS7ER9tytIfVuVG5qwMJAqrzZK27ygTDhBkEnehCcoxyHYfv0BaTVXXyL3gv-xyIV8oEN-MOQY7OQsZRKRsFjdXPqJ_OgCbWE0gYhPQT9iLBbq1-5NkXvX5NqU8f4OrAOmh06ilFEGYShFgQTtbEagJ-xk4Kpx77aNT8oB2LJE9s2842ugDpTbTFBBiraZBVOrGw8aZr2jcr1D_lAAuhSpb9t2CMQ-gXfu6tMerdMMhbASJ-Y91_QWwynbT1Ph_gK-M; HSID=AtBh8s0AdWhjBQjpW; APISID=Qq1D-Fv_sPFbsh_Z/A3sbOQdS0sEtLzKTl; CONSENT=YES+BG.en-GB+20150628-20-0; LOGIN_INFO=6401b5bc311185a417d6b9b81726f270c1oAAAB7IjEwIjogNDYzODkzMTQ2MDM0LCAiOCI6IDkxMjI0MzIzMjkyNSwgIjciOiAwLCAiNCI6ICJERUxFR0FURUQiLCAiMyI6IDEwMzY5Mjg2MjIsICIxIjogMX0=; PREF=f5=20030030&al=bg&f1=50000000&f4=20000";
                var metadata = webClient.DownloadString(metadataUrl);
                if (metadata.Contains("fail"))
                {
                    metadataUrl = $"http://www.youtube.com/get_video_info?video_id={videoId}&eurl={Uri.EscapeDataString("https://youtube.googleapis.com/v/" + videoId)}&sts=16682";
                    metadata = webClient.DownloadString(metadataUrl);
                }
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
            signatureChars = SwapSigunatureCharacters(signatureChars, 7);
            Array.Reverse(signatureChars);
            signatureChars = SwapSigunatureCharacters(signatureChars, 13);
            signatureChars = SwapSigunatureCharacters(signatureChars, 69);
            signatureChars = Slice(signatureChars, 3);
            Array.Reverse(signatureChars);
            signatureChars = SwapSigunatureCharacters(signatureChars, 14);
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
            signatureCharacters[0] = signatureCharacters[position % signatureCharacters.Length];
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