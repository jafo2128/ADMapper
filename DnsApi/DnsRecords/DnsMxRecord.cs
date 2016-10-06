namespace DnsApi.DnsRecords
{
    public class DnsMxRecord : IDnsRecord
    {
        public DnsMxRecord(ushort preference, string nameExchange)
        {
            Preference = preference;
            NameExchange = nameExchange;
        }

        public ushort Preference { get; private set; }
        public string NameExchange { get; private set; }
    }
}
