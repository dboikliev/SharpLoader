using System;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using SharpLoader.Constants;
using SharpLoader.Services.Contracts;

namespace SharpLoader.Services.Implementations
{
    public class YoutubePlaylistLinksService : IYoutubePlaylistLinksService
    {
        public string[] ExtractPlaylistLinks(string playlistUrl)
        {
            var web = new HtmlWeb();
            var doc = web.Load(playlistUrl);
            var links = doc.DocumentNode.SelectNodes("//a[contains(@class, 'playlist-video')]");
            var urls = links
                .Select(link =>
                {
                    var decodedUrl = HttpUtility.UrlDecode(link.GetAttributeValue("href", string.Empty)) ?? string.Empty;
                    var indexOfSecondParameter = decodedUrl.IndexOf("&", StringComparison.Ordinal);
                    decodedUrl = decodedUrl.Substring(0, indexOfSecondParameter);
                    return DomainsConstants.HttpsYouTube + decodedUrl;
                })
                .Distinct()
                .ToArray();
            return urls;
        }
    }
}
