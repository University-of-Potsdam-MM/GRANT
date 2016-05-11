using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{ 
    public struct BrailleRepresentation
    {
        Position position { get; set; }
        Content content { get; set; }
        String screenName { get; set; }
    }
}
