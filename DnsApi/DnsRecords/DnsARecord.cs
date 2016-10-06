using System.Net;

namespace DnsApi.DnsRecords
{
    public class DnsARecord : DnsRecordBase
    {
        public DnsARecord(IPAddress iPv4Address)
        {
            IPv4Address = iPv4Address;
        }

        // ReSharper disable once InconsistentNaming
        public IPAddress IPv4Address { get; private set; }

        public override string ToString()
        {
            return IPv4Address.ToString();
        }
    }
}
