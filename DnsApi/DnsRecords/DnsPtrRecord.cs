namespace DnsApi.DnsRecords
{
    public class DnsPtrRecord : DnsRecordBase
    {
        public DnsPtrRecord(string nameHost)
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
