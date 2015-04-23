using System.Threading.Tasks;

namespace SharpLoader.Helpers
{
    public interface IHtmlDownloader
    {
        Task<string> DownloadHtmlAsync(string url);
    }
}
