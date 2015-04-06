using System;

namespace SharpLoader.Exceptions
{
    class DomainNotSupportedException : Exception
    {
        public string Domain { get; private set; }

        public DomainNotSupportedException(string domain) : base(string.Format("The domain {0} is currently not supported.", domain))
        {
            Domain = domain;
        }
    }
}
