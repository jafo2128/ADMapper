namespace DnsApi.DnsRecords
{
    public class DnsPtrRecord : IDnsRecord
    {
        public DnsPtrRecord(string nameHost)
        {
            NameHost = nameHost;
        }

        public string NameHost { get; private set; }
    }
}
