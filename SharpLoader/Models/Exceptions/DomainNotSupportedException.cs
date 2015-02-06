using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VboxLoader.Models.Exceptions
{
    class DomainNotSupportedException : Exception
    {
        public string Domain { get; private set; }

        public DomainNotSupportedException(string domain) : base(string.Format("The domain {0} is currently not supported.", domain))
        {
            this.Domain = domain;
        }
    }
}
