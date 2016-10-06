namespace DnsApi.DnsRecords
{
    public class DnsNsRecord : IDnsRecord
    {
        public DnsNsRecord(string nameHost)
        {
            NameHost = nameHost;
        }

        public string NameHost { get; private set; }
    }
}
