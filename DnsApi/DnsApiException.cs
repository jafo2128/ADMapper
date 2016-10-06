using System;
using System.Runtime.Serialization;

namespace DnsApi
{
    [Serializable]
    public class DnsApiException : Exception
    {
        public DnsApiException()
            : base("Unknown DnsApiException")
        {
        }

        public DnsApiException(string message)
            : base(message)
        {
        }

        public DnsApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DnsApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
