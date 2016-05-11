using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    public struct OSMElement
    {
        public GeneralProperties properties { get; set; }
        public Events events { get; set; }
        public Interaction interaction { get; set; }
        public BrailleRepresentation brailleRepresentation { get; set; }
    }
}
