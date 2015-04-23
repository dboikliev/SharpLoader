using System.Net;
using System.Threading.Tasks;

namespace SharpLoader.Helpers
{
    public class HtmlDownloader : IHtmlDownloader
    {
        public async Task<string> DownloadHtmlAsync(string url)
        {
            using (var webClient = new WebClient())
            {
                var html = await webClient.DownloadStringTaskAsync(url);
                return html;
            }
        }
    }
}
