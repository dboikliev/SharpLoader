using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using FFMPEG.Interfaces;
using SharpLoader.Utils;

namespace FFMPEG.Implementations
{
    public class FfmpegEncoder : IFfmpegEncoder
    {
        private const string DurationRegex = "Duration: (?<duration>\\d{2}:\\d{2}:\\d{2}.\\d{2})";
        private const string ProgressRegex = "time=(?<time>\\d{2}:\\d{2}:\\d{2}.\\d{2})";

        public event EventHandler<VideoEncodingEventArgs> ProgressUpdated;

        private int? DurationInSeconds { get; set; }
        private int ProgressInSeconds { get; set; }

        public void EncodeToAvi(string fileName, string convertedFileName)
        {
            Process ffmpeg = new Process();
            ffmpeg.StartInfo.FileName =
                @"D:\Deyan\Documents\Visual Studio 2013\Repositories\SharpLoader\FFMPEG\Lib\ffmpeg\ffmpeg.exe";
            ffmpeg.StartInfo.Arguments = $"-y -i \"{fileName}\" \"{convertedFileName}.avi\" -vcodec mpeg4";
            ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ffmpeg.StartInfo.CreateNoWindow = true;
            ffmpeg.StartInfo.RedirectStandardOutput = true;
            ffmpeg.StartInfo.RedirectStandardError = true;
            ffmpeg.StartInfo.UseShellExecute = false;

            ffmpeg.ErrorDataReceived += Ffmpeg_ErrorDataReceived;
            ffmpeg.Start();
            ffmpeg.BeginErrorReadLine();
        }

        private void Ffmpeg_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!DurationInSeconds.HasValue && !string.IsNullOrEmpty(e.Data))
            {
                var match = Regex.Match(e.Data, DurationRegex);
                if (match.Success)
                {
                    var parts = match.Groups["duration"].Value.Split(':', '.');
                    DurationInSeconds = (int)(double.Parse(parts[0]) * 3600 + double.Parse(parts[1]) * 60 + double.Parse(parts[2]));
                }
            }
            else if (!string.IsNullOrEmpty(e.Data))
            {
                var match = Regex.Match(e.Data, ProgressRegex);
                if (match.Success)
                {
                    var parts = match.Groups["time"].Value.Split(':', '.');
                    ProgressInSeconds = (int)(double.Parse(parts[0]) * 3600 + double.Parse(parts[1]) * 60 + double.Parse(parts[2]));
                }
            }

            if (DurationInSeconds > 0)
            {
                UpdateProgress(DurationInSeconds.GetValueOrDefault(), ProgressInSeconds); 
            }
        }



        private void UpdateProgress(int totalDurationInSeconds, int progressInSeconds)
        {
            EventUtils.RaiseEvent(this, new VideoEncodingEventArgs() { ProgressInSeconds =  progressInSeconds, TotalDurationInSeconds =  totalDurationInSeconds}, ref ProgressUpdated);
        }

    }

    public class VideoEncodingEventArgs : EventArgs
    {
        public int TotalDurationInSeconds { get; set; }
        public int ProgressInSeconds { get; set; }
        public decimal ProgressInPercent => ProgressInSeconds * 100.0M / TotalDurationInSeconds;
    }
}
