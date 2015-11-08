
using System;
using FFMPEG.Implementations;

namespace FFMPEG.Interfaces
{
    public interface IFfmpegEncoder
    {
        event EventHandler<VideoEncodingEventArgs> ProgressUpdated;

        void EncodeToAvi(string fileName, string convertedFileName);
    }
}
