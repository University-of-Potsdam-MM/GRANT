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
        public List<OsmRelationship<String, String>> relationshipOfTrees;
        public String pathFilteredTree { get; set; }
        public String pathBrailleTree { get; set; }
        public String grantBrailleStrategyFullName { get; set; }
        public String grantBrailleStrategyNamespace { get; set; }
        public String grantDisplayStrategyFullName { get; set; }
        public String grantDisplayStrategyNamespace { get; set; }
        //+ Device
    }
}
