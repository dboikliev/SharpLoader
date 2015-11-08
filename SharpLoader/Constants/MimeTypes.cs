
using System.Collections.Generic;

namespace SharpLoader.Constants
{
    public sealed class MimeTypes
    {
        private MimeTypes()
        {
        }

        private static readonly Dictionary<string, string> MymeTypesFileFormats = new Dictionary<string, string>()
        {
            ["video/mp4"] = ".mp4",
            [ "video/quicktime"] = ".mov",
            [ "application/x-troff-msvideo"] = ".avi",
            [ "video/avi"] = ".avi",
            [ "video/msvideo"] = ".avi",
            [ "video/x-msvideo"] = ".avi",
            [ "audio/x-ms-wmv"] = ".wmv"
        };

        public static MimeTypes Instance { get; } = new MimeTypes();

        public string this[string mimeType] => MymeTypesFileFormats[mimeType];
    }
}
