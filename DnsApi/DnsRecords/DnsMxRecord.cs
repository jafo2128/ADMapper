namespace DnsApi.DnsRecords
{
    public class DnsMxRecord : DnsRecordBase
    {
        public DnsMxRecord(ushort preference, string nameExchange)
        {
            Preference = preference;
            NameExchange = nameExchange;
        }

        public ushort Preference { get; private set; }

        public string NameExchange { get; private set; }

        public override string ToString()
        {
            return $"{Preference} {NameExchange}";
        }
    }
}
