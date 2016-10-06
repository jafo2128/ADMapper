namespace DnsApi.DnsRecords
{
    class DnsCnameRecord : DnsRecordBase
    {
        public DnsCnameRecord(string nameHost)
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
