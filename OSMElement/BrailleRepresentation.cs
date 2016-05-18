using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{ 
    public struct BrailleRepresentation
    {
        public Position position { get; set; }
        public Content content { get; set; }
        public String screenName { get; set; }
    }
}
