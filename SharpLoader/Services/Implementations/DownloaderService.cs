using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SharpLoader.Constants;
using SharpLoader.Models.Downloader;
using SharpLoader.Models.Video;
using SharpLoader.Services.Contracts;
using SharpLoader.Utils;

namespace SharpLoader.Services.Implementations
{
    /// <summary>
    /// Contains the logic for downloading a video and gives information about the download progress and speed. 
    /// </summary>
    public class DownloaderService : IDownloaderService
    {
        private long _totalDownloadedBytes;
        private long _currentVideoSize;
        private long _bytesDownloadedPerSecond;

        public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;
        public event EventHandler<SpeedUpdatedEventArgs> SpeedUpdated;
        public event EventHandler<DownloadFinishedEventArgs> DownloadFinished;

        public void BeginDownload(VideoInfo videoInfo, string downloadLocation)
        {
            Task.Factory.StartNew(() =>
            {
                DownloadFile(videoInfo, downloadLocation);
                _bytesDownloadedPerSecond = 0;
                UpdateSpeed();
                EventUtils.RaiseEvent(this, new DownloadFinishedEventArgs(downloadLocation), ref DownloadFinished);
            });
        }

        private void DownloadFile(VideoInfo video, string downloadLocation)
        {
            _totalDownloadedBytes = 0;
            _currentVideoSize = video.FileSize;

            const int millisecondsInSecond = 1000;
            const int dueTime = 0;

            using (new Timer(_ => UpdateSpeed(), null, dueTime, millisecondsInSecond))
            {
                DownloadRanges(video, downloadLocation);
            }
        }

        private void DownloadRanges(VideoInfo video, string downloadLocation)
        {
            var ranges = Range.SplitLengthIntoRanges(video.FileSize, DownloaderContants.SegmentSizeInBytes);
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            CreateCleanFile(downloadLocation);
            Parallel.ForEach(ranges, options, range => DownloadRange(video.DownloadUrl, downloadLocation, range));
        }

        private void CreateCleanFile(string downloadLocation)
        {
            using (File.Create(downloadLocation)) { }
        }

        private void DownloadRange(string videoUrl, string downloadLocation, Range segment)
        {
            var request = (HttpWebRequest)WebRequest.Create(videoUrl);
            request.AddRange(segment.Start, segment.End);

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var webStream = response.GetResponseStream())
            using (var fileStream = File.Open(downloadLocation, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                fileStream.Position = segment.Start;
                webStream?.CopyTo(fileStream);
                var bytesWritten = fileStream.Position - segment.Start;
                _totalDownloadedBytes += bytesWritten;
                _bytesDownloadedPerSecond += bytesWritten;
                UpdateProgress();
            }
        }

        private void UpdateSpeed()
        {
            var megabytes = _bytesDownloadedPerSecond / 1024.0 / 1024.0;
            var speedArgs = new SpeedUpdatedEventArgs(megabytes);
            EventUtils.RaiseEvent(this, speedArgs, ref SpeedUpdated);
            _bytesDownloadedPerSecond = 0;
        }

        private void UpdateProgress()
        {
            var progressArgs = new ProgressUpdatedEventArgs
            {
                Progress = (int)(100.0 * _totalDownloadedBytes / _currentVideoSize)
            };
            EventUtils.RaiseEvent(this, progressArgs, ref ProgressUpdated);
        }
    }
}
