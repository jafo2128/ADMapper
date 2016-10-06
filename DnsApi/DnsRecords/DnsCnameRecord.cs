namespace DnsApi.DnsRecords
{
    class DnsCnameRecord : IDnsRecord
    {
        public DnsCnameRecord(string nameHost)
        {
            NameHost = nameHost;
        }

        public string NameHost { get; private set; }
    }
}
