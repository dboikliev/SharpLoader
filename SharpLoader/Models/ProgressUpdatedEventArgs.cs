using System;

namespace SharpLoader.Models
{
    sealed class ProgressUpdatedEventArgs : EventArgs
    {
        public long Progress { get; set; }

        public ProgressUpdatedEventArgs() { }

        public ProgressUpdatedEventArgs(int progress)
        {
            Progress = progress;
        }
    }
}
