using System;

namespace SharpLoader.Models.Downloader
{
    /// <summary>
    /// Provides information about the current speed of the download.
    /// </summary>
    public class SpeedUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Holds the download speed in MB/s as a double value.
        /// </summary>
        public double Speed { get; set; }

        public SpeedUpdatedEventArgs() { }

        public SpeedUpdatedEventArgs(double speed)
        {
            Speed = speed;
        }
    }
}
