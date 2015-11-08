using System;

namespace SharpLoader.Models.Downloader
{
    public class DownloadFinishedEventArgs : EventArgs
    {
        public string DownloadFileName { get; set; }

        public DownloadFinishedEventArgs(string downloadFileName)
        {
            DownloadFileName = downloadFileName;
        }
    }
}
