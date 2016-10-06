using System.Net;

namespace DnsApi.DnsRecords
{
    public class DnsARecord : IDnsRecord
    {
        public DnsARecord(IPAddress iPv4Address)
        {
            IPv4Address = iPv4Address;
        }

        // ReSharper disable once InconsistentNaming
        public IPAddress IPv4Address { get; private set; }
    }
}
