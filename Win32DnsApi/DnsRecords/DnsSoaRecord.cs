namespace Win32DnsApi.DnsRecords
{
    public class DnsSoaRecord : DnsRecordBase
    {
        public DnsSoaRecord(string primaryNameServer, string administratorName, uint serialNumber, uint refresh, uint retry, uint expire, uint defaultTimeToLive)
        {
            PrimaryNameServer = primaryNameServer;
            AdministratorName = administratorName;
            SerialNumber = serialNumber;
            Refresh = refresh;
            Retry = retry;
            Expire = expire;
            DefaultTimeToLive = defaultTimeToLive;
        }

        public string PrimaryNameServer { get; private set; }

        public string AdministratorName { get; private set; }

        public uint SerialNumber { get; private set; }

        public uint Refresh { get; private set; }

        public uint Retry { get; private set; }

        public uint Expire { get; private set; }

        public uint DefaultTimeToLive { get; private set; }

        public override string ToString()
        {
            return $"{PrimaryNameServer} {AdministratorName} {SerialNumber} {Refresh} {Retry} {Expire} {DefaultTimeToLive}";
        }
    }
}
