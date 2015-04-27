using System;
using System.Collections.Generic;

namespace SharpLoader.Models.Downloader
{
    /// <summary>
    /// Represents a part of a video file.
    /// </summary>
    public struct Range
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
        public Range(long start, long end) : this()
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

        public static IEnumerable<Range> SplitLengthIntoRanges(long lengthToSplit, long rangeLength)
        {
            var remaining = lengthToSplit;
            var currentPosition = 0L;
            while (remaining > 0)
            {
                var rangeStart = currentPosition;
                var rangeEnd = Math.Min(currentPosition + rangeLength, lengthToSplit);
                var range = new Range(rangeStart, rangeEnd);
                yield return range;
                remaining -= rangeLength;
                currentPosition += range.Length;
            }
        }
    }
}