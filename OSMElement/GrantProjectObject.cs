using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    [Serializable]
    public struct GrantProjectObject
    {
        public String grantBrailleStrategyFullName { get; set; }
        public String grantBrailleStrategyNamespace { get; set; }
        public String grantDisplayStrategyFullName { get; set; }
        public String grantDisplayStrategyNamespace { get; set; }
        public String grantTreeStrategyFullName { get; set; }
        public String grantTreeStrategyNamespace { get; set; }
        public String grantTreeOperationsFullName { get; set; }
        public String grantTreeOperationsNamespace { get; set; }
        //public String grantFilterStrategyFullName { get; set; } -> ist im gefilterten Baum
        //public String grantFilterStrategyNamespace { get; set; } -> ist im gefilterten Baum
        public String grantOperationSystemStrategyFullName { get; set; }
        public String grantOperationSystemStrategyNamespace { get; set; }

        public Device device { get; set; }

    }
}
