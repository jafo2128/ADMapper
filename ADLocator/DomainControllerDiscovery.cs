using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CLDAP.NET;
using Win32DnsApi;
using Win32DnsApi.DnsRecords;

namespace ADLocator
{
    public static class DomainControllerDiscovery
    {
        private class SrvResourcePrefix
        {
            public string Type { get; set; }

            public string Prefix { get; set; }
        }

        private static readonly SrvResourcePrefix[] _srvResourcePrefixes = new[]
        {
            new SrvResourcePrefix {Type = "All", Prefix = "_ldap._tcp."},
            new SrvResourcePrefix {Type = "DC", Prefix = "_ldap._tcp.dc._msdcs."},
            new SrvResourcePrefix {Type = "PDC", Prefix = "_ldap._tcp.pdc._msdcs."},
            new SrvResourcePrefix {Type = "GC", Prefix = "_ldap._tcp.gc._msdcs."}
        };
        
        private static Task<IList<DnsSrvRecord>> DnsSrvLookupAsync(string query)
        {
            return Task.Run(() => DnsQuery.LookUp<DnsSrvRecord>(query, true));
        }

        private static Task<IList<DnsARecord>> DnsALookupAsync(string query)
        {
            return Task.Run(() => DnsQuery.LookUp<DnsARecord>(query, true));
        }

        private static Task<IList<DnsAaaaRecord>> DnsAaaaLookupAsync(string query)
        {
            return Task.Run(() => DnsQuery.LookUp<DnsAaaaRecord>(query, true));
        }

        private static Task<PingResponse> PingServerAsync(string dnsName, IPAddress ipAddress)
        {
            return Task.Run(() => Cldap.Ping(dnsName, ipAddress, 389));
        }

        

        public static void Discover(string dnsName)
        {


            var srvRecords = DnsQuery.LookUp<DnsSrvRecord>(dnsName, true);
            foreach (var record in srvRecords)
            {
                
            }
        }
    }
}
