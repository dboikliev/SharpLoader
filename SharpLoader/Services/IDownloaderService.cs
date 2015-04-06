using System;
using SharpLoader.Models.Downloader;
using SharpLoader.Models.Video;

namespace SharpLoader.Services
{
    interface IDownloaderService
    {
        event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;
        event EventHandler<SpeedUpdatedEventArgs> SpeedUpdated;
        
        void BeginDownload(VideoInfo videoInfo, string downloadLocation);
    }
}
