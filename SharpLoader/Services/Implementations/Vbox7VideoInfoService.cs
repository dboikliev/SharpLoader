using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SharpLoader.Models.Video;
using SharpLoader.Services.Contracts;

namespace SharpLoader.Services.Implementations
{
    public class Vbox7VideoInfoService : IVideoInfoService
    {
        public VideoInfo GetVideoInfo(string videoUrl)
        {
            var videoId = ExtractVideoId(videoUrl);
            var metadataUrl = ExtractMetadataPageUrl(videoId);
            var metadata = GetMetadata(metadataUrl);
            var downloadUrl = ExtractVideoDownloadUrl(metadata);
            var thumnailUrl = ExtractThumbnailUrl(metadata);
            var thumbnail = GetVideoThumbnail(thumnailUrl);
            var size = GetVideoSizeInBytes(downloadUrl);
            var title = ExtractVideoTitle(metadata);
            

            var info = new VideoInfo
            {
                DownloadUrl = downloadUrl,
                Thumbnail = thumbnail,
                Title = title,
                FileSize = size,
                VideoUrl = videoUrl,
            };
            return info;
        }

        private string ExtractVideoTitle(string metadata)
        {
            const string titleGroup = "title";
            const string pattern = @"title=(?<{0}>.*?)&";
            var titlePattern = string.Format(pattern, titleGroup);
            var match = Regex.Match(metadata, titlePattern);
            var title = match.Groups[titleGroup].Value;
            var decodedTitle = WebUtility.UrlDecode(title);
            return decodedTitle;
        }

        private BitmapImage GetVideoThumbnail(string thumbnailUrl)
        {
            using (var webClient = new WebClient())
            {
                var imageBytes =  webClient.DownloadData(thumbnailUrl);
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

        private string ExtractThumbnailUrl(string metadata)
        {
            const string thumbnailUrlPattern = @"i\d*.vbox7.com(/\w+)*(?<image_name>\w+\.jpg)";
            var match = Regex.Match(metadata, thumbnailUrlPattern);
            var imageUrl = string.Format("http://{0}", match.Value);
            return imageUrl;
        }

        private long GetVideoSizeInBytes(string downloadUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(downloadUrl);
            var response = (HttpWebResponse)request.GetResponse();
            var contentLength = response.ContentLength;
            return contentLength;
        }

        private string ExtractVideoDownloadUrl(string metadata)
        {
            const string downloadUrlPattern = @"media\d*.vbox7.com/s/\w+/(?<filename>\w+\.\w{3})";
            var match = Regex.Match(metadata, downloadUrlPattern);
            var downloadUrl = string.Format("http://{0}", match.Value);
            return downloadUrl;
        }

        private string GetMetadata(string metadataUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(metadataUrl);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var responseContent = reader.ReadToEnd();
                        return responseContent;
                    }
                }
            }
        }

        private string ExtractMetadataPageUrl(string videoId)
        {
            const string metadataUrlFormat = "http://www.vbox7.com/etc/ext.do?key={0}&as3=1";
            var url = string.Format(metadataUrlFormat, videoId);
            return url;
        }

        private string ExtractVideoId(string videoUrl)
        {
            const string pattern = @"http://(www\.)?vbox7.com/play:(?<videoId>\w+)";
            var videoIdMatch = Regex.Match(videoUrl, pattern);
            var videoId = videoIdMatch.Groups["videoId"].Captures[0].Value;
            return videoId;
        }
    }
}
