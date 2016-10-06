namespace DnsApi.DnsRecords
{
    public class DnsTxtRecord : IDnsRecord
    {
        public DnsTxtRecord(string[] stringArray)
        {
            StringArray = stringArray;
        }

        public string[] StringArray { get; private set; }
    }
}
