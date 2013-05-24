using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DownloaderWPF.Models
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
            this.Speed = speed;
        }
    }
}
