using System.Linq;

namespace DnsApi.DnsRecords
{
    public class DnsTxtRecord : DnsRecordBase
    {
        public DnsTxtRecord(string[] stringArray)
        {
            StringArray = stringArray;
        }

        public string[] StringArray { get; private set; }

        public override string ToString()
        {
            return string.Join(";", StringArray);
        }
    }
}
