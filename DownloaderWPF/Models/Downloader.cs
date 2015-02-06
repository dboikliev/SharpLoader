using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpLoader.Models
{
    /// <summary>
    /// Contains the logic for downloading a video and gives information about the download progress and speed. 
    /// </summary>
    static class Downloader
    {   
        private static long totalDownloadedBytes = 0;
        private static long currentVideoSize = 0;
        private static long bytesDownloadedPerSecond = 0;

        public static event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;
        public static event EventHandler<SpeedUpdatedEventArgs> SpeedUpdated;

        private static void OnProgressUpdated(ProgressUpdatedEventArgs e)
        {
            EventHandler<ProgressUpdatedEventArgs> temp = Interlocked.CompareExchange(ref ProgressUpdated, null, null);
            if (temp != null)
            {
                temp(typeof(Downloader), e);
            }
        }

        private static void OnSpeedUpdated(SpeedUpdatedEventArgs e)
        {
            EventHandler<SpeedUpdatedEventArgs> temp = Interlocked.CompareExchange(ref SpeedUpdated, null, null);
            if (temp != null)
            {
                SpeedUpdated(typeof(Downloader), e);
            }
        }

        public static void Download(VideoInfo video, string downloadLocation, CancellationToken token)
        {
            totalDownloadedBytes = 0;
            currentVideoSize = video.FileSize;

            int millisecondsInSecond = 1000;
            int dueTime = 0;
            using (Timer timer = new Timer(UpdateSpeed, null, dueTime, millisecondsInSecond))
            {
                FileStream stream = File.Create(downloadLocation);
                stream.Dispose();
                foreach (FileSegment[] segmentGroup in video.Segments)
                {
                    Parallel.ForEach(segmentGroup, (segment) =>
                    {
                        token.ThrowIfCancellationRequested();
                        DownloadSegment(video.DownloadUrl, downloadLocation, segment);
                    });
                }
            }
        }

        private static void DownloadSegment(string videoUrl, string downloadLocation, FileSegment segment)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(videoUrl);
            request.AddRange(segment.Start, segment.End);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (FileStream fileStream = File.Open(downloadLocation, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    fileStream.Position = segment.Start;
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        byte[] data = reader.ReadBytes((int)segment.Length);

                        totalDownloadedBytes += data.Length;
                        UpdateProgress();

                        using (BinaryWriter writer = new BinaryWriter(fileStream))
                        {
                            writer.Write(data);
                            bytesDownloadedPerSecond += data.Length;
                        }
                    }
                }
            }
            response.Close();
        }

        private static void UpdateSpeed(object obj)
        {
            SpeedUpdatedEventArgs speedArgs = new SpeedUpdatedEventArgs((bytesDownloadedPerSecond / 1024D / 1024D));
            OnSpeedUpdated(speedArgs);
            bytesDownloadedPerSecond = 0;
        }

        private static void UpdateProgress()
        {
            ProgressUpdatedEventArgs progressArgs = new ProgressUpdatedEventArgs();
            progressArgs.Progress = (int)(100.0 * totalDownloadedBytes / currentVideoSize);
            OnProgressUpdated(progressArgs);
        }
    }
}
