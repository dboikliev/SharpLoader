using System.Threading.Tasks;

namespace SharpLoader.Models.Video
{
    public interface IVideoInfoExtractor
    {
        Task<VideoInfo> GetVideoInfo(string videoUrl);
    }
}
