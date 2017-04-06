using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OSMElement
{
    [Serializable]
    public struct OSMElement
    {
        public GeneralProperties properties { get; set; }

        public Events events { get; set; }
        public BrailleRepresentation brailleRepresentation { get; set; }

        public override string ToString()
        {
            return String.Format("GeneralProperties: {0}, BrailleRepresentation: {1}", properties.ToString(), brailleRepresentation.ToString());
        }
    }
}
