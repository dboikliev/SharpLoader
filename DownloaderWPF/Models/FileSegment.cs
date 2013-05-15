namespace DownloaderWPF.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    /// <summary>
    /// Represents a small section of a file.
    /// </summary>
    struct FileSegment
    {   
        /// <summary>
        /// Beginning of the segment.
        /// </summary>
        public long Start { get; private set; }

        /// <summary>
        /// End of the segment.
        /// </summary>
        public long End { get; private set; }

        /// <summary>
        /// The segment's size in bytes.
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// Sets the Start and End properties and calculates the Length property of the new FileSegment object. 
        /// </summary>
        /// <param name="start">Beginning position of the segment.</param>
        /// <param name="end">End position of the segment.</param>
        public FileSegment(long start, long end) : this()
        {
            this.Start = start;
            this.End = end;
            this.Length = end - start + 1;
        }

        public override string ToString()
        {
            string rangeAsString = string.Format("Start: {0}, End: {1}, Length: {2}", this.Start, this.End, this.Length);
            return rangeAsString;
        }

        /// <summary>
        /// Splits the length of the file (in bytes) into a list of FileSegment arrays.
        /// </summary>
        /// <param name="length">The size of the file in bytes.</param>
        /// <param name="bytesPerSegment">The bytes in one segment.</param>
        /// <param name="segmentsPerGroup">The number of segments in one group.</param>
        /// <returns></returns>
        public static IEnumerable<FileSegment[]> SplitLengthIntoSegmentGroups(long length, long bytesPerSegment = 1024 * 1024, int segmentsPerGroup = 10)
        {
            long start = 0;
            long end = start + bytesPerSegment;
            long remainingBytes = length;

            List<FileSegment[]> allSegments = new List<FileSegment[]>();

            while (remainingBytes > 0)
            {
                remainingBytes -= (end - start);
                end = Math.Min(end, length);

                FileSegment[] segments = SplitIntoSegments(start, end, segmentsPerGroup);
                allSegments.Add(segments);

                start = end + 1;
                end = start + bytesPerSegment;
            }

            return allSegments;
        }

        /// <summary>
        /// Splits a length into segments.
        /// </summary>
        /// <param name="from">Start position.</param>
        /// <param name="to">End position/</param>
        /// <param name="segmentCount">The number of segments in the group.</param>
        /// <returns>Array of FileSegment</returns>
        private static FileSegment[] SplitIntoSegments(long from, long to, int segmentCount)
        {
            long length = to - from;
            long bytesPerRange = length / segmentCount;
            FileSegment[] ranges = new FileSegment[segmentCount];

            long end = from + bytesPerRange;
            FileSegment segment = new FileSegment(from, end);
            ranges[0] = segment;
            for (int i = 1; i < segmentCount; i++)
            {
                from = end + 1;
                end = from + bytesPerRange;
                if (end > to)
                {
                    end = to;
                }

                segment = new FileSegment(from, end);
                ranges[i] = segment;
            }
            return ranges;
        }

        //public IEnumerable<Segment> GetAllSegments()
        //{
        //    List<Segment> segments = new List<Segment>();

        //    long remainingBytes = this.FileSize;
        //    long start = 0;
        //    long end = start + bytesPerSegment;
        //    while (remainingBytes > 0)
        //    {
        //        remainingBytes -= (end - start);
        //        end = Math.Min(end, this.FileSize);

        //        Segment segment = new Segment(start, end);
        //        segments.Add(segment);

        //        start = end + 1;
        //        end = start + bytesPerSegment;
        //    }
        //    return segments;
        //}
    }
}