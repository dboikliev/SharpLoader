using System;
using System.Windows.Media.Imaging;

namespace SharpLoader.Models.VideoInfo
{
    class YouTubeVideoInfo : VideoInfoBase
    {
        public YouTubeVideoInfo(string videoUrl) : base(videoUrl)
        {
            
        }

        protected override string GetVideoDownloadUrl()
        {
            throw new NotImplementedException();
        }
    }
}
