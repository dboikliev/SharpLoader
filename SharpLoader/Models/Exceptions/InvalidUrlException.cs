using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            this.Url = url;
        }
    }
}
