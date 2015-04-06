using System.Threading.Tasks;
using SharpLoader.Models.Video;

namespace SharpLoader.Services
{
    public interface IVideoInfoService
    {
        Task<VideoInfo> GetVideoInfo(string videoUrl);
    }
}
