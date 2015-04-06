using System.Collections;
using System.Collections.Generic;

namespace SharpLoader.Models.Downloader
{
    public class VideoFileSegmentGroup : IEnumerable<Range>
    {
        private IEnumerable<Range> Segments { get; set; }

        public VideoFileSegmentGroup(IEnumerable<Range> segments)
        {
            Segments = segments;
        }

        public IEnumerator<Range> GetEnumerator()
        {
            return Segments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Segments).GetEnumerator();
        }
    }
}
