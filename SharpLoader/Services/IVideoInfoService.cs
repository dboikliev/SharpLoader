using SharpLoader.Models.VideoInfo;

namespace SharpLoader.Services
{
    public interface IVideoInfoService
    {
        VideoInfoBase GetVideoInfo(string videoUrl);
    }
}
