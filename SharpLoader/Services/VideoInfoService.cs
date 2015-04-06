using System.Collections.Generic;
using System.Threading.Tasks;
using SharpLoader.Constants;
using SharpLoader.DependencyInjection;
using SharpLoader.Models.Video;

namespace SharpLoader.Services
{
    public class VideoInfoService : IVideoInfoService
    {
        private static readonly Dictionary<string, IVideoInfoExtractor> VideoInfoExtractors;

        static VideoInfoService()
        {
            VideoInfoExtractors = new Dictionary<string, IVideoInfoExtractor>
            {
                { DomainsConstants.Vbox7, new Vbox7VideoInfoExtractor() },
                { DomainsConstants.YouTube, new YouTubeVideoInfoExtractor() },
                { DomainsConstants.Vimeo, null },
                { DomainsConstants.Pornhub, null },
                { DomainsConstants.XVideos, null }
            };
        }

        private readonly IUrlService urlService;

        public VideoInfoService()
        {
            urlService = DependencyResolver.Instance.Resolve<IUrlService>();
        }

        public async Task<VideoInfo> GetVideoInfo(string videoUrl)
        {
            var domain = urlService.GetDomainFromUrl(videoUrl);
            var videoInfoStrategy = VideoInfoExtractors[domain];
            var videoInfo = await videoInfoStrategy.GetVideoInfo(videoUrl);
            return  videoInfo;
        }
    }
}
