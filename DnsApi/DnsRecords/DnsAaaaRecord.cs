using System.Net;

namespace DnsApi.DnsRecords
{
    public class DnsAaaaRecord : IDnsRecord
    {
        public DnsAaaaRecord(IPAddress iPv6Address)
        {
            IPv6Address = iPv6Address;
        }

        // ReSharper disable once InconsistentNaming
        public IPAddress IPv6Address { get; private set; }
    }
}
