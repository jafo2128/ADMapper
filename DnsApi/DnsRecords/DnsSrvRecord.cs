namespace DnsApi.DnsRecords
{
    public class DnsSrvRecord : IDnsRecord
    {
        public DnsSrvRecord(ushort priority, ushort weight, string nameTarget, ushort port)
        {
            Priority = priority;
            Weight = weight;
            NameTarget = nameTarget;
            Port = port;
        }

        public ushort Priority { get; private set; }

        public ushort Weight { get; private set; }
        
        public string NameTarget { get; private set; }

        public ushort Port { get; private set; }
    }
}
