using System.Collections.Generic;
using System.Linq;

namespace ADLocator
{
    public class DiscoveryResult
    {
        private IDictionary<string, string> _typeMap = new Dictionary<string, string>();
        private IDictionary<string, DiscoveredServer> _serverMap = new Dictionary<string, DiscoveredServer>();
        private IDictionary<string, IList<DiscoveryError>> _errorMap = new Dictionary<string, IList<DiscoveryError>>();

        public IList<DiscoveredServer> AllServers => _serverMap.Values.ToList();

        public IList<DiscoveredServer> DomainControllers
        {
            get { return null; }
        }
    }
}
