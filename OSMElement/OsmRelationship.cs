using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{

    /// <summary>
    /// Gibt die Beziehungen zwischen dem gefilterten Baum und der Braille-Darstellung an über "IdGenerated"
    /// http://stackoverflow.com/questions/166089/what-is-c-sharp-analog-of-c-stdpair
    /// </summary>
    /// <typeparam name="T">gibt die IdGenerated des Knotens im gefilterten Baum an</typeparam>
    /// <typeparam name="U">gibt die IdGenerated des Knotens im Baum der Braille-Darstellung an</typeparam>
    [Serializable]
    public class OsmRelationship<T, U>
    {
        public OsmRelationship()
        {
        }

        public OsmRelationship(T filteredTree, U brailleTree)
        {
            this.FilteredTree = filteredTree;
            this.BrailleTree = brailleTree;
        }

        /// <summary>
        /// bezieht sich auf das Element im gefiltert Baum
        /// </summary>
        public T FilteredTree { get; set; }
        /// <summary>
        /// bezieht sich auf das Element der Braille-GUI
        /// </summary>
        public U BrailleTree { get; set; }
    }

}
