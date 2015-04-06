using System;
using System.Threading.Tasks;
using SharpLoader.Models.Video;
using SharpLoader.Services.Contracts;

namespace SharpLoader.Services.Implementations
{
    public class YouTubeVideoInfoService : IVideoInfoService
    {
        public async Task<VideoInfo> GetVideoInfo(string videoUrl)
        {
            throw new NotImplementedException();
        }
    }
}