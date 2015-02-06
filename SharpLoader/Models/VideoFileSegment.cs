using System;
using System.Collections.Generic;

namespace SharpLoader.Models
{
    /// <summary>
    /// Represents a part of a video file.
    /// </summary>
    struct VideoFileSegment
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
        /// Sets the Start and End properties and calculates the Length property of the new VideoFileSegment object. 
        /// </summary>
        /// <param name="start">Beginning position of the segment.</param>
        /// <param name="end">End position of the segment.</param>
        public VideoFileSegment(long start, long end) : this()
        {
            Start = start;
            End = end;
            Length = end - start + 1;
        }

        public override string ToString()
        {
            var rangeAsString = string.Format("Start: {0}, End: {1}, Length: {2}", Start, End, Length);
            return rangeAsString;
        }

        /// <summary>
        /// Splits the length of the file (in bytes) into a list of VideoFileSegment arrays.
        /// </summary>
        /// <param name="length">The size of the file in bytes.</param>
        /// <param name="bytesPerSegment">The bytes in one segment.</param>
        /// <param name="segmentsPerGroup">The number of segments in one group.</param>
        /// <returns></returns>
        public static IEnumerable<VideoFileSegment[]> SplitLengthIntoSegmentGroups(long length, long bytesPerSegment = 1024 * 1024, int segmentsPerGroup = 10)
        {
            long start = 0;
            var end = start + bytesPerSegment;
            var remainingBytes = length;

            var allSegments = new List<VideoFileSegment[]>();

            while (remainingBytes > 0)
            {
                remainingBytes -= (end - start);
                end = Math.Min(end, length);

                var segments = SplitIntoSegments(start, end, segmentsPerGroup);
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
        /// <returns>Array of VideoFileSegment</returns>
        private static VideoFileSegment[] SplitIntoSegments(long from, long to, int segmentCount)
        {
            var length = to - from;
            var bytesPerRange = length / segmentCount;
            var ranges = new VideoFileSegment[segmentCount];

            var end = from + bytesPerRange;
            var segment = new VideoFileSegment(from, end);
            ranges[0] = segment;
            for (var i = 1; i < segmentCount; i++)
            {
                from = end + 1;
                end = from + bytesPerRange;
                if (end > to)
                {
                    end = to;
                }

                segment = new VideoFileSegment(from, end);
                ranges[i] = segment;
            }
            return ranges;
        }
    }
}