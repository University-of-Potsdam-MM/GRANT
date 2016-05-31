using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    /// <summary>
    /// Gibt die Beziehungen zwischen dem gespiegelten Baum und der Braille-Darstellung an über "IdGenerated"
    /// </summary>
    public struct osmRelationship
    {//http://stackoverflow.com/questions/166089/what-is-c-sharp-analog-of-c-stdpair


        public class OsmRelationship<T, U>
        {
            public OsmRelationship()
            {
            }

            public OsmRelationship(T first, U second)
            {
                this.First = first;
                this.Second = second;
            }

            /// <summary>
            /// First bezieht sich auf das Element im gespiegelten Baum
            /// </summary>
            public T First { get; set; }
            /// <summary>
            /// Second bezieht sich auf das Element der Braille-GUI
            /// </summary>
            public U Second { get; set; }
        };

       // public List<OsmRelationship<String, String>> relationship { get; set; }
    }
}
