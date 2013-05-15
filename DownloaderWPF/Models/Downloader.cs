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

namespace DownloaderWPF.Models
{
    static class Downloader
    {
        // public string Url { get; protected set; }
        private static long downloadedBytes = 0;
        private static long currentVideoSize = 0;

        public static event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

        private static void OnProgressUpdated(ProgressUpdatedEventArgs e)
        {
            EventHandler<ProgressUpdatedEventArgs> temp = Interlocked.CompareExchange(ref ProgressUpdated, null, null);
            if (temp != null)
            {
                temp(typeof(Downloader), e);
            }
        }

        public static void Download(VideoInfo video, string downloadLocation)
        {
            downloadedBytes = 0;
            currentVideoSize = video.FileSize;

            //ProgressUpdatedEventArgs e = new ProgressUpdatedEventArgs();
            //foreach (Segment segment in video.Segments)
            //{
            //    DownloadSegmentAsync(video.DownloadUrl, downloadLocation, segment);
            //    e.Progress = (int)(((double)Thread.VolatileRead(ref downloadedBytes) / currentVideoSize) * 100);
            //    ProgressUpdated(typeof(Downloader), e);
            //}


            //ProgressUpdatedEventArgs e = new ProgressUpdatedEventArgs();
            foreach (FileSegment[] segments in video.Segments)
            {

                Parallel.ForEach(segments, (segment) =>
                //DownloadFile(video.DownloadUrl, downloadLocation, video.Segments);
                //foreach (Segment segment in video.Segments)
                {
                    DownloadSegment(video.DownloadUrl, downloadLocation, segment);
                });
                Console.WriteLine();
            }
        }
        
        private static void DownloadSegmentAsync(string videoUrl, string downloadLocation, FileSegment segment)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(videoUrl);
            request.AddRange(segment.Start, segment.End);
            request.BeginGetResponse((result) =>
                {
                    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
                    
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (FileStream fileStream = File.Open(downloadLocation, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                        {
                            fileStream.Position = segment.Start;
                            using (BinaryReader reader = new BinaryReader(stream))
                            {
                                byte[] data = reader.ReadBytes((int)segment.Length);
                                //Interlocked.Add(ref downloadedBytes, data.Length);
                                using (BinaryWriter writer = new BinaryWriter(fileStream))
                                {
                                    writer.Write(data);

                                    Interlocked.Add(ref downloadedBytes, data.Length);
                                    ProgressUpdatedEventArgs e = new ProgressUpdatedEventArgs();
                                    e.Progress = (int)(100.0 * downloadedBytes / currentVideoSize);
                                    ProgressUpdated(typeof(Downloader), e);
                                }

                                //downloadedBytes += data.Length;
                                
                            }
                        }
                    }
                }, request);
        }

        private static void DownloadSegment(string videoUrl, string downloadLocation, FileSegment segment)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(videoUrl);
            request.AddRange(segment.Start, segment.End);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (FileStream fileStream = File.Open(downloadLocation, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    fileStream.Position = segment.Start;
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        byte[] data = reader.ReadBytes((int)segment.Length);
                        Interlocked.Add(ref downloadedBytes, data.Length);
                        ProgressUpdatedEventArgs e = new ProgressUpdatedEventArgs();
                        e.Progress = (int)(100.0 * Thread.VolatileRead(ref downloadedBytes) / currentVideoSize);
                        ProgressUpdated(typeof(Downloader), e);
                        using (BinaryWriter writer = new BinaryWriter(fileStream))
                        {
                            writer.Write(data);
                        }
                    }
                }
            }
            response.Close();
        }
    }
}
