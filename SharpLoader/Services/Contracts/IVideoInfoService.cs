using SharpLoader.Models.Video;

namespace SharpLoader.Services.Contracts
{
    public interface IVideoInfoService
    {
        VideoInfo GetVideoInfo(string videoUrl);
    }
}
