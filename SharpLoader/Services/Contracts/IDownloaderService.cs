using System;
using System.Threading.Tasks;
using SharpLoader.Models.Downloader;
using SharpLoader.Models.Video;

namespace SharpLoader.Services.Contracts
{
    interface IDownloaderService
    {
        event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;
        event EventHandler<SpeedUpdatedEventArgs> SpeedUpdated;
        event EventHandler<DownloadFinishedEventArgs> DownloadFinished;

        void BeginDownload(VideoInfo videoInfo, string downloadLocation);
    }
}
