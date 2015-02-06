using System;

namespace SharpLoader.Models
{
    /// <summary>
    /// Provides information about the current speed of the download.
    /// </summary>
    class SpeedUpdatedEventArgs : EventArgs
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
