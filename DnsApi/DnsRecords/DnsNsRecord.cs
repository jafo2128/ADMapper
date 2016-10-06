namespace DnsApi.DnsRecords
{
    public class DnsNsRecord : DnsRecordBase
    {
        public DnsNsRecord(string nameHost)
        {
            NameHost = nameHost;
        }

        public string NameHost { get; private set; }

        public override string ToString()
        {
            return NameHost;
        }
    }
}
