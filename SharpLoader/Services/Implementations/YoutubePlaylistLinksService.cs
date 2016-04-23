using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
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
                    var prefix = "https://www.youtube.com";

                    var decodedUrl = HttpUtility.UrlDecode(link.GetAttributeValue("href", string.Empty));
                    decodedUrl = decodedUrl?.Substring(0, decodedUrl.IndexOf("&")) ?? string.Empty;
                    return prefix + decodedUrl;
                })
                .Distinct()
                .ToArray();
            return urls;
        }
    }
}
