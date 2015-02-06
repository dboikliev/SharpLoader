using System.Collections.Generic;

namespace SharpLoader.Models
{
    class VideoFile
    {
        public long FileSize { get; protected set; }

        private IEnumerable<VideoFileSegment[]> segments;

        public VideoFile(long fileSize)
        {
            segments = VideoFileSegment.SplitLengthIntoSegmentGroups(fileSize);
        }
    }
}
