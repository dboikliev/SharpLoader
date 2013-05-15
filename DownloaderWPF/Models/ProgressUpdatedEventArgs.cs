using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DownloaderWPF.Models
{
    sealed class ProgressUpdatedEventArgs : EventArgs
    {
        public long Progress { get; set; }

        public ProgressUpdatedEventArgs()
        {
            
        }

        public ProgressUpdatedEventArgs(int progress)
        {
            this.Progress = progress;
        }
    }
}
