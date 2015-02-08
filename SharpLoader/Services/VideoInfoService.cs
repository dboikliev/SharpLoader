using System;
using System.Collections.Generic;
using SharpLoader.Models.VideoInfo;
using Domains = SharpLoader.Models.Constants.Domains;

namespace SharpLoader.Services
{
    public class VideoInfoService : IVideoInfoService
    {
        private readonly Dictionary<string, Func<string, VideoInfoBase>> videoInfoFactories;

        public VideoInfoService()
        {
            videoInfoFactories = new Dictionary<string, Func<string, VideoInfoBase>>
            {
                { Domains.Vbox7, videoUrl => new Vbox7VideoInfo(videoUrl) },
                { Domains.YouTube, videoUrl => new YouTubeVideoInfo(videoUrl) },
                { Domains.Vimeo, videoUrl => { throw new NotImplementedException(); }},
                { Domains.Pornhub, videoUrl => { throw new NotImplementedException(); }}
            };
        }

        public VideoInfoBase GetVideoInfo(string videoUrl)
        {
            var domain = GetDomainFromUrl(videoUrl);
            var videoInfoFactory = videoInfoFactories[domain];
            var videoInfo = videoInfoFactory(videoUrl);
            return videoInfo;
        }

        private string GetDomainFromUrl(string url)
        {
            throw new NotImplementedException();
        }
    }
}
