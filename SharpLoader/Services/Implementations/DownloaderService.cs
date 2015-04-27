using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SharpLoader.Constants;
using SharpLoader.Models.Downloader;
using SharpLoader.Models.Video;
using SharpLoader.Services.Contracts;

namespace SharpLoader.Services.Implementations
{
    /// <summary>
    /// Contains the logic for downloading a video and gives information about the download progress and speed. 
    /// </summary>
    public class DownloaderService : IDownloaderService
    {
        private long totalDownloadedBytes;
        private long currentVideoSize;
        private long bytesDownloadedPerSecond;

        public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;
        public event EventHandler<SpeedUpdatedEventArgs> SpeedUpdated;

        private void RaiseEvent<T>(T args, ref EventHandler<T> eventHandler) where T : EventArgs
        {
            var temp = Interlocked.CompareExchange(ref eventHandler, null, null);
            if (temp != null)
            {
                temp(this, args);
            }
        }

        public void BeginDownload(VideoInfo videoInfo, string downloadLocation)
        {
            var task = new Task(() =>
            {
                DownloadFile(videoInfo, downloadLocation);
            });
            task.Start();
        }

        private void DownloadFile(VideoInfo video, string downloadLocation)
        {
            totalDownloadedBytes = 0;
            currentVideoSize = video.FileSize;

            const int millisecondsInSecond = 1000;
            const int dueTime = 0;

            using (new Timer((obj) => UpdateSpeed(), null, dueTime, millisecondsInSecond))
            {
                DownloadSegmentGroups(video, downloadLocation);
            }
        }

        private void DownloadSegmentGroups(VideoInfo video, string downloadLocation)
        {
            var segments = Range.SplitLengthIntoRanges(video.FileSize, DownloaderContants.SegmentSizeInBytes).ToArray();
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(segments, options, range => DownloadRange(video.DownloadUrl, downloadLocation, range));
        }

        private void DownloadRange(string videoUrl, string downloadLocation, Range segment)
        {
            var request = (HttpWebRequest)WebRequest.Create(videoUrl);
            request.AddRange(segment.Start, segment.End);

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var fileStream = File.Open(downloadLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                fileStream.Position = segment.Start;
                using (var reader = new BinaryReader(stream))
                {
                    var data = reader.ReadBytes((int)segment.Length);

                    totalDownloadedBytes += data.Length;
                    UpdateProgress();

                    using (var writer = new BinaryWriter(fileStream))
                    {
                        writer.Write(data);
                        bytesDownloadedPerSecond += data.Length;
                    }
                }
            }
        }

        private void UpdateSpeed()
        {
            var megabytes = bytesDownloadedPerSecond / 1024.0 / 1024.0;
            var speedArgs = new SpeedUpdatedEventArgs(megabytes);
            RaiseEvent(speedArgs, ref SpeedUpdated);
            bytesDownloadedPerSecond = 0;
        }

        private void UpdateProgress()
        {
            var progressArgs = new ProgressUpdatedEventArgs
            {
                Progress = (int) (100.0 * totalDownloadedBytes / currentVideoSize)
            };
            RaiseEvent(progressArgs, ref ProgressUpdated);
        }
    }
}
