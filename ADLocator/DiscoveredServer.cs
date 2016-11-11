using System.Collections.Generic;
using System.Net;
using CLDAP.NET;

namespace ADLocator
{
    public class DiscoveredServer
    {
        public string Name { get; set; }

        public IList<IPAddress> Addresses { get; set; }

        public PingResponse Information { get; set; }
    }
}
