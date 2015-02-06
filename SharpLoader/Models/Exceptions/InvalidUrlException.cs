using System;

namespace VboxLoader.Models.Exceptions
{
    class InvalidUrlException : Exception
    {
        public string Url { get; set; }

        public InvalidUrlException()
        {
        }

        public InvalidUrlException(string url)
        {
            Url = url;
        }
    }
}
