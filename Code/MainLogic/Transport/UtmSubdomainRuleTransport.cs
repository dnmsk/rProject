using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLogic.Transport {
    public struct UtmSubdomainRuleTransport {
        public int ID { get; set; }
        public string SubdomainName { get; set; }
        public string TargetDomain { get; set; }
    }
}
